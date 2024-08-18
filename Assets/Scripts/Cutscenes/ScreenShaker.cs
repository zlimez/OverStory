using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Credits to: https://www.raziel619.com/blog/screenshake-in-unity-canvas-alternative-approach/
public class ScreenShaker : MonoBehaviour
{

    public float shakeAmount = 10f;
    private float shakeTime = 0.0f;
    private Vector3 initialPosition;
    private bool isScreenShaking = false;

    //Code adapted from Camera Vibration in Canvas Based Unity Game � Yuno's Wonderland

    // Update is called once per frame
    void Update()
    {
        if (shakeTime > 0)
        {
            this.transform.position = Random.insideUnitSphere * shakeAmount + initialPosition;
            shakeTime -= Time.deltaTime;
        }
        else if (isScreenShaking)
        {
            isScreenShaking = false;
            shakeTime = 0.0f;
            this.transform.position = initialPosition;

        }
    }

    public void ScreenShakeForTime(float time)
    {
        initialPosition = this.transform.position;
        shakeTime = time;
        isScreenShaking = true;
    }
}