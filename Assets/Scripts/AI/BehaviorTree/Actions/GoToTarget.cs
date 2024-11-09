using UnityEngine;
using System.Collections.Generic;

namespace BehaviorTree.Actions
{
    public class GotoTargetByFixedSpd : CfAction
    {
        private Transform _transform;
        private float _speed;
        string _targetName;

        public GotoTargetByFixedSpd(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            List<object> dataRef = Tree.GetData(_params);
            _speed = (float)dataRef[0];
            _transform = (Transform)dataRef[1];
            _targetName = (string)dataRef[2];
        }

        // Indefinite pursuit
        public override void Update()
        {
            Transform target = Tree.GetDatum<Transform>(_targetName, true);

            if (Vector2.Distance(_transform.position, target.position) > 0.01f)
                _transform.position = Vector3.MoveTowards(
                    _transform.position, target.position, _speed * Time.deltaTime);
            else State = State.SUCCESS;
        }
    }

    public class GotoTargetByCurve : CfAction
    {
        protected Transform _transform;
        protected string _targetName;
        protected AnimationCurve _mvmtCurve;
        protected float _duration = -1f, _speed = -1f;
        protected TargetType _type;
        protected MoveBy _moveBy;

        protected float _timer = 0;
        protected Vector3 _startPos, _destPos;

        public enum TargetType { Transform, Vector3 }
        public enum MoveBy { Speed, Duration }

        public GotoTargetByCurve(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            _transform = Tree.GetDatum<Transform>(_params[0]);
            _targetName = _params[1];
            _mvmtCurve = Tree.GetDatum<AnimationCurve>(_params[2]);
            _type = Tree.GetDatum<TargetType>(_params[3]);
            _moveBy = Tree.GetDatum<MoveBy>(_params[4]);
            if (_moveBy == MoveBy.Speed)
                _speed = Tree.GetDatum<float>(_params[5]);
            else _duration = Tree.GetDatum<float>(_params[5]);
        }

        public override void Update()
        {
            if (_timer < _duration)
                _transform.position = Vector3.Lerp(_startPos, _destPos, _mvmtCurve.Evaluate(_timer / _duration));
            else State = State.SUCCESS;
            _timer += Time.deltaTime;
        }

        protected override void OnInit()
        {
            base.OnInit();
            _startPos = _transform.position;
            _destPos = _type == TargetType.Transform ? Tree.GetDatum<Transform>(_targetName).position : Tree.GetDatum<Vector3>(_targetName);
            _timer = 0;
            if (_moveBy == MoveBy.Speed)
                _duration = Vector2.Distance(_startPos, _destPos) / _speed;
        }
    }
}