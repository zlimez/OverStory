using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Assertions;
using Utils;

// References:
// https://youtu.be/bxG3XP4MVzk?si=uCBvvdi91vS84dh1
// https://pikuma.com/blog/verlet-integration-2d-cloth-physics-simulation
// https://github.com/EricHu33/Verlet-Integration-In-Unity/blob/master/Assets/Scripts/VerletIntegration.cs

/* TODO:
 * 1. Add weights to endpoint to prevent slip
 * 2. Diagnose stability issues
 */
namespace VerletPhysics
{
    [Serializable]
    public struct RopeArgs
    {
        public float SegmentLength, MaxLength;
        public Transform Start, End;
        public float StartMass, EndMass;
        public int ConstrainRuns, ResolveCollisionInterval;
        public Vector2 Gravity;
        public float Drag, BounceFactor;
        public int PointUpdateBatchSize;
        public int ResolveCollisionsBatchSize;
    }
    
    public class Rope
    {
        public enum PinPoint { Start, End, Both }

        private readonly float _segmentLength;
        public readonly float MaxLength;
        public Transform Start, End;
        private readonly float _startMass, _endMass;
        
        // Physics params
        private readonly int _constrainRuns, _resolveCollisionInterval;
        private readonly Vector2 _gravity;
        private readonly float _drag, _bounceFactor;

        private readonly int _pointUpdateBatchSize, _resolveCollisionsBatchSize;

        private NativeArray<Point> _nActivePoints;
        private NativeArray<(Vector2 force, bool isPersistent)> _nPointForces;
        private NativeArray<Stick> _nSticks;
        private NativeArray<C_Collider> _nColliders;
        private NativeArray<Vector2> _nColVertices;
        private JobHandle[] _jobHandles;
        private int _lastJobInd;
        private bool _stepStarted;
        
        public float Width => _segmentLength * 0.5f;
        public Vector2 EndVel { get; private set; } = Vector2.zero;

        private bool _willAdjLen;
        private float _lenChange;
        private PinPoint _pinPoint = PinPoint.Both, _nxtPinPoint = PinPoint.Both;
        
        private readonly List<(int pointIdx, Vector2 force, bool isPersistent)> _forceQueue = new();

        public Rope(RopeArgs args)
        {
            _segmentLength = args.SegmentLength;
            MaxLength = args.MaxLength;
            Start = args.Start;
            End = args.End;
            _startMass = args.StartMass;
            _endMass = args.EndMass;
            _constrainRuns = args.ConstrainRuns;
            _resolveCollisionInterval = args.ResolveCollisionInterval;
            _gravity = args.Gravity;
            _drag = args.Drag;
            _bounceFactor = args.BounceFactor;
            _pointUpdateBatchSize = args.PointUpdateBatchSize;
            _resolveCollisionsBatchSize = args.ResolveCollisionsBatchSize;
        }

        public float Len { get; private set; }
        public int PointCnt { get; private set; }
        private int _stickCnt;

        #region Setup Teardown
        public void SetColliders(List<Collider2D> cols)
        {
            _nColliders = new NativeArray<C_Collider>(cols.Count, Allocator.Persistent);
            List<Vector2> colVerts = new();
            for (var i = 0; i < cols.Count; i++)
            {
                var col = cols[i];
#if UNITY_EDITOR
                Debug.Log($"Collider {i}: {col.name}");
#endif
                if (col is not (PolygonCollider2D or CircleCollider2D or BoxCollider2D))
                {
                    Debug.LogWarning($"Unsupported collider type: {col.GetType()}");
                    continue;
                }
                C_Collider cCol = new()
                {
                    Type = col is CircleCollider2D ? C_ColliderType.Circle : C_ColliderType.Polygon,
                    Center = col.bounds.center,
                    AABB = (col.bounds.min, col.bounds.max)
                };

                switch (col)
                {
                    case CircleCollider2D circleCol:
                        cCol.Radius = circleCol.radius * Mathf.Max(circleCol.transform.lossyScale.x, circleCol.transform.lossyScale.y);
                        break;
                    case PolygonCollider2D polyCol:
                    {
                        cCol.VertexCount = polyCol.points.Length;
                        foreach (var t in polyCol.points)
                            colVerts.Add(polyCol.transform.TransformPoint(t));
                        break;
                    }
                    case BoxCollider2D boxCol:
                    {
                        cCol.VertexCount = 4;
                        var extents = boxCol.size * 0.5f;
                        colVerts.Add(boxCol.transform.TransformPoint(boxCol.offset + new Vector2(-extents.x, -extents.y)));
                        colVerts.Add(boxCol.transform.TransformPoint(boxCol.offset + new Vector2(extents.x, -extents.y)));
                        colVerts.Add(boxCol.transform.TransformPoint(boxCol.offset + new Vector2(extents.x, extents.y)));
                        colVerts.Add(boxCol.transform.TransformPoint(boxCol.offset + new Vector2(-extents.x, extents.y)));
                        break;
                    }
                }
                _nColliders[i] = cCol;
            }
            _nColVertices = new NativeArray<Vector2>(colVerts.ToArray(), Allocator.Persistent);
        }

