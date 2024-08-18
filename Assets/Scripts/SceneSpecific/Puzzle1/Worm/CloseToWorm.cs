using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DigitalRuby.Tween;

public class CloseToWorm : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    public Animator wormAnimator;
    public bool DEBUG = true;
    public bool hasPlayedOnce = false;

    private void OnTriggerEnter(Collider collision)
    {
        if (DEBUG || !hasPlayedOnce)
        {
            Debug.Log("Close to worm");

            // Start Screen shake
            CinemachineBasicMultiChannelPerlin noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            System.Action<ITween<float>> BandInCallBack = (t) =>
            {
                noise.m_AmplitudeGain = t.CurrentValue;
            };
            // // completion defaults to null if not passed in
            gameObject.Tween("Start camera shake", 1.0f, .5f, 2.0f, TweenScaleFunctions.CubicEaseInOut, BandInCallBack);
            hasPlayedOnce = true;

            // Start worm animation
            wormAnimator.SetBool("isBiting", true);
        }
    }

    private void OnTriggerExit(Collider collision)
    {

        // Stop Screen shake
        CinemachineBasicMultiChannelPerlin noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        System.Action<ITween<float>> BandInCallBack = (t) =>
        {
            noise.m_AmplitudeGain = t.CurrentValue;
        };
        gameObject.Tween("Start camera shake", 1.0f, 0.0f, 2.0f, TweenScaleFunctions.CubicEaseInOut, BandInCallBack);
        hasPlayedOnce = false;

        // Start worm animation
        wormAnimator.SetBool("isBiting", false);
    }
}
