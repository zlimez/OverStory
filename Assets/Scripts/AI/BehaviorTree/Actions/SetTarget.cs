using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree.Actions
{
    public class SetTarget : CfAction
    {
        string _targetName;
        Transform _transform;
        Transform[] _possTargets;
        Func<Transform, Transform, float> _targetingFunc; // Min value is chosen as target

        public SetTarget(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            List<object> dataRef = Tree.GetData(_params);
            _targetName = (string)dataRef[0];
            _transform = (Transform)dataRef[1];
            _possTargets = (Transform[])dataRef[2];
            _targetingFunc = (Func<Transform, Transform, float>)dataRef[3];
        }

        public override void Update()
        {
            Transform target = _possTargets[0];
            float minVal = _targetingFunc(target, _transform);
            for (int i = 1; i < _possTargets.Length; i++)
            {
                float val = _targetingFunc(_possTargets[i], _transform);
                if (val < minVal)
                {
                    minVal = val;
                    target = _possTargets[i];
                }
            }

            Tree.SetDatum(_targetName, target);
            State = State.SUCCESS;
        }
    }

    public class ClearVar : CfAction
    {
        string _varName;

        public ClearVar(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            _varName = _params[0];
        }

        public override void Update()
        {
            Tree.ClearDatum(_varName);
            State = State.SUCCESS;
        }
    }

    public class SetVar : CfAction
    {
        string _varName;
        object _value;

        public SetVar(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            _varName = _params[0];
            _value = Tree.GetDatum<object>(_params[1]);
        }

        public override void Update()
        {
            Tree.SetDatum(_varName, _value);
            State = State.SUCCESS;
        }
    }
}
