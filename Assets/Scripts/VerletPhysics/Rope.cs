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
namespace VerletPhysics
{
    [Serializable]
    public class Rope
    {
        public enum PinPoint { Start, End, Both }
        public float SegmentLength = 0.2f;
        public float MaxLength = 20f;
        public Transform Start, End;
        public float Width => SegmentLength * 0.5f;
        public float MoveEndsSmoothTime = 0.3f;

        // Physics params
        [Range(1, 100)] public int ConstrainRuns = 10;
        [Range(1, 8)] public int ResolveCollisionInterval = 4;
        public Vector2 gravity = new(0, -2f);
        [Range(0, 1)] public float drag = 0.1f;
        public float bounceFactor = 0.2f;

        public int PointUpdateBatchSize = 64;
        public int ResolveCollisionsBatchSize = 8;

        NativeArray<Point> n_activePoints;
        NativeArray<(Vector2, bool)> n_pointForces;
        NativeArray<Stick> n_sticks;
        NativeArray<C_Collider> n_colliders;
        NativeArray<Vector2> n_col_vertices;
        JobHandle[] _jobHandles;
        int _lastJobInd = 0;
        public Vector2 EndVel { get; private set; } = Vector2.zero;
        Vector3 _startVel, _endVel;

        bool _willAdjLen = false;
        float _lenChange = 0f;
        List<(int, Vector2, bool)> _forceQueue = new();
        int _maxNumPoints = 0;
        PinPoint _pinPoint = PinPoint.Both, _nxtPinPoint = PinPoint.Both;

        public float Len { get; private set; } = 0f;
        public int NumSegs { get; private set; } = 0;
        public int NumPoints { get; private set; } = 0;
        public int NumSticks { get; private set; } = 0;
        public int NumColliders { get; private set; } = 0;

        #region Setup Teardown
        public void SetColliders(List<Collider2D> cols)
        {
            n_colliders = new NativeArray<C_Collider>(cols.Count, Allocator.Persistent);
            List<Vector2> colVerts = new();
            for (int i = 0; i < cols.Count; i++)
            {
                var col = cols[i];
                Debug.Log($"Collider {i}: {col.name}");
                if (!(col is PolygonCollider2D || col is CircleCollider2D || col is BoxCollider2D))
                {
                    Debug.LogWarning($"Unsupported collider type: {col.GetType()}");
                    continue;
                }
                C_Collider ccol = new()
                {
                    Type = col is CircleCollider2D ? C_ColliderType.Circle : C_ColliderType.Polygon,
                    Center = col.bounds.center,
                    AABB = (col.bounds.min, col.bounds.max)
                };

                if (col is CircleCollider2D circleCol) ccol.Radius = circleCol.radius * Mathf.Max(circleCol.transform.lossyScale.x, circleCol.transform.lossyScale.y);
                else if (col is PolygonCollider2D polyCol)
                {
                    ccol.VertexCount = polyCol.points.Length;
                    for (int j = 0; j < polyCol.points.Length; j++)
                        colVerts.Add(polyCol.transform.TransformPoint(polyCol.points[j]));
                }
                else
                {
                    var boxCol = col as BoxCollider2D;
                    ccol.VertexCount = 4;
                    Vector2 extents = boxCol.size * 0.5f;
                    colVerts.Add(boxCol.transform.TransformPoint(boxCol.offset + new Vector2(-extents.x, -extents.y)));
                    colVerts.Add(boxCol.transform.TransformPoint(boxCol.offset + new Vector2(extents.x, -extents.y)));
                    colVerts.Add(boxCol.transform.TransformPoint(boxCol.offset + new Vector2(extents.x, extents.y)));
                    colVerts.Add(boxCol.transform.TransformPoint(boxCol.offset + new Vector2(-extents.x, extents.y)));
                }
                n_colliders[i] = ccol;
            }
            n_col_vertices = new NativeArray<Vector2>(colVerts.ToArray(), Allocator.Persistent);
            // foreach (var n_col in n_colliders) Debug.Log($"Collider: {n_col.Type}, Center: {n_col.Center}, Radius: {n_col.Radius}, AABB: {n_col.AABB.Item1}, {n_col.AABB.Item2}");
            // foreach (var v in n_col_vertices) Debug.Log($"Collider Vertex: {v}");
        }

