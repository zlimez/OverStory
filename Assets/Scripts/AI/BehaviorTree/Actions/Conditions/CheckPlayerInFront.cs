using BehaviorTree;
using UnityEngine;

public class CheckPlayerInFront : CfAction
{
    private Transform _transform;

    public CheckPlayerInFront(string[] parameters) : base(parameters) { }

    public override void Setup(BT tree)
    {
        base.Setup(tree);
        _transform = Tree.GetDatum<Transform>(_params[0]);
    }

    public override void Update()
    {
        State = Vector3.Dot(_transform.forward, Tree.GetDatum<Transform>("target").position - _transform.position) > 0 ? State.SUCCESS : State.FAILURE;
    }
}
