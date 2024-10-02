using BehaviorTree;
using BehaviorTree.Actions;
using Tuples;
using UnityEngine;

public class DroneBT : MonoBT
{
    [Header("Follow Settings")]
    [SerializeField] Transform followTransform;
    [SerializeField] float smoothTime;
    [Header("Hover Settings")]
    [SerializeField] Transform hoverTop;
    [SerializeField] Transform hoverBtm;
    [SerializeField] float hoverInterval;
    [SerializeField][Tooltip("Threshold distance from the player that the drone will stop follow and start hovering")] float distToStop;
    [SerializeField][Tooltip("Threshold distance from the player that the drone will exit hover and start following again")] float distToStart;


    public override void Setup()
    {
        Pair<string, object>[] droneParams = {
            new("followTransform", followTransform),
            new("droneTransform", transform),
            new("smoothTime", smoothTime),

            new("hoverTop", hoverTop),
            new("hoverBtm", hoverBtm),
            new("hoverInterval", hoverInterval),

            new("distToStop", distToStop),
            new("distToStart", distToStart),

            new("droneSprite", GetComponent<SpriteManager>())
        };

        _bT = new BT(new FollowTarget(new string[] { "followTransform", "droneTransform", "smoothTime", "hoverTop", "hoverBtm", "hoverInterval", "distToStop", "distToStart", "droneSprite" }), droneParams);
    }
}