        public void Init()
        {
            int mpc = Mathf.CeilToInt(MaxLength / SegmentLength) + 2;
            if (!n_activePoints.IsCreated || mpc != _maxNumPoints) n_activePoints = new NativeArray<Point>(mpc, Allocator.Persistent);
            if (!n_pointForces.IsCreated || mpc != _maxNumPoints) n_pointForces = new NativeArray<(Vector2, bool)>(mpc, Allocator.Persistent);
            if (!n_sticks.IsCreated || mpc != _maxNumPoints) n_sticks = new NativeArray<Stick>(mpc - 1, Allocator.Persistent);
            _maxNumPoints = mpc;
            if (_jobHandles == null || _jobHandles.Length < ConstrainRuns * 2 + 1) _jobHandles = new JobHandle[ConstrainRuns * 2 + 1];
            n_activePoints[0] = new Point { Pos = Start.position, OldPos = Start.position, Pinned = true };
            n_activePoints[1] = new Point { Pos = End.position, OldPos = End.position, Pinned = true };
            n_sticks[0] = new Stick
            {
                P0 = 0,
                P1 = 1,
                Length = SegmentLength,
                MaxOnly = true
            };
            NumSegs = 1; Len = 0; NumPoints = 2; NumSticks = 1;
        }

        public void Dispose()
        {
            if (n_activePoints.IsCreated) n_activePoints.Dispose();
            if (n_pointForces.IsCreated) n_pointForces.Dispose();
            if (n_sticks.IsCreated) n_sticks.Dispose();
            if (n_colliders.IsCreated) n_colliders.Dispose();
            if (n_col_vertices.IsCreated) n_col_vertices.Dispose();
            _jobHandles = null;
        }
        #endregion

        #region Workers
        void Pin(PinPoint nPinPoint)
        {
            Debug.Log("Changing pin to " + nPinPoint);
            _pinPoint = nPinPoint;
            var sp = n_activePoints[0];
            var ep = n_activePoints[NumPoints - 1];
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
            n_activePoints[0] = sp;
            n_activePoints[NumPoints - 1] = ep;
            Debug.Log($"sp {sp.Pos} {sp.Pinned}, ep {ep.Pos} {ep.Pinned}");
        }

        void Extend(float exlength)
        {
            Len = Mathf.Clamp(Len + exlength, 0, MaxLength);
            if (Len > NumSegs * SegmentLength)
            {
                NumSticks--;
                Point ep = n_activePoints[--NumPoints];

                while (Len > NumSegs * SegmentLength)
                {
                    float flength = Len - NumSegs * SegmentLength;
                    var spawnPos = Vector2.Lerp(n_activePoints[NumPoints - 1].Pos, ep.Pos, SegmentLength / flength);
                    n_activePoints[NumPoints++] = new Point { Pos = spawnPos, OldPos = spawnPos, Pinned = false };
                    // Debug.Log($"Extending rope: {spawnPos}, NumSegs: {NumSegs}, Len: {Len}, SegmentLength: {SegmentLength}");
                    n_sticks[NumSticks++] = new Stick
                    {
                        P0 = NumPoints - 2,
                        P1 = NumPoints - 1,
                        Length = SegmentLength,
                        MaxOnly = false
                    };
                    NumSegs++;
                }

                n_activePoints[NumPoints++] = ep;
                n_sticks[NumSticks++] = new Stick { P0 = NumPoints - 2, P1 = NumPoints - 1, Length = SegmentLength, MaxOnly = true };
            }
        }