        public void Init()
        {
            var maxPoints = Mathf.CeilToInt(MaxLength / _segmentLength) + 2;
            _nActivePoints = new NativeArray<Point>(maxPoints, Allocator.Persistent);
            _nPointForces = new NativeArray<(Vector2 force, bool isPersistent)>(maxPoints, Allocator.Persistent);
            _nSticks = new NativeArray<Stick>(maxPoints - 1, Allocator.Persistent);
            _jobHandles = new JobHandle[_constrainRuns * 2 + 1];
            _nActivePoints[0] = new Point { Pos = Start.position, OldPos = Start.position, Pinned = true, Mass = _startMass };
            _nActivePoints[1] = new Point { Pos = End.position, OldPos = End.position, Pinned = true, Mass = _endMass };
            _nSticks[0] = new Stick
            {
                P0 = 0,
                P1 = 1,
                Length = _segmentLength,
                MaxOnly = true
            };
            Len = 0; PointCnt = 2; _stickCnt = 1;
        }

        public void Dispose()
        {
            if (_nActivePoints.IsCreated) _nActivePoints.Dispose();
            if (_nPointForces.IsCreated) _nPointForces.Dispose();
            if (_nSticks.IsCreated) _nSticks.Dispose();
            if (_nColliders.IsCreated) _nColliders.Dispose();
            if (_nColVertices.IsCreated) _nColVertices.Dispose();
            _jobHandles = null;
        }
        #endregion

        #region Workers
        private void Pin(PinPoint newPinPoint)
        {
#if UNITY_EDITOR
            Debug.Log("Changing pin to " + newPinPoint);
#endif
            _pinPoint = newPinPoint;
            var sp = _nActivePoints[0];
            var ep = _nActivePoints[PointCnt - 1];
            switch (_pinPoint)
            {
                case PinPoint.Start:
                    sp.Pinned = true;
                    ep.Pinned = false;
                    break;
                case PinPoint.End:
                    ep.Pinned = true;
                    sp.Pinned = false;
                    break;
                case PinPoint.Both:
                    sp.Pinned = true;
                    ep.Pinned = true;
                    break;
            }
            _nActivePoints[0] = sp;
            _nActivePoints[PointCnt - 1] = ep;
        }

        private void Extend(float exLen)
        {
            Len = Mathf.Clamp(Len + exLen, 0, MaxLength);
            if (Len <= _stickCnt * _segmentLength) return;
            _stickCnt--;
            var ep = _nActivePoints[--PointCnt];

            while (Len > (_stickCnt + 1) * _segmentLength)
            {
                var fLen = Len - _stickCnt * _segmentLength;
                var spawnPos = Vector2.Lerp(_nActivePoints[PointCnt - 1].Pos, ep.Pos, _segmentLength / fLen);
                _nActivePoints[PointCnt++] = new Point { Pos = spawnPos, OldPos = spawnPos, Pinned = false, Mass = 1f };
                _nSticks[_stickCnt++] = new Stick
                {
                    P0 = PointCnt - 2,
                    P1 = PointCnt - 1,
                    Length = _segmentLength,
                    MaxOnly = false
                };
            }

            _nActivePoints[PointCnt++] = ep;
            _nSticks[_stickCnt++] = new Stick { P0 = PointCnt - 2, P1 = PointCnt - 1, Length = _segmentLength, MaxOnly = true };
        }

