// using System.Collections.Generic;
// using BehaviorTree;
// using BehaviorTree.Actions;
// using Tuples;
// using UnityEngine;

// public class GuardBT : MonoBehaviour
// {
//     [SerializeField] int hitpoints;
//     [SerializeField] float patrolSpeed;
//     [SerializeField] float chaseSpeed;
//     [SerializeField] float range;
//     [SerializeField] float waitTime;
//     [SerializeField] float attackTime;
//     [SerializeField] Transform[] waypoints;
//     private BT bT;

//     void Awake()
//     {
//         Pair<string, object>[] charParams = {
//             new("hitpoints", hitpoints),
//             new("patrolSpeed", patrolSpeed),
//             new("chaseSpeed", chaseSpeed),
//             new("range", range),
//             new("waitTime", waitTime),
//             new("attackTime", attackTime),
//             new("waypoints", waypoints),
//             new("charTransform", GetComponent<Transform>())
//         };

//         bT = new BT(new ActiveSelector(new List<Node>{
//             new Sequence(new List<Node>{
//                 new CheckEnemyInRange(new string[] { "charTransform", "range" }),
//                 new GotoTarget(new string[] { "chaseSpeed", "charTransform" }),
//                 new Attack(new string[] { "attackTime", "hitpoints" })
//             }),
//             new Patrol(new string[] { "patrolSpeed", "waypoints", "charTransform", "waitTime" })
//         }), charParams);
//     }

//     void Update()
//     {
//         bT.Tick();
//     }
// }
