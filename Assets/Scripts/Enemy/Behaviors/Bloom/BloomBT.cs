using System;
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
    [SerializeField] float preChompTime, chompRestTime, chompDamage;
    [SerializeField] AnimationCurve chompCurve;
    [SerializeField] Transform neck1Tfm, neck2Tfm, neck3Tfm, headTfm;
    [Header("Pollen AOE Settings")]
    [SerializeField] float aoeTime;
    [SerializeField] float aoeTriggerRange;
    [SerializeField] ParticleSystem aoeParticleSystem;
    [SerializeField] float aoeRestTime;

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

    IEnumerator SetupRoutine()
    {
        yield return new WaitForSeconds(0.4f);
        Blackboard bb = new(new Pair<string, string>[] { new(aggro.EEEvent, aggro.EEEvent) });
        _bbs.Add(bb);

        EnemyAttr attr = GetComponent<EnemyManager>().attributes;
        attr = NormalizeEnemyAttr(attr);
        aggro.gameObject.GetComponent<BoxCollider2D>().size *= attr.alertness;
        float probNoAttack, probAttack;
        if (attr.friendliness > 0.9f) probNoAttack = 1.0f;
        else if (attr.friendliness > 0.5f) probNoAttack = attr.friendliness;
        else probNoAttack = 0.0f;
        probAttack = 1.0f - probNoAttack;
        if (attr.friendliness < 0.5f) attr.speed *= 1.0f + (0.5f - attr.friendliness) * 2.0f;
        else attr.speed *= 1.0f - (attr.friendliness - 0.5f);

        Pair<string, object>[] bloomParams = {
            new("aggro", aggro),
            new("bloomSprite", GetComponent<SpriteManager>()),
            new("bloomManager", GetComponent<EnemyManager>()),
            new("bloomTfm", transform),

            new("chompTime", chompTime / attr.speed),
            new("preChompTime", preChompTime/ attr.speed),
            new("chompRestTime", chompRestTime / attr.speed),
            new("chompDmg", chompDamage),
            new("chompCurve", chompCurve),
            new("bloomAnim", GetComponent<BloomAnim>()),
            new("neck1Tfm", neck1Tfm),
            new("neck2Tfm", neck2Tfm),
            new("neck3Tfm", neck3Tfm),
            new("headTfm", headTfm),

            new("aoeTime", aoeTime / attr.speed),
            new("aoeEmitter", aoeParticleSystem),
            new("aoeRestTime", aoeRestTime / attr.speed)
        };

        _bT = new BT(new Sequence(
            new List<Node> {
                new CheckPlayerInAggro(new string[] { "aggro" }),
                new ProbSelector(
                    new List<Node> {
                        new Sequence(new List<Node> {
                            new XFaceTarget(new string[] { "bloomSprite" }),
                            new Chomp(new string[] { "preChompTime", "chompTime", "chompRestTime", "chompDmg", "chompCurve", "bloomAnim", "neck1Tfm", "neck2Tfm", "neck3Tfm", "headTfm", "bloomManager" })
                        }),
                        new Sequence(new List<Node> {
                            new Aoe(new string[] { "aoeEmitter", "bloomAnim", "aoeTime" }),
                            new Wait(new string[] { "aoeRestTime" })
                        }),
                        new Wait(new string[] { "aoeRestTime" })
                    },
                    new Func<List<object>, float>[] {
                        (obj) => Mathf.Abs(((Transform)obj[0]).position.x - ((Transform)obj[1]).position.x) <= aoeTriggerRange ? 0.0f : probAttack,
                        (obj) => Mathf.Abs(((Transform)obj[0]).position.x - ((Transform)obj[1]).position.x) <= aoeTriggerRange ? probAttack : 0.0f,
                        (obj) => probNoAttack
                    },
                    new string[][] { new string[] { "target", "bloomTfm" }, new string[] { "target", "bloomTfm" }, new string[] { "target", "bloomTfm" }}
                )
            })
        , bloomParams, new Blackboard[] { bb });
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(aoeTriggerRange * 2, 1, 1));
    }
#endif
}
