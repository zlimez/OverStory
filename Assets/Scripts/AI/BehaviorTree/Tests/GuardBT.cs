using System.Collections.Generic;
using BehaviorTree;
using Tuples;
using UnityEngine;

public class GuardBT : MonoBehaviour
{
    // public float updateInterval = 0.5f;
    // private float timeSinceLastUpdate = 0.0f;

    [SerializeField] int hitpoints;
    [SerializeField] float patrolSpeed;
    [SerializeField] float chaseSpeed;
    [SerializeField] float range;
    [SerializeField] float waitTime;
    [SerializeField] float attackTime;
    [SerializeField] Transform[] waypoints;
    private Transform _charTransform;
    private BT bT;

    // Start is called before the first frame update
    void Awake()
    {
        _charTransform = GetComponent<Transform>();
        Pair<string, object>[] charParams = {
            new("hitpoints", hitpoints),
            new("patrolSpeed", patrolSpeed),
            new("chaseSpeed", chaseSpeed),
            new("range", range),
            new("waitTime", waitTime),
            new("attackTime", attackTime),
            new("waypoints", waypoints),
            new("charTransform", _charTransform)
        };

        bT = new BT(new ActiveSelector(new List<Node>{
            new Sequence(new List<Node>{
                new CheckEnemyInRange(),
                new GoToTarget(),
                new Attack()
            }),
            new Patrol()
        }), charParams);
        // bT = new BT(new ActiveSequence(new List<Node>{
        //     new CheckEnemyInRange(),
        //     new GoToTarget(),
        //     new Attack()
        // }), charParams);
    }

    void Update()
    {
        bT.Tick();
    }
}
