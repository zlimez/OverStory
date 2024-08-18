using System.Collections;
using System.Collections.Generic;
using DigitalRuby.Tween;
using UnityEngine;

public class SpriteFadeinOnEnable : MonoBehaviour
{
    public float maxOpacity;
    // Start is called before the first frame update
    void OnEnable()
    {
        System.Action<ITween<float>> BandInCallBack = (t) =>
     {
         Color curColor = GetComponent<SpriteRenderer>().color;
         curColor.a = t.CurrentValue;
         GetComponent<SpriteRenderer>().color = curColor;
     };

        // completion defaults to null if not passed in
        gameObject.Tween("BandOut", 0.0f, maxOpacity, 2.0f, TweenScaleFunctions.CubicEaseInOut, BandInCallBack);
    }

}