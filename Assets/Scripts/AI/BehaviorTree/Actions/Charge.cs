using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using Environment.Enemy.Anim;

public class Charge : CfAction
{
    static readonly int _obstacleLayerMask = 1 << 7;
    float _stunTime;
    float _restTime;
    float _chargeDist;
    float _chargeupTime;
    float _chargeSpeed; // Avg speed of charge
    float _stunRaycastDist;
    AnimationCurve _chargeCurve;
    Transform _transform;
    SpriteManager _spriteManager;
    HogAnim _chargeTypeAnim;

    Vector3 startPos;
    float chargeTime;
    float chargeTimer = 0;
    float pauseTime = 0;
    bool isResting = false;
    bool isStunned = false;
    float restTimer = 0;

    public Charge(string[] _params) : base(_params) { }

    public override void Setup(BT tree)
    {
        base.Setup(tree);
        List<object> dataRef = Tree.GetData(_params);
        _stunTime = (float)dataRef[0];
        _restTime = (float)dataRef[1];
        _chargeupTime = (float)dataRef[2];
        _chargeDist = (float)dataRef[3];
        _chargeSpeed = (float)dataRef[4];
        _chargeCurve = (AnimationCurve)dataRef[5];
        _transform = (Transform)dataRef[6];
        _spriteManager = (SpriteManager)dataRef[7];
        _chargeTypeAnim = (HogAnim)dataRef[8];
        _stunRaycastDist = (float)dataRef[9];
        chargeTime = _chargeDist / _chargeSpeed;
    }

    public override void Update()
    {
        if (isResting)
        {
            if (isStunned && restTimer >= _stunTime)
            {
                isStunned = false;
                _chargeTypeAnim.TransitionToState(HogAnim.State.Wake.ToString());
            }
            else if (restTimer >= pauseTime)
            {
                isResting = false;
                restTimer = 0;
                State = State.SUCCESS; // One set of charge is done
            }
            else
                restTimer += Time.deltaTime;
        }
        else
        {
            if (chargeTimer == 0)
            {
                _chargeTypeAnim.TransitionToState(HogAnim.State.ChargeUp.ToString());
                startPos = _transform.position;
            }

            if (chargeTimer >= chargeTime + _chargeupTime)
            {
                _chargeTypeAnim.TransitionToState(HogAnim.State.Idle.ToString());
                isResting = true;
                chargeTimer = 0;
                pauseTime = _restTime;
                return;
            }

            // Consider collision into wall
            if (Physics2D.Raycast(_transform.position, _spriteManager.forward, _stunRaycastDist, _obstacleLayerMask))
            {
                _chargeTypeAnim.TransitionToState(HogAnim.State.Stun.ToString());
                isResting = true;
                isStunned = true;
                chargeTimer = 0;
                pauseTime = _restTime + _stunTime;
                return;
            }

            if (chargeTimer >= _chargeupTime)
            {
                float t = (chargeTimer - _chargeupTime) / chargeTime;
                _transform.position = _chargeCurve.Evaluate(t) * _chargeDist * _spriteManager.forward + startPos;
                _chargeTypeAnim.StateBySpeed(Utils.Curves.GetGradient(_chargeCurve, t) * _chargeDist);
            }

            chargeTimer += Time.deltaTime;
        }
    }
}
