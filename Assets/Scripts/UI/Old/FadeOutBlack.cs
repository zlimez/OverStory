using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.Tween;
using UnityEngine.UI;

public class FadeOutBlack : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        System.Action<ITween<float>> BandInCallBack = (t) =>
        {
            GetComponent<Image>().color = new Color(0, 0, 0, t.CurrentValue);
        };

        // completion defaults to null if not passed in
        gameObject.Tween("BandIn", 1.0f, 0.0f, 2.0f, TweenScaleFunctions.CubicEaseInOut, BandInCallBack);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
