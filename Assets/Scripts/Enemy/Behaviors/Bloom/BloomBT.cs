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
    [SerializeField] float chompRestTime, chompDamage;
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
            new("bloomTfm", transform),

            new("chompTime", chompTime),
            new("chompRestTime", chompRestTime),
            new("chompDmg", chompDamage),
            new("chompCurve", chompCurve),
            new("bloomAnim", GetComponent<BloomAnim>()),
            new("neck1Tfm", neck1Tfm),
            new("neck2Tfm", neck2Tfm),
            new("neck3Tfm", neck3Tfm),
            new("headTfm", headTfm),

            new("aoeTime", aoeTime),
            new("aoeEmitter", aoeParticleSystem),
            new("aoeRestTime", aoeRestTime)
        };

        _bT = new BT(new Sequence(
            new List<Node> {
                new CheckPlayerInAggro(new string[] { "aggro" }),
                new ProbSelector(
                    new List<Node> {
                        new Sequence(new List<Node> {
                            new XFaceTarget(new string[] { "bloomSprite" }),
                            new Chomp(new string[] { "chompTime", "chompRestTime", "chompDmg", "chompCurve", "bloomAnim", "neck1Tfm", "neck2Tfm", "neck3Tfm", "headTfm", "bloomManager" })
                        }),
                        new Sequence(new List<Node> {
                            new Aoe(new string[] { "aoeEmitter", "bloomAnim", "aoeTime" }),
                            new Wait(new string[] { "aoeRestTime" })
                        })
                    },
                    new Func<List<object>, float>[] {
                        (obj) => Mathf.Abs(((Transform)obj[0]).position.x - ((Transform)obj[1]).position.x) <= aoeTriggerRange ? 0.0f : 1.0f,
                        (obj) => Mathf.Abs(((Transform)obj[0]).position.x - ((Transform)obj[1]).position.x) <= aoeTriggerRange ? 1.0f : 0.0f
                    },
                    new string[][] { new string[] { "target", "bloomTfm" }, new string[] { "target", "bloomTfm" } }
                )
            })
        , bloomParams, new Blackboard[] { bb });
    }

#if DEBUG
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(aoeTriggerRange * 2, 1, 1));
    }
#endif
}
