using System.Collections;
using Abyss.EventSystem;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private GameEvent triggerOpenEvent, triggerCloseEvent;
    [SerializeField] private bool isClosed = true;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private Transform closePos, openPos;

    private void OnEnable()
    {
        if (triggerOpenEvent.EventName != "") EventManager.Subscribe(triggerOpenEvent, Open);
        if (triggerCloseEvent.EventName != "") EventManager.Subscribe(triggerCloseEvent, Close);
    }

    private void OnDisable()
    {
        if (triggerOpenEvent.EventName != "") EventManager.Unsubscribe(triggerOpenEvent, Open);
        if (triggerCloseEvent.EventName != "") EventManager.Unsubscribe(triggerCloseEvent, Close);
    }

    private void Open(object obj = null)
    {
        if (!isClosed) return;
        StartCoroutine(OpenRoutine());
    }

    void Close(object obj = null)
    {
        if (isClosed) return;
        StartCoroutine(CloseRoutine());
    }

    private IEnumerator OpenRoutine()
    {
        isClosed = false;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(closePos.position, openPos.position, curve.Evaluate(t));
            yield return null;
        }
        transform.position = openPos.position;
    }

    private IEnumerator CloseRoutine()
    {
        isClosed = true;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(openPos.position, closePos.position, curve.Evaluate(t));
            yield return null;
        }
        transform.position = closePos.position;
    }
}
