using System.Collections;
using Abyss.EventSystem;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] GameEvent triggerOpenEvent, triggerCloseEvent;
    [SerializeField] bool isClosed = true;
    [SerializeField] AnimationCurve curve;
    [SerializeField] Transform closePos, openPos;

    void OnEnable()
    {
        if (triggerOpenEvent.EventName != "") EventManager.StartListening(triggerOpenEvent, Open);
        if (triggerCloseEvent.EventName != "") EventManager.StartListening(triggerCloseEvent, Close);
    }

    void OnDisable()
    {
        if (triggerOpenEvent.EventName != "") EventManager.StopListening(triggerOpenEvent, Open);
        if (triggerCloseEvent.EventName != "") EventManager.StopListening(triggerCloseEvent, Close);
    }

    void Open(object obj = null)
    {
        if (!isClosed) return;
        StartCoroutine(OpenRoutine());
    }

    void Close(object obj = null)
    {
        if (isClosed) return;
        StartCoroutine(CloseRoutine());
    }

    IEnumerator OpenRoutine()
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

    IEnumerator CloseRoutine()
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
