using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class Charge : CfAction
{
    static readonly int _obstacleLayerMask = 1 << 7;
    float _stunTime;
    float _restTime;
    float _chargeDist;
    float _chargeSpeed; // Avg speed of charge
    AnimationCurve _chargeCurve;
    Transform _transform;

    Vector3 startPos;
    float chargeTime;
    float chargeTimer = 0;
    float pauseTime = 0;
    bool isResting = false;
    float restTimer = 0;

    public Charge(string[] _params) : base(_params) { }

    public override void Setup(BT tree)
    {
        base.Setup(tree);
        List<object> dataRef = Tree.GetData(_params);
        _stunTime = (float)dataRef[0];
        _restTime = (float)dataRef[1];
        _chargeDist = (float)dataRef[2];
        _chargeSpeed = (float)dataRef[3];
        _chargeCurve = (AnimationCurve)dataRef[4];
        _transform = (Transform)dataRef[5];
        chargeTime = _chargeDist / _chargeSpeed;
    }

    public override void Update()
    {
        if (isResting)
        {
            if (restTimer >= pauseTime)
            {
                isResting = false;
                restTimer = 0;
                State = State.SUCCESS;
            }
            else
                restTimer += Time.deltaTime;
        }
        else
        {
            if (chargeTimer == 0) startPos = _transform.position;

            if (chargeTimer >= chargeTime)
            {
                isResting = true;
                chargeTimer = 0;
                pauseTime = _restTime;
                return;
            }

            float t = chargeTimer / chargeTime;
            _transform.position = _chargeCurve.Evaluate(t) * _chargeDist * _transform.forward + startPos;
            
            // Consider collision into wall
            if (Physics.Raycast(startPos, _transform.forward, 0.5f, _obstacleLayerMask))
            {
                isResting = true;
                chargeTimer = 0;
                pauseTime = _restTime + _stunTime;
                return;
            }

            chargeTimer += Time.deltaTime;
        }
    }
}