        private void Retract(float retLength)
        {
            Len = Mathf.Clamp(Len - retLength, 0, MaxLength);
            if (Len >= (_stickCnt - 1) * _segmentLength) return;
            _stickCnt--;
            var ep = _nActivePoints[--PointCnt];

            while (Len < (_stickCnt - 1) * _segmentLength)
            {
                PointCnt--;
                _stickCnt--;
            }

            _nActivePoints[PointCnt++] = ep;
            _nSticks[_stickCnt++] = new Stick { P0 = PointCnt - 2, P1 = PointCnt - 1, Length = _segmentLength, MaxOnly = true };
            Assert.IsTrue(_stickCnt >= 1 && PointCnt >= 2, "Rope must have at least one segment and two points.");
        }

        private void AddForces()
        {
            foreach (var forceJob in _forceQueue)
            {
                var (index, f, persist) = forceJob;
                if (index < 0 || index >= PointCnt) 
                    throw new Exception($"Index {index} out of range for applying force (Perhaps outdated).");
                _nPointForces[index] = (f, persist);
            }
            _forceQueue.Clear();
        }
        #endregion
        
        #region Interface
        public void QueueExtend(float length)
        {
            _willAdjLen = true;
            _lenChange = length;
        }

        public void QueueRetract(float length)
        {
            _willAdjLen = true;
            _lenChange = -length;
        }

        public void QueuePin(PinPoint newPinPoint) => _nxtPinPoint = newPinPoint;

        public void QueueForce(Vector2 force, int index, bool persist = false) => _forceQueue.Add((index, force, persist));

        public void Draw(LineRenderer lineRenderer)
        {
            lineRenderer.positionCount = PointCnt;
            for (var i = 0; i < PointCnt; i++)
                lineRenderer.SetPosition(i, _nActivePoints[i].Pos);
        }

        public void StartStep(float deltaTime)
        {
            if (_stepStarted) CompleteStepIfBegan();
            _stepStarted = true;

            PinToTfm();

            var updatePosJob = new UpdatePosJob
            {
                PointCnt = PointCnt,
                ActivePoints = _nActivePoints,
                PointForces = _nPointForces,
                Gravity = _gravity,
                DeltaTime = deltaTime,
                Drag = _drag
            };
            _jobHandles[0] = updatePosJob.Schedule(PointCnt, _pointUpdateBatchSize);

            var maintainDistJob = new MaintainDistJob
            {
                StickCnt = _stickCnt,
                Sticks = _nSticks,
                ActivePoints = _nActivePoints
            };

            var resolveCollisionsJob = new ResolveCollisionsJob
            {
                SegmentLength = _segmentLength,
                BounceFactor = _bounceFactor,
                PointCnt = PointCnt,
                ActivePoints = _nActivePoints,
                Colliders = _nColliders,
                ColVertices = _nColVertices
            };

            var j = 1;
            for (var i = 0; i < _constrainRuns; i++)
            {
                _jobHandles[j] = maintainDistJob.Schedule(_jobHandles[j - 1]);
                j++;
                if (i % _resolveCollisionInterval != 0 || _nColliders.Length <= 0) continue;
                _jobHandles[j] = resolveCollisionsJob.Schedule(PointCnt, _resolveCollisionsBatchSize, _jobHandles[j - 1]);
                j++;
            }
            _lastJobInd = j - 1;
        }

        public void CompleteStepIfBegan()
        {
            if (!_stepStarted) return;

            _jobHandles[_lastJobInd].Complete();
            for (var i = 0; i < PointCnt; i++) if (!_nPointForces[i].isPersistent) _nPointForces[i] = (Vector2.zero, false);
            AddForces();
            if (_willAdjLen)
            {
                _willAdjLen = false;
                if (_lenChange > 0) Extend(_lenChange);
                else Retract(-_lenChange);
            }

            EndVel = _nActivePoints[PointCnt - 1].Pos - _nActivePoints[PointCnt - 1].OldPos;
            if (_nxtPinPoint != _pinPoint) Pin(_nxtPinPoint);

            _stepStarted = false;
        }

        public Vector2 StartPointPos => _nActivePoints[0].Pos;
        public Vector2 EndPointPos => _nActivePoints[PointCnt - 1].Pos;
        public bool StartPinned => _nActivePoints[0].Pinned;
        public bool EndPinned => _nActivePoints[PointCnt - 1].Pinned;
        #endregion
        
