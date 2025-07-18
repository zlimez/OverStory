using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// References:
// https://www.youtube.com/watch?v=IfeUeMYSl3E
// https://pikuma.com/blog/verlet-integration-2d-cloth-physics-simulation
namespace VerletPhysics
{
    public class Rope : MonoBehaviour
    {
        public enum PinPoint { Start, End, Both }
        [Header("Rope")]
        [SerializeField] float segmentLength = 0.2f;
        [SerializeField] float maxLength = 20f;
        [SerializeField] Transform ropeStart, ropeEnd;

        [Header("Physics")]
        [SerializeField] int constrainRuns = 10;
        [SerializeField] Vector2 gravity = new(0, -2f);
        [SerializeField] ClampedFloatParameter drag = new(0.1f, 0, 1f);
        [SerializeField] float bounceFactor = 0.2f;

        readonly List<Point> _activePoints = new();
        readonly List<Stick> _sticks = new();
        LineRenderer _lineRenderer;
        float _length = 0;
        int _numSegs = 0;

        void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _activePoints.Add(new Point { Pos = ropeStart.position, OldPos = ropeStart.position, Pinned = true });
            _activePoints.Add(new Point { Pos = ropeEnd.position, OldPos = ropeEnd.position, Pinned = true });
            _sticks.Add(new Stick { P0 = 0, P1 = 1, Length = segmentLength, MaxOnly = true });
        }

        public void Extend(float exlength)
        {
            _length = Mathf.Clamp(_length + exlength, 0, maxLength);
            if (_length > (_numSegs + 1) * segmentLength)
            {
                _sticks.RemoveAt(_sticks.Count - 1);
                _activePoints.RemoveAt(_activePoints.Count - 1);
                Point ep = _activePoints[^1];

                while (_length > (_numSegs + 1) * segmentLength)
                {
                    float flength = _length - _numSegs * segmentLength;
                    var spawnPos = Vector2.Lerp(_activePoints[^1].Pos, (Vector2)ropeEnd.position, segmentLength / flength);
                    _activePoints.Add(new Point { Pos = spawnPos, OldPos = spawnPos, Pinned = false });
                    _sticks.Add(new Stick
                    {
                        P0 = _activePoints.Count - 2,
                        P1 = _activePoints.Count - 1,
                        Length = segmentLength,
                        MaxOnly = false
                    });
                    _numSegs++;
                }

                _activePoints.Add(ep);
                _sticks.Add(new Stick { P0 = _activePoints.Count - 2, P1 = _activePoints.Count - 1, Length = segmentLength, MaxOnly = true });
            }
        }

        public void Retract(float retLength)
        {
            _length = Mathf.Clamp(_length - retLength, 0, maxLength);
            if (_length < (_numSegs + 1) * segmentLength)
            {
                _sticks.RemoveAt(_sticks.Count - 1);
                _activePoints.RemoveAt(_activePoints.Count - 1);
                Point ep = _activePoints[^1];

                while (_length < (_numSegs + 1) * segmentLength)
                {
                    _activePoints.RemoveAt(_activePoints.Count - 1);
                    _sticks.RemoveAt(_sticks.Count - 1);
                    _numSegs--;
                }

                _activePoints.Add(ep);
                _sticks.Add(new Stick { P0 = _activePoints.Count - 2, P1 = _activePoints.Count - 1, Length = segmentLength, MaxOnly = true });
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

        // TODO: Move resolve constraints to job system
        void Tick(float deltaTime)
        {
            UpdatePos(deltaTime);
            for (int _ = 0; _ < constrainRuns; _++)
            {
                MaintainDist();
                ResolveCollisions();
            }
            Draw();
        }

        void UpdatePos(float deltaTime)
        {
            for (int i = 0; i < _activePoints.Count; i++)
            {
                if (_activePoints[i].Pinned) continue;
                Point p = _activePoints[i];
                Vector2 newPos = p.Pos + (p.Pos - p.OldPos) * (1f - drag.value) + (1f - drag.value) * deltaTime * deltaTime * gravity;
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
                float colRad = segmentLength * 0.5f;
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

        void Draw()
        {
            _lineRenderer.positionCount = _activePoints.Count;
            for (int i = 0; i < _activePoints.Count; i++)
                _lineRenderer.SetPosition(i, _activePoints[i].Pos);
        }
    }
}
