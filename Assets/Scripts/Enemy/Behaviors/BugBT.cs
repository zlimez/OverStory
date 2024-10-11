// using System.Collections;
// using System.Collections.Generic;
// using Abyss.Environment.Enemy;
// using BehaviorTree;
// using BehaviorTree.Actions;
// using Tuples;
// using UnityEngine;

// public class BugBT : MonoBT
// {
// 	[SerializeField] Arena arena;
// 	[Header("Dash Settings")]
// 	[SerializeField] float dashDistance;
// 	[SerializeField] float dashSpeed;

// 	[Header("Run Settings")]
// 	[SerializeField] float runSpeed;
// 	[SerializeField] Transform dropDownPoint;

// 	[Header("Jump Settings")]
// 	[SerializeField] float waitBeforeJumpTime;

// 	public override void Setup()
// 	{
// 		StartCoroutine(SetupRoutine());
// 		GetComponent<EnemyManager>().OnDeath += StopBT;
// 	}

// 	// Invoked by spawner
// 	// NOTE: Delayed due to apPortrait bug
// 	IEnumerator SetupRoutine()
// 	{
// 		yield return new WaitForSeconds(0.1f);
// 		var aggro = GetComponent<Aggro>();
// 		Blackboard bb = new(new Pair<string, string>[] { new(aggro.EEEvent, aggro.EEEvent) });
// 		Pair<string, object>[] bugParams = {
// 			new("dashDistance", dashDistance),
// 			new("dashSpeed", dashSpeed),

// 			new("runSpeed", runSpeed),
// 			new("dropDownPoint", dropDownPoint),
// 			new("waitBeforeJumpTime", waitBeforeJumpTime),

// 			new("bugSprite", GetComponent<SpriteManager>()),
// 			new("arena",arena)
// 		};

// 		_bT = new BT(new Sequence(new List<Node> {
// 			new CheckPlayerInArena(new string[] {"arena"}),
// 			// new Appear(), //TODO: Add an appear action?
// 			new XFaceTarget(new string[] {"bugSprite"}),
// 			new GoToTarget(new string[] {"dashSpeed"}), //TODO: Add a param with target location (transform)
// 			new ReverseDir(new string[] {"bugSprite"}),
// 			new GoToTarget(new string[] {"runSpeed", "dropDownPoint"}),
// 			// new Disappear() //TODO: Implement
// 		})
// 		, bugParams, new Blackboard[] { bb });
// 	}
// }
