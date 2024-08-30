using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;

public class CheckEnemyInRange : Action
{
    private static readonly int _enemyLayerMask = 1 << 6;
    private Transform _transform;
    private float _range;
    // private Animator _animator;

    public override void Setup(BT tree)
    {
        base.Setup(tree);
        List<object> dataRef = Tree.GetData(new List<string> { "charTransform", "range" });
        _transform = (Transform)dataRef[0];
        _range = (float)dataRef[1];
    }

    public override void Update()
    {
        Debug.Log("Checking enemy in range");
        var t = Tree.GetDatum<Transform>("target", true);
        if (t == null)
        {
            Collider[] colliders = Physics.OverlapSphere(_transform.position, _range, _enemyLayerMask);

            if (colliders.Length > 0)
            {
                Tree.SetDatum("target", colliders[0].transform);
                // _animator.SetBool("Walking", true);
                State = State.SUCCESS;
            } else State = State.FAILURE;
            return;
        }

        State = State.SUCCESS;
    }
}
