using UnityEngine;

using BehaviorTree;
using System.Collections.Generic;

public class GoToTarget : CfAction
{
    private Transform _transform;
    private float _speed;

    public GoToTarget(string[] parameters) : base(parameters) { }

    public override void Setup(BT tree)
    {
        base.Setup(tree);
        List<object> dataRef = Tree.GetData(_params);
        _speed = (float)dataRef[0];
        _transform = (Transform)dataRef[1];
    }

    // Indefinite pursuit
    public override void Update()
    {
        Transform target = Tree.GetDatum<Transform>("target", true);

        if (Vector3.Distance(_transform.position, target.position) > 0.01f)
        {
            _transform.position = Vector3.MoveTowards(
                _transform.position, target.position, _speed * Time.deltaTime);
            _transform.LookAt(target.position);
        } else State = State.SUCCESS;
    }

}