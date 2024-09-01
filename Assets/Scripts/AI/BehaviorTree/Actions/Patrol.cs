using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class Patrol : CfAction
{
    private int _currWaypoint = 0;
    private float _waitCounter = 0f;
    private bool _waiting = false;
    private float _speed;
    private Transform[] _waypoints;
    private Transform _transform;
    private float _waitTime;

    public Patrol(string[] parameters) : base(parameters) { }

    public override void Setup(BT tree)
    {
        base.Setup(tree);
        List<object> dataRef = Tree.GetData(_params);
        _speed = (float)dataRef[0];
        _waypoints = (Transform[])dataRef[1];
        _transform = (Transform)dataRef[2];
        _waitTime = (float)dataRef[3];
    }

    public override void Update()
    {
        if (_waiting)
        {
            _waitCounter += Time.deltaTime;
            if (_waitCounter >= _waitTime)
            {
                _waiting = false;
                // _animator.SetBool("Walking", true);
            }
        }
        else
        {
            Transform wp = _waypoints[_currWaypoint];
            if (Vector3.Distance(_transform.position, wp.position) < 0.01f)
            {
                _transform.position = wp.position;
                _waitCounter = 0f;
                _waiting = true;

                _currWaypoint = (_currWaypoint + 1) % _waypoints.Length;
                // _animator.SetBool("Walking", false);
            }
            else
            {
                _transform.position = Vector3.MoveTowards(_transform.position, wp.position, _speed * Time.deltaTime);
                _transform.LookAt(wp.position);
            }
        }
    }
}