        void Retract(float retLength)
        {
            Len = Mathf.Clamp(Len - retLength, 0, MaxLength);
            if (Len < (NumSegs - 1) * SegmentLength)
            {
                NumSticks--;
                Point ep = n_activePoints[--NumPoints];

                while (Len < (NumSegs - 1) * SegmentLength)
                {
                    NumPoints--;
                    NumSticks--;
                    NumSegs--;
                }

                n_activePoints[NumPoints++] = ep;
                n_sticks[NumSticks++] = new Stick { P0 = NumPoints - 2, P1 = NumPoints - 1, Length = SegmentLength, MaxOnly = true };
                Assert.IsTrue(NumSticks >= 1 && NumSegs >= 1 && NumPoints >= 2, "Rope must have at least one segment and two points.");
            }
        }

        void AddForces()
        {
            foreach (var forceJob in _forceQueue)
            {
                var (index, f, persist) = forceJob;
                if (index < 0 || index >= NumPoints) Debug.LogWarning($"Index {index} out of range for applying force (Perhaps outdated).");
                else n_pointForces[index] = (f, persist);
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

        public void QueuePin(PinPoint nPinPoint)
        {
            if (_pinPoint == nPinPoint) return;
            // Debug.Log($"Pinning rope: {nPinPoint}");
            _nxtPinPoint = nPinPoint;
        }

        public void QueueForce(Vector2 force, int index, bool persist = false) => _forceQueue.Add((index, force, persist));

        public void Draw(LineRenderer lineRenderer)
        {
            lineRenderer.positionCount = NumPoints;
            for (int i = 0; i < NumPoints; i++)
                lineRenderer.SetPosition(i, n_activePoints[i].Pos);
        }

        public void StartTick(float deltaTime)
        {
            PinToTfm();

            var updatePosJob = new UpdatePosJob
            {
                ActivePoints = n_activePoints,
                PointForces = n_pointForces,
                Gravity = gravity,
                DeltaTime = deltaTime,
                Drag = drag
            };
            _jobHandles[0] = updatePosJob.Schedule(NumPoints, PointUpdateBatchSize);

            var maintainDistJob = new MaintainDistJob
            {
                Sticks = n_sticks,
                ActivePoints = n_activePoints
            };

            var resolveCollisionsJob = new ResolveCollisionsJob
            {
                SegmentLength = SegmentLength,
                BounceFactor = bounceFactor,
                ActivePoints = n_activePoints,
                Colliders = n_colliders,
                ColVertices = n_col_vertices
            };

            int j = 1;
            for (int i = 0; i < ConstrainRuns; i++)
            {
                _jobHandles[j] = maintainDistJob.Schedule(_jobHandles[j - 1]);
                j++;
                if (i % ResolveCollisionInterval == 0 && n_colliders != null && n_colliders.Length > 0)
                {
                    _jobHandles[j] = resolveCollisionsJob.Schedule(NumPoints, ResolveCollisionsBatchSize, _jobHandles[j - 1]);
                    j++;
                }
            }
            _lastJobInd = j - 1;
        }

        public void CompleteTick(LineRenderer ropeRenderer)
        {
            _jobHandles[_lastJobInd].Complete();
            for (int i = 0; i < NumPoints; i++) if (!n_pointForces[i].Item2) n_pointForces[i] = (Vector2.zero, false);
            AddForces();
            if (_willAdjLen)
            {
                _willAdjLen = false;
                if (_lenChange > 0) Extend(_lenChange);
                else Retract(-_lenChange);
            }

            // for (int i = 0; i < NumPoints; i++) Debug.Log($"Point {i}: Pos: {n_activePoints[i].Pos}, OldPos: {n_activePoints[i].OldPos}, Pinned: {n_activePoints[i].Pinned}");
            EndVel = n_activePoints[NumPoints - 1].Pos - n_activePoints[NumPoints - 1].OldPos;
            if (_nxtPinPoint != _pinPoint) Pin(_nxtPinPoint);
            MoveTfmAlong(); //FIXME: needs something smoother
            Draw(ropeRenderer);
        }
        #endregion
        #region Verlet Stages
        void PinToTfm()
        {
            var sp = n_activePoints[0];
            var ep = n_activePoints[NumPoints - 1];
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
            n_activePoints[0] = sp;
            n_activePoints[NumPoints - 1] = ep;
        }

        void MoveTfmAlong()
        {
            if (!n_activePoints[0].Pinned) Start.position = n_activePoints[0].Pos;
            if (!n_activePoints[NumPoints - 1].Pinned) End.position = n_activePoints[NumPoints - 1].Pos;
            // if (!n_activePoints[0].Pinned) Start.position = Vector3.SmoothDamp(Start.position, n_activePoints[0].Pos, ref _startVel, MoveEndsSmoothTime);
            // else _startVel = Vector3.zero;
            // if (!n_activePoints[NumPoints - 1].Pinned) End.position = Vector3.SmoothDamp(End.position, n_activePoints[NumPoints - 1].Pos, ref _endVel, MoveEndsSmoothTime);
            // else _endVel = Vector3.zero;
        }

        [BurstCompile]
        struct UpdatePosJob : IJobParallelFor
        {
            public NativeArray<Point> ActivePoints;
            [ReadOnly] public NativeArray<(Vector2, bool)> PointForces;
            public Vector2 Gravity;
            public float DeltaTime;
            public float Drag;

            public void Execute(int index)
            {
                Point p = ActivePoints[index];
                if (p.Pinned) return;
                Vector2 newPos = p.Pos + (p.Pos - p.OldPos) * (1f - Drag) + (1f - Drag) * DeltaTime * DeltaTime * (Gravity + PointForces[index].Item1);
                p.OldPos = p.Pos;
                p.Pos = newPos;
                ActivePoints[index] = p;
            }
        }

        [BurstCompile]
        struct MaintainDistJob : IJob
        {
            [ReadOnly] public NativeArray<Stick> Sticks;
            public NativeArray<Point> ActivePoints;

            public void Execute()
            {
                for (int i = 0; i < Sticks.Length; i++)
                {
                    Stick stick = Sticks[i];
                    Point p0 = ActivePoints[stick.P0], p1 = ActivePoints[stick.P1];
                    Vector2 delta = p0.Pos - p1.Pos;
                    float dist = Mathf.Max(delta.magnitude, Const.EPS);

                    if ((stick.MaxOnly && dist < stick.Length) || (p0.Pinned && p1.Pinned)) return;
                    float diff = (stick.Length - dist) / dist;
                    Vector2 offset = (!p0.Pinned && !p1.Pinned ? 0.5f : 1f) * diff * delta;

                    if (!p0.Pinned) p0.Pos += offset;
                    if (!p1.Pinned) p1.Pos -= offset;
                    ActivePoints[stick.P0] = p0;
                    ActivePoints[stick.P1] = p1;
                }
            }
        }

        [BurstCompile]
        struct ResolveCollisionsJob : IJobParallelFor
        {
            public float SegmentLength;
            public float BounceFactor;
            public NativeArray<Point> ActivePoints;
            [ReadOnly] public NativeArray<C_Collider> Colliders;
            [ReadOnly] public NativeArray<Vector2> ColVertices;

            public void Execute(int index)
            {
                Point p = ActivePoints[index];
                if (p.Pinned) return;

                Vector2 v = p.Pos - p.OldPos;
                float colRad = SegmentLength * 0.5f;
                int vi = 0;
                foreach (var col in Colliders)
                {
                    Vector2 rectMin = col.AABB.Item1;
                    Vector2 rectMax = col.AABB.Item2;
                    rectMin -= Vector2.one * colRad;
                    rectMax += Vector2.one * colRad;
                    if (!CollisionDetection.PointInAABB(p.Pos, rectMin, rectMax))
                    {
                        if (col.Type == C_ColliderType.Polygon) vi += col.VertexCount;
                        continue;
                    }

                    float depth = 0; Vector2 dn = Vector2.zero;
                    if (col.Type == C_ColliderType.Polygon)
                    {
                        var hasCol = CollisionDetection.SATCheck(p.Pos, colRad, new NativePoly() { VertexStart = vi, VertexCount = col.VertexCount, Vertices = ColVertices }, out dn, out depth);
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
