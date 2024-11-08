using UnityEngine;

namespace BehaviorTree.Actions
{
    public class CheckPlayerInFront : CfAction
    {
        Transform _transform;
        SpriteManager _spriteManager;

        public CheckPlayerInFront(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            _transform = Tree.GetDatum<Transform>(_params[0]);
            _spriteManager = Tree.GetDatum<SpriteManager>(_params[1]);
        }

        public override void Update() => State = Vector3.Dot(_spriteManager.forward, Tree.GetDatum<Transform>("target").position - _transform.position) > 0 ? State.SUCCESS : State.FAILURE;
    }

    public class CheckTargetExists : CfAction
    {
        string _targetName;
        public CheckTargetExists(string[] parameters) : base(parameters) { }
        public override void Setup(BT tree)
        {
            base.Setup(tree);
            _targetName = _params[0];
        }

        public override void Update()
        {
            Debug.Log("Checking target exists: " + _targetName + " " + Tree.GetDatum<object>(_targetName, true));
            State = Tree.GetDatum<object>(_targetName, true) != null ? State.SUCCESS : State.FAILURE;
        }
    }
}
