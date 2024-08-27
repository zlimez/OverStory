using System.Collections;
using DigitalRuby.Tween;
using Abyss.EventSystem;
using UnityEngine;

public class HintShow : MonoBehaviour
{
    public SpriteRenderer buttonPressHint;
    private bool hasSeenHint;

    void Start()
    {
        if (buttonPressHint != null)
        {
            // Make Hint invsible at the start
            Color tmp = buttonPressHint.color;
            tmp.a = 0f;
            buttonPressHint.color = tmp;
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (hasSeenHint || buttonPressHint == null) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            System.Action<ITween<float>> BandInCallBack = (t) =>
            {
                Color tmp = buttonPressHint.color;
                tmp.a = t.CurrentValue;
                buttonPressHint.color = tmp;
            };

            // completion defaults to null if not passed in
            gameObject.Tween("FadeIn", buttonPressHint.color.a, 1.0f, 1.0f, TweenScaleFunctions.CubicEaseInOut, BandInCallBack);
        }

    }
    private void OnTriggerExit(Collider collision)
    {
        if (hasSeenHint || buttonPressHint == null) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            System.Action<ITween<float>> BandInCallBack = (t) =>
            {
                Color tmp = buttonPressHint.color;
                tmp.a = t.CurrentValue;
                buttonPressHint.color = tmp;
            };

            // completion defaults to null if not passed in
            gameObject.Tween("FadeIn", buttonPressHint.color.a, 0.0f, 1.0f, TweenScaleFunctions.CubicEaseInOut, BandInCallBack);
        }
        // hasSeenHint = true;
    }

}
