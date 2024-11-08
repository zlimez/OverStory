using System.Collections.Generic;
using Abyss.EventSystem;
using BehaviorTree;
using BehaviorTree.Actions;
using Tuples;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DroneBT : MonoBT
{
    [Header("Follow Settings")]
    [SerializeField] Transform followTransform;
    [SerializeField] float smoothTime;
    [Header("Follow Hover Settings")]
    [SerializeField] float hoverInterval;
    [SerializeField] Transform hoverTop, hoverBtm;
    [SerializeField][Tooltip("Threshold distance from the player that the drone will stop follow and start hovering")] float distToStop;
    [SerializeField][Tooltip("Threshold distance from the player that the drone will exit hover and start following again")] float distToStart;
    [Header("Construction Settings")]
    [SerializeField] float timeToBuildLoc;
    [SerializeField] float transitionTime;
    [SerializeField] AnimationCurve moveToBuildLocCurve, transitionCurve;
    [SerializeField] Light2D headLight;
    [SerializeField] float buildIntensity, buildZRot;
    [SerializeField] Color buildColor;
    [SerializeField] Vector3 buildHoverAmp;


    public override void Setup()
    {
        Blackboard bb = new(new Pair<string, string[]>[] { new("buildLoc", new string[] { PlayEvents.BuildStart.ToString(), PlayEvents.BuildEnd.ToString() }) });
        Pair<string, object>[] droneParams = {
            new("followTransform", followTransform),
            new("droneTransform", transform),
            new("droneSprite", GetComponent<SpriteManager>()),
            new("smoothTime", smoothTime),
             new("distToStop", distToStop),
            new("distToStart", distToStart),

            new("hoverTop", hoverTop),
            new("hoverBtm", hoverBtm),
            new("hoverInterval", hoverInterval),

            new("mvToBuildCurve", moveToBuildLocCurve),
            new("mvToBuildType", GotoTargetByCurve.TargetType.Transform),
            new("mvToBuildBy", GotoTargetByCurve.MoveBy.Duration),
            new("mvToBuildDuration", timeToBuildLoc),

            new("headLight", headLight),
            new("normalIntensity", headLight.intensity),
            new("buildIntensity", buildIntensity),
            new("normalColor", headLight.color),
            new("buildColor", buildColor),
            new("normalZRot", transform.eulerAngles.z),
            new("buildZRot", buildZRot),
            new("buildHoverAmp", buildHoverAmp),
            new("transitionCurve", transitionCurve),
            new("transitionTime", transitionTime)
        };

        _bT = new BT(new ObserveSelector(new List<Node> {
            new ObserveSequence(new List<Node> {
                new CheckTargetExists(new string[] { "buildLoc" }),
                new XFaceTarget(new string[] { "droneSprite", "buildLoc" }),
                new GotoTargetByCurve(new string[] { "droneTransform", "buildLoc", "mvToBuildCurve", "mvToBuildType", "mvToBuildBy", "mvToBuildDuration" }),
                new DroneAdjust(new string[] { "headLight", "transitionTime", "buildIntensity", "buildColor", "buildZRot", "transitionCurve", "droneTransform", "droneSprite" }),
                new Hover(new string[] { "buildLoc", "buildHoverAmp", "hoverInterval", "droneTransform" })
            }, new string[] { "buildLoc" }, (obj) => { return true; }),

            new Failer(new Selector(new List<Node> {
                new DroneInDefault(new string[] { "headLight", "normalIntensity", "normalColor", "droneTransform" }),
                new DroneAdjust(new string[] { "headLight", "transitionTime", "normalIntensity", "normalColor", "normalZRot", "transitionCurve", "droneTransform", "droneSprite" }),
            })),

            new DroneFollow(new string[] { "followTransform", "droneTransform", "smoothTime", "hoverTop", "hoverBtm", "hoverInterval", "distToStop", "distToStart", "droneSprite" })
        }, new string[] { "buildLoc" }, (obj) => { return true; })
        , droneParams, new Blackboard[] { bb });
    }
}
