using System;
using Abyss.EventSystem;
using Tuples;
using UnityEngine;

public class ConvoTrigger : MonoBehaviour
{
    [SerializeField] Conversation conversation;
    [SerializeField][Tooltip("Conditions to trigger this convo")] EventCondChecker condChecker; // TODO: Expand to include ors and parantheses
    [SerializeField] bool noRepeat = false;
    [SerializeField] DynamicEvent[] eventsToTrigger;
    bool _triggered = false;
    protected GameObject player;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && CheckConditionsMet())
        {
            player = other.gameObject;
            Execute();
        }
    }

    protected virtual void Execute()
    {
        DialogueManager.Instance.HardStartConvo(conversation);
        _triggered = true;
        foreach (var evt in eventsToTrigger)
            EventManager.InvokeEvent(new GameEvent(evt.EventName));
    }

    protected bool CheckConditionsMet()
    {
        if (noRepeat && _triggered) return false;
        return condChecker.IsMet();
    }
}

[Serializable]
public class EventCondChecker
{
    [SerializeField][Tooltip("Dyanamic events (none core) that must have either occurred or not occurred (and)")] Pair<DynamicEvent, bool>[] dynamicEventConditions; // TODO: Expand to include ors and parantheses
    [SerializeField][Tooltip("Static core events that must have either occurred or not occurred (and)")] Pair<StaticEvent, bool>[] staticEventConditions;
    [SerializeField][Tooltip("Static core events that must have occured exactly n times (and)")] Pair<StaticEvent, int>[] staticEventCountConditions;
    [SerializeField][Tooltip("Dynamic events (none core) that must have occured exactly n times (and)")] Pair<DynamicEvent, int>[] dynamicEventCountConditions;

    public bool IsMet()
    {
        foreach (var eventCond in dynamicEventConditions)
        {
            bool hasOccurred = EventLedger.Instance.HasOccurred(new GameEvent(eventCond.Head.EventName));
            if ((hasOccurred && !eventCond.Tail) || (!hasOccurred && eventCond.Tail)) return false;
        }
        foreach (var eventCond in staticEventConditions)
        {
            bool hasOccurred = EventLedger.Instance.HasOccurred(eventCond.Head);
            if ((hasOccurred && !eventCond.Tail) || (!hasOccurred && eventCond.Tail)) return false;
        }
        foreach (var eventCountCond in dynamicEventCountConditions)
        {
            int count = EventLedger.Instance.GetEventCount(new GameEvent(eventCountCond.Head.EventName));
            if (count != eventCountCond.Tail) return false;
        }
        foreach (var eventCountCond in staticEventCountConditions)
        {
            int count = EventLedger.Instance.GetEventCount(eventCountCond.Head);
            if (count != eventCountCond.Tail) return false;
        }
        return true;
    }
}