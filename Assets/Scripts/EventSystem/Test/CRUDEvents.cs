using Abyss.EventSystem;
using Tuples;
using UnityEngine;

public class CRUDEvents : MonoBehaviour
{
    [SerializeField] DynamicEvent[] eventsToInvoke;
    [SerializeField] DynamicEvent[] eventsToRecord, eventsToRemove;

    public void Record()
    {
        foreach (var evt in eventsToRecord) EventLedger.Instance.Record(new GameEvent(evt.EventName));
    }

    public void Remove()
    {
        foreach (var evt in eventsToRemove) EventLedger.Instance.Remove(new GameEvent(evt.EventName));
    }

    public void Invoke()
    {
        foreach (var evt in eventsToInvoke) EventManager.InvokeEvent(new GameEvent(evt.EventName));
    }
}
