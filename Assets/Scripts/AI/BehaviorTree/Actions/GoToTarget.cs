using UnityEngine;

using BehaviorTree;
using System.Collections.Generic;

public class GoToTarget : Action
{
    private Transform _transform;
    private float _speed;

    public override void Setup(BT tree)
    {
        base.Setup(tree);
        List<object> dataRef = Tree.GetData(new List<string> { "chaseSpeed", "charTransform" });
        _speed = (float)dataRef[0];
        _transform = (Transform)dataRef[1];
    }

    // Indefinite pursuit
    public override void Update()
    {
        Debug.Log("Going to target");
        Transform target = Tree.GetDatum<Transform>("target", true);

        if (Vector3.Distance(_transform.position, target.position) > 0.01f)
        {
            _transform.position = Vector3.MoveTowards(
                _transform.position, target.position, _speed * Time.deltaTime);
            _transform.LookAt(target.position);
        } else State = State.SUCCESS;
    }

}