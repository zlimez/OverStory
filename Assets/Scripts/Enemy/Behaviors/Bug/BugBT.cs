using System;
using System.Collections;
using System.Collections.Generic;
using Abyss.Environment.Enemy;
using BehaviorTree;
using BehaviorTree.Actions;
using Tuples;
using UnityEngine;

public class BugBT : MonoBT
{
    public Arena Arena;
    [SerializeField] float aftJumpDashTime;
    [Header("Drop Settings")]
    [SerializeField] AnimationCurve dropCurve;
    [SerializeField] float dropDuration;
    [SerializeField][Tooltip("Space that must be available to the bug to drop into")] float minSpace;

    [Header("Dash Settings")]
    public Transform LeftEnd;
    public Transform RightEnd;
    [SerializeField] AnimationCurve dashCurve;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDamage;

    [Header("Jump Settings")]
    [SerializeField] AnimationCurve jumpCurve;
    [SerializeField] float jumpDuration;
    [SerializeField] float waitBeforeJumpTime;

    public override void Setup()
    {
        StartCoroutine(SetupRoutine());
        GetComponent<EnemyManager>().OnDefeated += StopBT;
    }

    protected override void OnDisable()
    {
        GetComponent<EnemyManager>().OnDefeated -= StopBT;
        base.OnDisable();
    }

    // Invoked by spawner
    // NOTE: Delayed due to apPortrait bug
    IEnumerator SetupRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        // Blackboard bb = new(new Pair<string, string>[] { new(arena.EEEvent, arena.EEEvent) });
        EnemyAttr attr = GetComponent<EnemyManager>().attributes;
        attr = NormalizeEnemyAttr(attr);
        if (attr.friendliness < 0.5f) attr.speed *= 1.0f + (0.5f - attr.friendliness) * 2.0f;
        else attr.speed *= 1.0f - (attr.friendliness - 0.5f);


        Pair<string, object>[] bugParams = {
            new("bugTfm", transform),
            new("dropCurve", dropCurve),
            new("dropDuration", dropDuration),
            new("minSpace", minSpace),

            new("leftEnd", LeftEnd),
            new("rightEnd", RightEnd),
            new("dashCurve", dashCurve),
            new("dashSpeed", dashSpeed * attr.speed),
            new("dashDamage", dashDamage),

            new("jumpDest", "jumpDest"),
            new("jumpCurve", jumpCurve),
            new("jumpDuration", jumpDuration),
            new("jumpDestType", GotoTargetByCurve.Type.Vector3),
            new("waitBeforeJumpTime", waitBeforeJumpTime),

            new("bugSprite", GetComponent<SpriteManager>()),
            new("bugManager", GetComponent<EnemyManager>()),
            new("aftJumpDashTime", aftJumpDashTime / attr.speed),
            new("arena",Arena)
        };

        _bT = new BT(
            new Sequence(new List<Node> {
                new CheckPlayerInArena(new string[] {"arena"}),
                new DropFrmCanopy(new string[] { "leftEnd", "rightEnd", "bugTfm", "dropCurve", "dropDuration", "minSpace", "jumpDest", "bugSprite" }),
                new DashLeftOrRight(new string[] { "leftEnd", "rightEnd", "bugTfm", "dashCurve", "dashSpeed", "dashDamage", "bugManager" }),
                new Wait(new string[] { "waitBeforeJumpTime" }),
                new GotoTargetByCurve(new string[] { "bugTfm", "jumpDest", "jumpCurve", "jumpDuration", "jumpDestType" }),
                new Wait(new string[] { "aftJumpDashTime" }),
            })
        , bugParams, new Blackboard[] { });
    }
}
