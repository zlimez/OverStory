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

    public class CheckVarExists : CfAction
    {
        string _varName;
        public CheckVarExists(string[] parameters) : base(parameters) { }
        public override void Setup(BT tree)
        {
            base.Setup(tree);
            _varName = _params[0];
        }

        public override void Update()
        {
            Debug.Log("Checking if " + _varName + " exists");
            State = Tree.GetDatum<object>(_varName, true) != null ? State.SUCCESS : State.FAILURE;
        }
    }
}
