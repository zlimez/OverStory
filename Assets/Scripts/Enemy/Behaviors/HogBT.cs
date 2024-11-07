using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using BehaviorTree.Actions;
using Abyss.Environment.Enemy;
using Abyss.Environment.Enemy.Anim;
using Tuples;
using UnityEngine;

public class HogBT : MonoBT
{
    public Arena Arena;
    [Header("Charge Settings")]
    // Stun time if during charge the hog hits an obstacle
    [SerializeField] float stunTime;
    [SerializeField] float shortRestTime, longRestTime;
    [SerializeField] float shortChargeupTime, longChargeupTime;
    [SerializeField] float shortChargeDist, longChargeDist;
    [SerializeField] float stunRaycastDist;
    [SerializeField] AnimationCurve chargeCurve;
    [SerializeField] Aggro aggro;
    [SerializeField] float chargeSpeed;
    [SerializeField] float chargeDamage, chargeDamageCooldown;
    [Header("Patrol Settings")]
    [SerializeField] float patrolSpeed, waitTime;
    public Transform[] Waypoints;

    protected override void OnDisable()
    {
        GetComponent<EnemyManager>().OnDefeated -= StopBT;
        base.OnDisable();
    }

    public override void Setup()
    {
        StartCoroutine(SetupRoutine());
        GetComponent<EnemyManager>().OnDefeated += StopBT;
    }

    // Invoked by spawner
    // NOTE: Delayed due to apPortrait bug
    IEnumerator SetupRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        Blackboard bb = new(new Pair<string, string>[] { new(aggro.EEEvent, aggro.EEEvent), new(Arena.EEEvent, Arena.EEEvent) });
        _bbs.Add(bb);

        EnemyAttr attr = GetComponent<EnemyManager>().attributes;
        attr = NormalizeEnemyAttr(attr);
        aggro.gameObject.GetComponent<BoxCollider2D>().size *= attr.alertness;
        float probLongCharge, probShortCharge, probNoAttack;
        if (attr.friendliness > 0.9f)
        {
            probNoAttack = 1.0f;
            probShortCharge = 0.0f;
        }
        else if (attr.friendliness > 0.5f)
        {
            probNoAttack = attr.friendliness;
            probShortCharge = (1.0f - probNoAttack) / 2.0f;
        }
        else
        {
            probNoAttack = 0.0f;
            probShortCharge = attr.friendliness;
        }
        probLongCharge = 1.0f - probShortCharge - probNoAttack;

        Pair<string, object>[] hogParams = {
            new("stunTime", stunTime / attr.speed),
            new("shortRestTime", shortRestTime / attr.speed),
            new("longRestTime", longRestTime / attr.speed),
            new("shortChargeupTime", shortChargeupTime / attr.speed),
            new("longChargeupTime", longChargeupTime / attr.speed),
            new("shortChargeDist", shortChargeDist),
            new("longChargeDist", longChargeDist),
            new("chargeCurve", chargeCurve),
            new("chargeSpeed", chargeSpeed * attr.speed),
            new("stunRaycastDist", stunRaycastDist),

            new("patrolSpeed", patrolSpeed * attr.speed),
            new("waitTime", waitTime / attr.speed),
            new("waypoints", Waypoints),

            new("arena", Arena),
            new("aggro", aggro),
            new("hog", transform),
            new("hogSprite", GetComponent<SpriteManager>()),
            new("hogAnimator", GetComponent<PortraitAnim>()),
            new("hogWalkAnim", HogAnim.State.Walk.ToString()),
            new("hogIdleAnim", HogAnim.State.Idle.ToString()),

            new("chargeDamage", chargeDamage),
            new("chargeDamageCooldown", chargeDamageCooldown),
            new("hogManager", GetComponent<EnemyManager>())
        };

        var shortChargeArgs = new string[] { "stunTime", "shortRestTime", "shortChargeupTime", "shortChargeDist", "chargeSpeed", "chargeCurve", "hog", "hogSprite", "hogAnimator", "stunRaycastDist", "chargeDamage", "hogManager" };
        var longChargeArgs = new string[] { "stunTime", "longRestTime", "longChargeupTime", "longChargeDist", "chargeSpeed", "chargeCurve", "hog", "hogSprite", "hogAnimator", "stunRaycastDist", "chargeDamage", "hogManager" };
        _bT = new BT(new ObserveSelector(new List<Node> {
            new Sequence(new List<Node> {
                new CheckPlayerInArena(new string[] { "arena" }),
                new CheckPlayerInAggro(new string[] { "aggro" }),
                new XFaceTarget(new string[] { "hogSprite" }),
                new ProbSelector(
                    new List<Node> {
                        new Charge(longChargeArgs),
                        new Sequence(new List<Node> {
                            new Charge(shortChargeArgs),
                            new Inverter(new CheckPlayerInFront(new string[] { "hog", "hogSprite" })),
                            new ReverseDir(new string[] { "hogSprite" }),
                            new Charge(shortChargeArgs),
                        }),
                        new Patrol(new string[] { "patrolSpeed", "waypoints", "hog", "waitTime", "hogSprite", "hogAnimator", "hogIdleAnim", "hogWalkAnim" })
                    },
                    new Func<List<object>, float>[] {
                        (obj) => { return probLongCharge; },
                        (obj) => { return probShortCharge; },
                        (obj) => { return probNoAttack; },
                    },
                    new string[][] { new string[] { "target", "hog" }, new string[] { "target", "hog" }, new string[] { "target", "hog" }}
                )
            }), new Patrol(new string[] { "patrolSpeed", "waypoints", "hog", "waitTime", "hogSprite", "hogAnimator", "hogIdleAnim", "hogWalkAnim" })
        }, new string[] { aggro.EEEvent, Arena.EEEvent }, (obj) => { return true; })
        , hogParams, new Blackboard[] { bb });
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, GetComponent<SpriteManager>().forward * stunRaycastDist);
    }
#endif
}
