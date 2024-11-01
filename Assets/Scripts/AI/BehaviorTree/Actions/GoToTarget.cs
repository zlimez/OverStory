using UnityEngine;
using System.Collections.Generic;

namespace BehaviorTree.Actions
{
    public class GotoTfmBySpd : CfAction
    {
        private Transform _transform;
        private float _speed;
        string _targetName;

        public GotoTfmBySpd(string[] parameters) : base(parameters) { }

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
        private Transform _transform;
        string _targetName;
        AnimationCurve _mvmtCurve;
        float _duration = -1f, _speed = -1f;
        TargetType _type;
        MoveBy _moveBy;

        float _timer = 0;
        Vector3 _startPos, _destPos;

        public enum TargetType { Transform, Vector3 }
        public enum MoveBy { Speed, Duration }

        public GotoTargetByCurve(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            List<object> dataRef = Tree.GetData(_params);
            _transform = (Transform)dataRef[0];
            _targetName = (string)dataRef[1];
            _mvmtCurve = (AnimationCurve)dataRef[2];
            _type = (TargetType)dataRef[3];
            _moveBy = (MoveBy)dataRef[4];
            if (_moveBy == MoveBy.Speed)
                _speed = (float)dataRef[5];
            else _duration = (float)dataRef[5];
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