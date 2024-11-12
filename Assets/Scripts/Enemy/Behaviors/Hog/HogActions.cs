using Abyss;
using UnityEngine;

namespace BehaviorTree.Actions
{
    public class CheckHogLure : CfAction
    {
        static readonly int _obstacleLayerMask = 1 << (int)AbyssSettings.Layers.Ground | 1 << (int)AbyssSettings.Layers.Obstacle;
        SpriteManager _spriteManager;
        Transform _transform;
        string _destVarName;

        public CheckHogLure(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            _spriteManager = Tree.GetDatum<SpriteManager>(_params[0]);
            _transform = Tree.GetDatum<Transform>(_params[1]);
            _destVarName = _params[2];
        }

        public override void Update()
        {
            var lureInfo = Tree.GetDatum<(float, Vector3)>(_destVarName);

            if (Vector2.SqrMagnitude(lureInfo.Item2 - _transform.position) > lureInfo.Item1 * lureInfo.Item1
                    || Physics2D.Raycast(_transform.position, lureInfo.Item2 - _transform.position, Vector2.Distance(lureInfo.Item2, _transform.position), _obstacleLayerMask))
            {
                State = State.FAILURE;
                return;
            }

            _spriteManager.Face(lureInfo.Item2);
            State = State.SUCCESS;
        }
    }

    public class TravelTo : CfAction
    {
        string _destVarName;
        Arena _arena;
        Transform _patrolLeft, _patrolRight, _transform;
        AnimationCurve _travelCurve;
        float _speed;

        float _duration, _timer = 0;
        Vector3 _startPos, _destPos;
        Vector3 _plStart, _plEnd, _prStart, _prEnd;
        Vector3 _arenaStart, _arenaEnd;
        Arena.Anchor _anchorUsed;

        public TravelTo(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            _destVarName = _params[0];
            _arena = Tree.GetDatum<Arena>(_params[1]);
            _patrolLeft = Tree.GetDatum<Transform>(_params[2]);
            _patrolRight = Tree.GetDatum<Transform>(_params[3]);
            _transform = Tree.GetDatum<Transform>(_params[4]);
            _travelCurve = Tree.GetDatum<AnimationCurve>(_params[5]);
            _speed = Tree.GetDatum<float>(_params[6]);
        }

        public override void Update()
        {
            if (_timer < _duration)
            {
                _transform.position = Vector3.Lerp(_startPos, _destPos, _travelCurve.Evaluate(_timer / _duration));
                _arena.MoveTo(Vector3.Lerp(_arenaStart, _arenaEnd, _travelCurve.Evaluate(_timer / _duration)), _anchorUsed);
                _patrolLeft.position = Vector3.Lerp(_plStart, _plEnd, _travelCurve.Evaluate(_timer / _duration));
                _patrolRight.position = Vector3.Lerp(_prStart, _prEnd, _travelCurve.Evaluate(_timer / _duration));
            }
            else State = State.SUCCESS;
            _timer += Time.deltaTime;
        }

        // Move arena, patrolWps, transform to destination using arena as base line
        protected override void OnInit()
        {
            base.OnInit();

            var lureInfo = Tree.GetDatum<(float, Vector3)>(_destVarName);
            _arenaEnd = lureInfo.Item2;

            _timer = 0;
            _startPos = _transform.position;
            _plStart = _patrolLeft.position;
            _prStart = _patrolRight.position;

            if ((_arenaEnd - _transform.position).x < 0)
            {
                _anchorUsed = Arena.Anchor.BottomLeft;
                _plEnd = _arenaEnd;
                _prEnd = _plEnd + (_prStart - _plStart);
            }
            else
            {
                _anchorUsed = Arena.Anchor.BottomRight;
                _prEnd = _arenaEnd;
                _plEnd = _prEnd + (_plStart - _prStart);
            }

            _arenaStart = _arena.GetAnchorPos(_anchorUsed);
            _destPos = new(Random.Range(_plEnd.x, _prEnd.x), _prEnd.y, 0);

            _duration = Vector2.Distance(_startPos, _destPos) / _speed;
        }
    }
}
