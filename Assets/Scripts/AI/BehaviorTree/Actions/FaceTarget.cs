using UnityEngine;

// DO NOT USE
namespace BehaviorTree.Actions
{
    public class FaceTarget : CfAction
    {
        Transform _transform;

        public FaceTarget(string[] parameters) : base(parameters) { }

        public override void Setup(BT tree)
        {
            base.Setup(tree);
            _transform = Tree.GetDatum<Transform>(_params[0]);
        }

        public override void Update()
        {
            _transform.LookAt(new Vector3(Tree.GetDatum<Transform>("target").position.x, _transform.position.y, _transform.position.z));
            State = State.SUCCESS;
        }
    }
}
