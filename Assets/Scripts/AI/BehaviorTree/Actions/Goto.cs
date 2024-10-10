using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree.Actions
{
    public class Goto : CfAction
    {
        Transform _transform;
        Transform _dest;
        AnimationCurve _mvmtCurve;
        float _duration;
        float _timer = 0;
        Vector3 _startPos;

        public Goto(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            List<object> dataRef = Tree.GetData(_params);
            _transform = (Transform)dataRef[0];
            _dest = (Transform)dataRef[1];
            _mvmtCurve = (AnimationCurve)dataRef[2];
            _duration = (float)dataRef[3];
        }

        public override void Update()
        {
            if (_timer < _duration)
                _transform.position = Vector3.Lerp(_startPos, _dest.position, _mvmtCurve.Evaluate(_timer / _duration));
            else State = State.SUCCESS;
            _timer += Time.deltaTime;
        }

        protected override void OnInit()
        {
            base.OnInit();
            _startPos = _transform.position;
            _timer = 0;
        }
    }

    public class Teleport : CfAction
    {
        Transform _transform;
        Transform _dest;

        public Teleport(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            List<object> dataRef = Tree.GetData(_params);
            _transform = (Transform)dataRef[0];
            _dest = (Transform)dataRef[1];
        }

        public override void Update()
        {
            _transform.position = _dest.position;
            State = State.SUCCESS;
        }
    }
}
