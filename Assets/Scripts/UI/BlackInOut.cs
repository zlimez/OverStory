using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Abyss.EventSystem;

public class BlackInOut : MonoBehaviour
{
    public Image blackImage;
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 1f;
    [SerializeField] private float closedInterval = 1.5f;
    private float timeElapsed = 0;
    [SerializeField] private GameEvent[] closeTriggers, openTriggers, closeOpenTriggers;
    [SerializeField] DynamicEvent[] dynCloseTriggers, dynOpenTriggers, dynCloseOpenTriggers;

    void OnEnable()
    {
        if (closeTriggers != null)
            foreach (GameEvent closeTrigger in closeTriggers)
                EventManager.StartListening(closeTrigger, Close);

        if (dynCloseTriggers != null)
            foreach (var dynCloseTrigger in dynCloseTriggers)
                EventManager.StartListening(new GameEvent(dynCloseTrigger.EventName), Close);

        if (openTriggers != null)
            foreach (GameEvent openTrigger in openTriggers)
                EventManager.StartListening(openTrigger, Open);

        if (dynOpenTriggers != null)
            foreach (var dynOpenTrigger in dynOpenTriggers)
                EventManager.StartListening(new GameEvent(dynOpenTrigger.EventName), Open);

        if (closeOpenTriggers != null)
            foreach (GameEvent closeOpenTrigger in closeOpenTriggers)
                EventManager.StartListening(closeOpenTrigger, CloseOpen);

        if (dynCloseOpenTriggers != null)
            foreach (var dynCloseOpenTrigger in dynCloseOpenTriggers)
                EventManager.StartListening(new GameEvent(dynCloseOpenTrigger.EventName), CloseOpen);
    }

    void OnDisable()
    {
        if (closeTriggers != null)
            foreach (GameEvent closeTrigger in closeTriggers)
                EventManager.StopListening(closeTrigger, Close);

        if (dynCloseTriggers != null)
            foreach (var dynCloseTrigger in dynCloseTriggers)
                EventManager.StopListening(new GameEvent(dynCloseTrigger.EventName), Close);

        if (openTriggers != null)
            foreach (GameEvent openTrigger in openTriggers)
                EventManager.StopListening(openTrigger, Open);

        if (dynOpenTriggers != null)
            foreach (var dynOpenTrigger in dynOpenTriggers)
                EventManager.StopListening(new GameEvent(dynOpenTrigger.EventName), Open);

        if (closeOpenTriggers != null)
            foreach (GameEvent closeOpenTrigger in closeOpenTriggers)
                EventManager.StopListening(closeOpenTrigger, CloseOpen);

        if (dynCloseOpenTriggers != null)
            foreach (var dynCloseOpenTrigger in dynCloseOpenTriggers)
                EventManager.StopListening(new GameEvent(dynCloseOpenTrigger.EventName), CloseOpen);
    }

    public void Close(object input = null) => StartCoroutine(Close());
    public void Open(object input = null) => StartCoroutine(Open());
    public void CloseOpen(object input = null) => StartCoroutine(CloseOpen());

    IEnumerator Close()
    {
        timeElapsed = 0;
        while (timeElapsed < fadeInDuration)
        {
            timeElapsed += Time.unscaledDeltaTime;
            blackImage.color = Color.LerpUnclamped(Color.clear, Color.black, timeElapsed / fadeInDuration);
            yield return null;
        }
        EventManager.InvokeEvent(UIEvents.BlackIn);
    }

    IEnumerator Open()
    {
        timeElapsed = 0;
        while (timeElapsed < fadeOutDuration)
        {
            timeElapsed += Time.unscaledDeltaTime;
            blackImage.color = Color.LerpUnclamped(Color.black, Color.clear, timeElapsed / fadeOutDuration);
            yield return null;
        }

        EventManager.InvokeEvent(UIEvents.BlackOut);
    }

    IEnumerator CloseOpen()
    {
        timeElapsed = 0;
        while (timeElapsed < fadeInDuration)
        {
            timeElapsed += Time.unscaledDeltaTime;
            blackImage.color = Color.LerpUnclamped(Color.clear, Color.black, timeElapsed / fadeInDuration);
            yield return null;
        }
        EventManager.InvokeEvent(UIEvents.BlackIn);
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
            blackImage.color = Color.LerpUnclamped(Color.black, Color.clear, timeElapsed / fadeOutDuration);
            yield return null;
        }

        EventManager.InvokeEvent(UIEvents.BlackOut);
    }
}
