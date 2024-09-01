using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;

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
        _transform.LookAt(Tree.GetDatum<Transform>("target"));
        State = State.SUCCESS;
    }
}