        #region Verlet Stages
        private void PinToTfm()
        {
            var sp = _nActivePoints[0];
            var ep = _nActivePoints[PointCnt - 1];
            if (sp.Pinned)
            {
                sp.OldPos = sp.Pos;
                sp.Pos = Start.position;
            }
            if (ep.Pinned)
            {
                ep.OldPos = ep.Pos;
                ep.Pos = End.position;
            }
            _nActivePoints[0] = sp;
            _nActivePoints[PointCnt - 1] = ep;
        }

        [BurstCompile]
        private struct UpdatePosJob : IJobParallelFor
        {
            public int PointCnt;
            public NativeArray<Point> ActivePoints;
            [ReadOnly] public NativeArray<(Vector2 force, bool isPersistent)> PointForces;
            public Vector2 Gravity;
            public float DeltaTime;
            public float Drag;

            public void Execute(int index)
            {
                if (index >= PointCnt) return;
                var p = ActivePoints[index];
                if (p.Pinned) return;
                var newPos = p.Pos + (p.Pos - p.OldPos) * (1f - Drag) + (1f - Drag) * DeltaTime * DeltaTime * (Gravity + PointForces[index].force);
                p.OldPos = p.Pos;
                p.Pos = newPos;
                ActivePoints[index] = p;
            }
        }

        [BurstCompile]
        private struct MaintainDistJob : IJob
        {
            public int StickCnt;
            [ReadOnly] public NativeArray<Stick> Sticks;
            public NativeArray<Point> ActivePoints;

            public void Execute()
            {
                for (var i = 0; i < StickCnt; i++)
                {
                    var stick = Sticks[i];
                    Point p0 = ActivePoints[stick.P0], p1 = ActivePoints[stick.P1];
                    var delta = p0.Pos - p1.Pos;
                    var dist = Mathf.Max(delta.magnitude, Const.EPS);

                    if ((stick.MaxOnly && dist < stick.Length) || (p0.Pinned && p1.Pinned)) return;
                    var diff = (stick.Length - dist) / dist;
                    var totMass = p0.Mass + p1.Mass;
                    float massRatio0 = p1.Mass / totMass, massRatio1 = p0.Mass / totMass;
                    var offset = diff * delta;
                    
                    if (!p0.Pinned && !p1.Pinned)
                    {
                        p0.Pos += offset * massRatio0;
                        p1.Pos -= offset * massRatio1;
                    }
                    else if (!p0.Pinned) p0.Pos += offset;
                    else p1.Pos -= offset;

                    ActivePoints[stick.P0] = p0;
                    ActivePoints[stick.P1] = p1;
                }
            }
        }

        [BurstCompile]
        private struct ResolveCollisionsJob : IJobParallelFor
        {
            public float SegmentLength;
            public float BounceFactor;
            public int PointCnt;
            public NativeArray<Point> ActivePoints;
            [ReadOnly] public NativeArray<C_Collider> Colliders;
            [ReadOnly] public NativeArray<Vector2> ColVertices;

            public void Execute(int index)
            {
                if (index >= PointCnt) return;
                var p = ActivePoints[index];
                if (p.Pinned) return;

                var v = p.Pos - p.OldPos;
                var colRad = SegmentLength * 0.5f;
                var vi = 0;
                foreach (var col in Colliders)
                {
                    var rectMin = col.AABB.BtmLeft;
                    var rectMax = col.AABB.TopRight;
                    rectMin -= Vector2.one * colRad;
                    rectMax += Vector2.one * colRad;
                    if (!CollisionDetection.PointInAABB(p.Pos, rectMin, rectMax))
                    {
                        if (col.Type == C_ColliderType.Polygon) vi += col.VertexCount;
                        continue;
                    }

                    float depth = 0; 
                    var dn = Vector2.zero;
                    if (col.Type == C_ColliderType.Polygon)
                    {
                        var hasCol = CollisionDetection.SatCheck(p.Pos, colRad,
                            new NativePoly { VertexStart = vi, VertexCount = col.VertexCount, Vertices = ColVertices }, out dn, out depth);
                        vi += col.VertexCount;
                        if (!hasCol) continue;
                    }
                    else if (col.Type == C_ColliderType.Circle && !CollisionDetection.CircleCollision(p.Pos, colRad, col.Center, col.Radius, out dn, out depth)) continue;
                    p.Pos += dn * depth;
                    v = Vector2.Reflect(v, dn) * BounceFactor;
                }
                p.OldPos = p.Pos - v;
                ActivePoints[index] = p;
            }
        }
        #endregion
    }
}
