using Abyss.EventSystem;
using Tuples;
using UnityEngine;

public class EventsTrigger : MonoBehaviour
{
    [SerializeField][Tooltip("Conditions to trigger this convo")] EventCondChecker condChecker;
    [SerializeField][Tooltip("Tail boolean refers to whether to record the event")] Pair<DynamicEvent, bool>[] eventsToTriggerAndRecord;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && condChecker.IsMet())
        {
            foreach (var evt in eventsToTriggerAndRecord)
            {
                EventManager.InvokeEvent(new GameEvent(evt.Head.EventName));
                if (evt.Tail) EventLedger.Instance.Record(new GameEvent(evt.Head.EventName));
            }
        }
    }
}
