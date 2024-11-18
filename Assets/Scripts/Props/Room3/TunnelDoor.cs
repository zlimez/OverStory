using System.Collections;
using Abyss.EventSystem;
using Abyss.Interactables;
using UnityEngine;

public class TunnelDoor : Interactable
{
    [SerializeField] Transform openPos, doorObj;
    [SerializeField] AnimationCurve curve;
    [SerializeField] DynamicEvent tunnelDoorOpenedEvent;
    [SerializeField] float timeToOpen = 2f;

    bool _isOpen = false;

    void OnEnable()
    {
        if (EventLedger.Instance == null)
            EventManager.StartListening(SystemEvents.LedgerReady, Load);
        else Load();
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if (!_isOpen) base.OnTriggerEnter2D(collider);
    }

    public override void Interact()
    {
        base.Interact();
        StartCoroutine(OpenRoutine());
    }

    IEnumerator OpenRoutine()
    {
        float t = 0;
        var startPos = doorObj.position;
        while (t < timeToOpen)
        {
            t += Time.deltaTime;
            doorObj.position = Vector3.Lerp(startPos, openPos.position, curve.Evaluate(t / timeToOpen));
            yield return null;
        }
        doorObj.position = openPos.position;
    }

    void Load(object input = null)
    {
        if (EventLedger.Instance.HasOccurred(new GameEvent(tunnelDoorOpenedEvent.EventName)))
        {
            _isOpen = true;
            doorObj.position = openPos.position;
        }
    }

    void OnDisable() => EventManager.StopListening(SystemEvents.LedgerReady, Load);
}
