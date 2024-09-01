using BehaviorTree;
using UnityEngine;

public class ReverseDir : CfAction
{
    Transform _transform;

    public ReverseDir(string[] parameters) : base(parameters) { }

    public override void Setup(BT tree)
    {
        base.Setup(tree);
        _transform = Tree.GetDatum<Transform>(_params[0]);
    }

    public override void Update()
    {
        Debug.Log("Reversed");
        _transform.Rotate(0, 180, 0);
        State = State.SUCCESS;
    }
}
