using System;
using System.Collections.Generic;
using BehaviorTree;
using Tuples;
using UnityEngine;

public class HogBT : MonoBehaviour
{
    [Header("Charge Settings")]
    // Stun time if during charge the hog hits an obstacle
    [SerializeField] float stunTime;
    [SerializeField] float shortRestTime;
    [SerializeField] float longRestTime;
    [SerializeField] float shortChargeDist;
    [SerializeField] float longChargeDist;
    [SerializeField] AnimationCurve chargeCurve;
    [SerializeField] float chargeSpeed;
    [Header("Patrol Settings")]
    [SerializeField] float patrolSpeed;
    [SerializeField] float waitTime;
    [SerializeField] Transform[] waypoints;
    [Header("Misc")]
    [SerializeField] Arena arena;

    BT bT;

    void Awake()
    {
        Blackboard bb = new(new Pair<string, string>[] { new(arena.EEEvent, arena.EEEvent) });
        Pair<string, object>[] hogParams = {
            new("stunTime", stunTime),
            new("shortRestTime", shortRestTime),
            new("longRestTime", longRestTime),
            new("shortChargeDist", shortChargeDist),
            new("longChargeDist", longChargeDist),
            new("chargeCurve", chargeCurve),
            new("chargeSpeed", chargeSpeed),

            new("patrolSpeed", patrolSpeed),
            new("waitTime", waitTime),
            new("waypoints", waypoints),

            new("arena", arena),
            new("hog", gameObject.transform)
        };

        bT = new BT(new ObserveSelector(new List<Node> {
            new Sequence(new List<Node> {
                new CheckPlayerInArena(new string[] { "arena" }),
                new FaceTarget(new string[] { "hog" }),
                new ProbSelector(
                    new List<Node> {
                        new Charge(new string[] { "stunTime", "longRestTime", "longChargeDist", "chargeSpeed", "chargeCurve", "hog" }),
                        new Sequence(new List<Node> {
                            new Charge(new string[] { "stunTime", "shortRestTime", "shortChargeDist", "chargeSpeed", "chargeCurve", "hog" }),
                            new Inverter(new CheckPlayerInFront(new string[] { "hog" })),
                            new ReverseDir(new string[] { "hog" }),
                            new Charge(new string[] { "stunTime", "shortRestTime", "shortChargeDist", "chargeSpeed", "chargeCurve", "hog" })
                        })
                    },
                    new Func<List<object>, float>[] {
                        (obj) => { return 0.5f; }, // TODO: Implement a function that returns the probability of the first action
                        (obj) => { return 0.5f; }
                    },
                    new string[][] { new string[] { "target", "hog" }, new string[] { "target", "hog" }}
                )
            })
            , new Patrol(new string[] { "patrolSpeed", "waypoints", "hog", "waitTime" })
        }, new string[] { arena.EEEvent }, (obj) => { return true; })
        , hogParams, new Blackboard[] { bb });
    }


    void Update()
    {
        bT.Tick();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 2.0f);
    }
}
