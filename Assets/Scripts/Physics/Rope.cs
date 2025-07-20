using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// References:
// https://www.youtube.com/watch?v=IfeUeMYSl3E
// https://pikuma.com/blog/verlet-integration-2d-cloth-physics-simulation
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

        // Physics params
        public int ConstrainRuns = 10;
        public Vector2 gravity = new(0, -2f);
        public ClampedFloatParameter drag = new(0.1f, 0, 1f);
        public float bounceFactor = 0.2f;

        readonly List<Point> _activePoints = new();
        readonly List<Stick> _sticks = new();
        readonly List<(Vector2, bool)> _pointForces = new(); // true if persistent false if one time only

        public float Len { get; private set; } = 0f;
        public int NumSegs { get; private set; } = 0;

        public int NumPoints => _activePoints.Count;

        public Vector2 PointPos(int index)
        {
            if (index < 0 || index >= _activePoints.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range of active points.");
            return _activePoints[index].Pos;
        }

        public Vector2 PointForce(int index)
        {
            if (index < 0 || index >= _pointForces.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range of point forces.");
            return _pointForces[index].Item1;
        }

        public void ApplyForce(Vector2 force, int index, bool persist = false)
        {
            if (index < 0 || index >= _activePoints.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range of active points.");
            _pointForces[index] = (force, persist);
        }

        public Vector2 PointVel(int index, float deltaTime)
        {
            if (index < 0 || index >= _activePoints.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range of active points.");
            Point p = _activePoints[index];
            return (p.Pos - p.OldPos) / deltaTime;
        }

        public void Extend(float exlength)
        {
            Len = Mathf.Clamp(Len + exlength, 0, MaxLength);
            if (Len > (NumSegs + 1) * SegmentLength)
            {
                _sticks.RemoveAt(_sticks.Count - 1);
                _activePoints.RemoveAt(_activePoints.Count - 1);
                Point ep = _activePoints[^1];

                while (Len > (NumSegs + 1) * SegmentLength)
                {
                    float flength = Len - NumSegs * SegmentLength;
                    var spawnPos = Vector2.Lerp(_activePoints[^1].Pos, (Vector2)End.position, SegmentLength / flength);
                    _activePoints.Add(new Point { Pos = spawnPos, OldPos = spawnPos, Pinned = false });
                    _sticks.Add(new Stick
                    {
                        P0 = _activePoints.Count - 2,
                        P1 = _activePoints.Count - 1,
                        Length = SegmentLength,
                        MaxOnly = false
                    });
                    NumSegs++;
                }

                _activePoints.Add(ep);
                _sticks.Add(new Stick { P0 = _activePoints.Count - 2, P1 = _activePoints.Count - 1, Length = SegmentLength, MaxOnly = true });
            }
        }

        public void Retract(float retLength)
        {
            Len = Mathf.Clamp(Len - retLength, 0, MaxLength);
            if (Len < (NumSegs + 1) * SegmentLength)
            {
                _sticks.RemoveAt(_sticks.Count - 1);
                _activePoints.RemoveAt(_activePoints.Count - 1);
                Point ep = _activePoints[^1];

                while (Len < (NumSegs + 1) * SegmentLength)
                {
                    _activePoints.RemoveAt(_activePoints.Count - 1);
                    _sticks.RemoveAt(_sticks.Count - 1);
                    NumSegs--;
                }

                _activePoints.Add(ep);
                _sticks.Add(new Stick { P0 = _activePoints.Count - 2, P1 = _activePoints.Count - 1, Length = SegmentLength, MaxOnly = true });
            }
        }

        public void Pin(PinPoint pinPoint)
        {
            var sp = _activePoints[0];
            var ep = _activePoints[^1];
            switch (pinPoint)
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
            _activePoints[0] = sp;
            _activePoints[^1] = ep;
        }

        public void Init()
        {
            _activePoints.Clear();
            _sticks.Clear();
            NumSegs = 0; Len = 0;
            _activePoints.Add(new Point { Pos = Start.position, OldPos = Start.position, Pinned = true });
            _activePoints.Add(new Point { Pos = End.position, OldPos = End.position, Pinned = true });
            _sticks.Add(new Stick { P0 = 0, P1 = 1, Length = SegmentLength, MaxOnly = true });
        }

        public void Draw(LineRenderer lineRenderer)
        {
            lineRenderer.positionCount = _activePoints.Count;
            for (int i = 0; i < _activePoints.Count; i++)
                lineRenderer.SetPosition(i, _activePoints[i].Pos);
        }

        // TODO: Move resolve constraints to job system
        public void Tick(float deltaTime)
        {
            var sp = _activePoints[0];
            var ep = _activePoints[^1];
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
            _activePoints[0] = sp;
            _activePoints[^1] = ep;

            UpdatePos(deltaTime);
            for (int _ = 0; _ < ConstrainRuns; _++)
            {
                MaintainDist();
                ResolveCollisions();
            }

            MoveTfmAlong();
        }

        void MoveTfmAlong()
        {
            if (!_activePoints[0].Pinned) Start.position = _activePoints[0].Pos;
            if (!_activePoints[^1].Pinned) End.position = _activePoints[^1].Pos;
        }

        void UpdatePos(float deltaTime)
        {
            for (int i = 0; i < _activePoints.Count; i++)
            {
                if (_activePoints[i].Pinned) continue;
                Point p = _activePoints[i];
                Vector2 newPos = p.Pos + (p.Pos - p.OldPos) * (1f - drag.value) + (1f - drag.value) * deltaTime * deltaTime * (gravity + _pointForces[i].Item1);
                if (!_pointForces[i].Item2) _pointForces[i] = (Vector2.zero, false);
                p.OldPos = p.Pos;
                p.Pos = newPos;
                _activePoints[i] = p;
            }
        }

        void MaintainDist()
        {
            for (int i = 0; i < _sticks.Count; i++)
            {
                Stick stick = _sticks[i];
                Point p0 = _activePoints[stick.P0], p1 = _activePoints[stick.P1];
                Vector2 delta = p1.Pos - p0.Pos;
                float dist = delta.magnitude;
                if (dist == 0) continue;

                if (stick.MaxOnly && dist < stick.Length) continue;
                float diff = (stick.Length - dist) / dist;
                Vector2 offset = (p0.Pinned && p1.Pinned ? 0.5f : 1f) * diff * delta;

                if (!p0.Pinned) p0.Pos += offset;
                if (!p1.Pinned) p1.Pos -= offset;
                _activePoints[stick.P0] = p0;
                _activePoints[stick.P1] = p1;
            }

        }

        void ResolveCollisions()
        {
            for (int i = 0; i < _activePoints.Count; i++)
            {
                if (_activePoints[i].Pinned) continue;
                Point p = _activePoints[i];
                Vector2 v = p.Pos - p.OldPos;
                float colRad = SegmentLength * 0.5f;
                Collider2D[] cols = Physics2D.OverlapCircleAll(p.Pos, colRad, Abyss.Settings.LayerMask.OBSTACLE_LMASK);
                foreach (var col in cols)
                {
                    Vector2 cp = col.ClosestPoint(p.Pos);
                    Vector2 delta = cp - p.Pos;
                    float dist = delta.magnitude;

                    if (dist < colRad)
                    {
                        Vector2 dn = delta.normalized;
                        if (dn == Vector2.zero) dn = (p.Pos - (Vector2)col.transform.position).normalized;
                        p.Pos += dn * (colRad - dist);
                        _activePoints[i] = p;
                        v = Vector2.Reflect(v, dn) * bounceFactor;
                    }
                }
                p.OldPos = p.Pos - v;
                _activePoints[i] = p;
            }
        }
    }
}
