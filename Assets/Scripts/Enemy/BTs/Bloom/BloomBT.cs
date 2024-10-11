using System.Collections;
using System.Collections.Generic;
using Abyss.Environment.Enemy;
using Abyss.Environment.Enemy.Anim;
using BehaviorTree;
using BehaviorTree.Actions;
using Tuples;
using UnityEngine;

public class BloomBT : MonoBT
{
    [SerializeField] Aggro aggro;
    [Header("Chomp Settings")]
    [SerializeField] float chompTime;
    [SerializeField] float restTime, chompDamage;
    [SerializeField] AnimationCurve chompCurve;
    [SerializeField] Transform neck1Tfm, neck2Tfm, neck3Tfm, headTfm;

    public override void Setup()
    {
        StartCoroutine(SetupRoutine());
        GetComponent<EnemyManager>().OnDeath += StopBT;
    }

    IEnumerator SetupRoutine()
    {
        yield return new WaitForSeconds(0.4f);
        Blackboard bb = new(new Pair<string, string>[] { new(aggro.EEEvent, aggro.EEEvent) });
        _bbs.Add(bb);
        Pair<string, object>[] bloomParams = {
            new("aggro", aggro),
            new("bloomSprite", GetComponent<SpriteManager>()),
            new("bloomManager", GetComponent<EnemyManager>()),

            new("chompTime", chompTime),
            new("restTime", restTime),
            new("chompDmg", chompDamage),
            new("chompCurve", chompCurve),
            new("bloomAnim", GetComponent<BloomAnim>()),
            new("neck1Tfm", neck1Tfm),
            new("neck2Tfm", neck2Tfm),
            new("neck3Tfm", neck3Tfm),
            new("headTfm", headTfm)
        };

        _bT = new BT(new Sequence(
            new List<Node> {
                new CheckPlayerInAggro(new string[] { "aggro" }),
                new XFaceTarget(new string[] { "bloomSprite" }),
                new Chomp(new string[] { "chompTime", "restTime", "chompDmg", "chompCurve", "bloomAnim", "neck1Tfm", "neck2Tfm", "neck3Tfm", "headTfm", "bloomManager" })
            })
        , bloomParams, new Blackboard[] { bb });
    }
}
