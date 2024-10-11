using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;
using Abyss.Environment.Enemy.Anim;
using Abyss.Player;
using Abyss.Environment.Enemy;

public class Charge : CfAction
{
    static readonly int _obstacleLayerMask = 1 << 7;
    float _stunTime;
    float _restTime;
    float _chargeDist;
    float _chargeupTime;
    float _chargeSpeed; // Avg speed of charge
    float _stunRaycastDist;
    float _chargeDmg;
    AnimationCurve _chargeCurve;
    Transform _transform;
    SpriteManager _spriteManager;
    HogAnim _chargeTypeAnim;
    EnemyManager _enemyManager;

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
        _chargeDmg = (float)dataRef[10];
        _enemyManager = (EnemyManager)dataRef[11];
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
                State = State.SUCCESS; // One set of charge is done
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
                _enemyManager.OnStrikePlayer -= ChargeHit;
                isResting = true;
                pauseTime = _restTime;
                return;
            }

            // Consider collision into wall
            if (Physics2D.Raycast(_transform.position, _spriteManager.forward, _stunRaycastDist, _obstacleLayerMask))
            {
                _chargeTypeAnim.TransitionToState(HogAnim.State.Stun.ToString());
                _enemyManager.OnStrikePlayer -= ChargeHit;
                isResting = true;
                isStunned = true;
                pauseTime = _restTime + _stunTime;
                return;
            }

            if (chargeTimer >= _chargeupTime)
            {
                _enemyManager.OnStrikePlayer += ChargeHit;
                float t = (chargeTimer - _chargeupTime) / chargeTime;
                _transform.position = _chargeCurve.Evaluate(t) * _chargeDist * _spriteManager.forward + startPos;
                _chargeTypeAnim.StateBySpeed(Utils.Curves.GetGradient(_chargeCurve, t) * _chargeDist);
            }

            chargeTimer += Time.deltaTime;
        }
    }

    protected override void OnInit()
    {
        base.OnInit();
        chargeTimer = 0;
        isResting = false;
        isStunned = false;
        restTimer = 0;
    }

    void ChargeHit(float str)
    {
        Tree.GetDatum<Transform>("target").gameObject.GetComponent<PlayerManager>().TakeHit(str + _chargeDmg, true, _enemyManager.transform.position);
    }
}
