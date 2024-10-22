using Abyss.EventSystem;
using Tuples;
using UnityEngine;

public class ConvoTrigger : MonoBehaviour
{
    [SerializeField] Conversation conversation;
    [SerializeField][Tooltip("Events that must have either occurred or not occurred to trigger the conversation (and)")] Pair<DynamicEvent, bool>[] eventConditions; // TODO: Expand to include ors and parantheses
    [SerializeField] bool noRepeat = false;
    bool _triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((noRepeat && !_triggered) || !noRepeat) && other.CompareTag("Player") && CheckEventConditionsMet())
        {
            DialogueManager.Instance.StartConvo(conversation);
            _triggered = true;
        }
    }

    bool CheckEventConditionsMet()
    {
        foreach (var eventCond in eventConditions)
        {
            bool hasOccurred = EventLedger.Instance.HasOccurred(new GameEvent(eventCond.Head.EventName));
            if ((hasOccurred && !eventCond.Tail) || (!hasOccurred && eventCond.Tail)) return false;
        }
        return true;
    }
}
