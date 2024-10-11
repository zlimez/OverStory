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
    [Header("Charge Settings")]
    // Stun time if during charge the hog hits an obstacle
    [SerializeField] float stunTime;
    [SerializeField] float shortRestTime;
    [SerializeField] float longRestTime;
    [SerializeField] float shortChargeupTime;
    [SerializeField] float longChargeupTime;
    [SerializeField] float shortChargeDist;
    [SerializeField] float longChargeDist;
    [SerializeField] float stunRaycastDist;
    [SerializeField] AnimationCurve chargeCurve;
    [SerializeField] Aggro aggro;
    [SerializeField] float chargeSpeed;
    [SerializeField] float chargeDamage;
    [SerializeField] float chargeDamageCooldown;
    [Header("Patrol Settings")]
    [SerializeField] float patrolSpeed;
    [SerializeField] float waitTime;
    public Transform[] waypoints;

#if DEBUG
    protected override void OnEnable()
    {
        Setup();
    }

    protected override void OnDisable()
    {
        GetComponent<EnemyManager>().OnDeath -= StopBT;
        StopBT();
    }
#endif
    public override void Setup()
    {
        StartCoroutine(SetupRoutine());
        GetComponent<EnemyManager>().OnDeath += StopBT;
    }

    // Invoked by spawner
    // NOTE: Delayed due to apPortrait bug
    IEnumerator SetupRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        Blackboard bb = new(new Pair<string, string>[] { new(aggro.EEEvent, aggro.EEEvent) });
        _bbs.Add(bb);
        Pair<string, object>[] hogParams = {
            new("stunTime", stunTime),
            new("shortRestTime", shortRestTime),
            new("longRestTime", longRestTime),
            new("shortChargeupTime", shortChargeupTime),
            new("longChargeupTime", longChargeupTime),
            new("shortChargeDist", shortChargeDist),
            new("longChargeDist", longChargeDist),
            new("chargeCurve", chargeCurve),
            new("chargeSpeed", chargeSpeed),
            new("stunRaycastDist", stunRaycastDist),

            new("patrolSpeed", patrolSpeed),
            new("waitTime", waitTime),
            new("waypoints", waypoints),

            new("aggro", aggro),
            new("hog", transform),
            new("hogSprite", GetComponent<SpriteManager>()),
            new("hogAnim", GetComponent<HogAnim>()),

            new("chargeDamage", chargeDamage),
            new("chargeDamageCooldown", chargeDamageCooldown),
            new("hogManager", GetComponent<EnemyManager>())
        };

        var shortChargeArgs = new string[] { "stunTime", "shortRestTime", "shortChargeupTime", "shortChargeDist", "chargeSpeed", "chargeCurve", "hog", "hogSprite", "hogAnim", "stunRaycastDist", "chargeDamage", "hogManager" };
        var longChargeArgs = new string[] { "stunTime", "longRestTime", "longChargeupTime", "longChargeDist", "chargeSpeed", "chargeCurve", "hog", "hogSprite", "hogAnim", "stunRaycastDist", "chargeDamage", "hogManager" };
        _bT = new BT(new ObserveSelector(new List<Node> {
            new Sequence(new List<Node> {
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
                        })
                    },
                    new Func<List<object>, float>[] {
                        (obj) => { return 0.5f; }, // TODO: Implement a function that returns the probability of the first action
                        (obj) => { return 0.5f; }
                    },
                    new string[][] { new string[] { "target", "hog" }, new string[] { "target", "hog" }}
                )
            })
            , new Patrol(new string[] { "patrolSpeed", "waypoints", "hog", "waitTime", "hogSprite", "hogAnim" })
        }, new string[] { aggro.EEEvent }, (obj) => { return true; })
        , hogParams, new Blackboard[] { bb });
    }

    protected override void OnDestroy()
    {
        GetComponent<EnemyManager>().OnDeath -= StopBT;
        base.OnDestroy();
    }

#if DEBUG
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, GetComponent<SpriteManager>().forward * stunRaycastDist);
    }
#endif
}
