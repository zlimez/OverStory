using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Abyss.EventSystem;

public class Curtain : MonoBehaviour
{
    public Image blackCurtain;
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 1f;
    [SerializeField] private float closedInterval = 1.5f;
    private float timeElapsed = 0;
    [SerializeField] private GameEvent[] closeTriggers, openTriggers, closeOpenTriggers;

    void OnEnable()
    {
        foreach (GameEvent closeTrigger in closeTriggers)
        {
            EventManager.StartListening(closeTrigger, Close);
        }

        foreach (GameEvent openTrigger in openTriggers)
        {
            EventManager.StartListening(openTrigger, Open);
        }

        foreach (GameEvent closeOpenTrigger in closeOpenTriggers)
        {
            EventManager.StartListening(closeOpenTrigger, CloseAndOpen);
        }
        // Every time there is teleportation draw the curtain
        EventManager.StartListening(UIEventCollection.PrepToTeleport, CloseAndOpen);
    }

    void OnDisable()
    {
        foreach (GameEvent closeTrigger in closeTriggers)
        {
            EventManager.StopListening(closeTrigger, Close);
        }

        foreach (GameEvent openTrigger in openTriggers)
        {
            EventManager.StopListening(openTrigger, Open);
        }

        foreach (GameEvent closeOpenTrigger in closeOpenTriggers)
        {
            EventManager.StopListening(closeOpenTrigger, CloseAndOpen);
        }

        EventManager.StopListening(UIEventCollection.PrepToTeleport, CloseAndOpen);
    }

    public void Close(object input = null)
    {
        StartCoroutine(Darken());
    }

    public void Open(object input = null)
    {
        StartCoroutine(Lighten());
    }

    public void CloseAndOpen(object input = null)
    {
        StartCoroutine(DarkenThenLighten());
    }

    IEnumerator Darken()
    {
        Debug.Log("Closing curtain");
        timeElapsed = 0;
        while (timeElapsed < fadeInDuration)
        {
            timeElapsed += Time.unscaledDeltaTime;
            blackCurtain.color = Color.LerpUnclamped(ColorUtils.transparent, Color.black, timeElapsed / fadeInDuration);
            yield return null;
        }
        EventManager.InvokeEvent(UIEventCollection.CurtainDrawn);
    }

    IEnumerator Lighten()
    {
        Debug.Log("Opening curtain");
        timeElapsed = 0;
        while (timeElapsed < fadeOutDuration)
        {
            timeElapsed += Time.unscaledDeltaTime;
            blackCurtain.color = Color.LerpUnclamped(Color.black, ColorUtils.transparent, timeElapsed / fadeOutDuration);
            yield return null;
        }

        EventManager.InvokeEvent(UIEventCollection.CurtainOpen);
    }

    IEnumerator DarkenThenLighten()
    {
        timeElapsed = 0;
        while (timeElapsed < fadeInDuration)
        {
            timeElapsed += Time.unscaledDeltaTime;
            blackCurtain.color = Color.LerpUnclamped(ColorUtils.transparent, Color.black, timeElapsed / fadeInDuration);
            yield return null;
        }
        EventManager.InvokeEvent(UIEventCollection.CurtainDrawn);
        timeElapsed = 0;
        while (timeElapsed < closedInterval)
        {
            timeElapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        timeElapsed = 0;
        while (timeElapsed < fadeOutDuration)
        {
            timeElapsed += Time.unscaledDeltaTime;
            blackCurtain.color = Color.LerpUnclamped(Color.black, ColorUtils.transparent, timeElapsed / fadeOutDuration);
            yield return null;
        }

        EventManager.InvokeEvent(UIEventCollection.CurtainOpen);
    }
}
