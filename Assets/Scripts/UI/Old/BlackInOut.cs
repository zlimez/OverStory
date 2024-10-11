using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Abyss.EventSystem;

public class BlackInOut : MonoBehaviour
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
            EventManager.StartListening(closeTrigger, Close);

        foreach (GameEvent openTrigger in openTriggers)
            EventManager.StartListening(openTrigger, Open);

        foreach (GameEvent closeOpenTrigger in closeOpenTriggers)
            EventManager.StartListening(closeOpenTrigger, CloseOpen);
    }

    void OnDisable()
    {
        foreach (GameEvent closeTrigger in closeTriggers)
            EventManager.StopListening(closeTrigger, Close);

        foreach (GameEvent openTrigger in openTriggers)
            EventManager.StopListening(openTrigger, Open);

        foreach (GameEvent closeOpenTrigger in closeOpenTriggers)
            EventManager.StopListening(closeOpenTrigger, CloseOpen);
    }

    public void Close(object input = null)
    {
        StartCoroutine(Close());
    }

    public void Open(object input = null)
    {
        StartCoroutine(Open());
    }

    public void CloseOpen(object input = null)
    {
        StartCoroutine(CloseOpen());
    }

    IEnumerator Close()
    {
        timeElapsed = 0;
        while (timeElapsed < fadeInDuration)
        {
            timeElapsed += Time.unscaledDeltaTime;
            blackCurtain.color = Color.LerpUnclamped(ColorUtils.transparent, Color.black, timeElapsed / fadeInDuration);
            yield return null;
        }
        EventManager.InvokeEvent(UIEventCollection.BlackIn);
    }

    IEnumerator Open()
    {
        timeElapsed = 0;
        while (timeElapsed < fadeOutDuration)
        {
            timeElapsed += Time.unscaledDeltaTime;
            blackCurtain.color = Color.LerpUnclamped(Color.black, ColorUtils.transparent, timeElapsed / fadeOutDuration);
            yield return null;
        }

        EventManager.InvokeEvent(UIEventCollection.BlackOut);
    }

    IEnumerator CloseOpen()
    {
        timeElapsed = 0;
        while (timeElapsed < fadeInDuration)
        {
            timeElapsed += Time.unscaledDeltaTime;
            blackCurtain.color = Color.LerpUnclamped(ColorUtils.transparent, Color.black, timeElapsed / fadeInDuration);
            yield return null;
        }
        EventManager.InvokeEvent(UIEventCollection.BlackIn);
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

        EventManager.InvokeEvent(UIEventCollection.BlackOut);
    }
}
