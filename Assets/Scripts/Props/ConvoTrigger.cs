using System;
using Abyss.EventSystem;
using Tuples;
using UnityEngine;

public class ConvoTrigger : MonoBehaviour
{
    [SerializeField] Conversation conversation;
    [SerializeField][Tooltip("Conditions to trigger this convo")] EventCondChecker condChecker; // TODO: Expand to include ors and parantheses
    [SerializeField] bool noRepeat = false;
    [SerializeField][Tooltip("Tail boolean refers to whether to record the event")] Pair<DynamicEvent, bool>[] eventsToTriggerAndRecord;
    [SerializeField] Pair<Item, int>[] itemsToGive, itemsToRemove;

    [SerializeField] float cooldownTime = 5f;
    float _lastTriggerTime = -Mathf.Infinity;

    bool _triggered = false;
    protected GameObject player;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && CheckConditionsMet())
        {
            player = other.gameObject;
            Execute();
            _lastTriggerTime = Time.time;
        }
    }

    protected virtual void Execute()
    {
        DialogueManager.Instance.HardStartConvo(conversation);
        _triggered = true;
        foreach (var evt in eventsToTriggerAndRecord)
        {
            EventManager.InvokeEvent(new GameEvent(evt.Head.EventName));
            if (evt.Tail) EventLedger.Instance.Record(new GameEvent(evt.Head.EventName));
        }

        foreach (var items in itemsToGive)
            GameManager.Instance.Inventory.MaterialCollection.Add(items.Head, items.Tail);

        foreach (var items in itemsToRemove)
            GameManager.Instance.Inventory.MaterialCollection.RemoveStock(items.Head, items.Tail);
    }

    protected bool CheckConditionsMet()
    {
        if (noRepeat && _triggered) return false;
        if (Time.time - _lastTriggerTime < cooldownTime) return false;
        return condChecker.IsMet();
    }
}

[Serializable]
public class EventCondChecker
{
    [SerializeField] bool invert = false;
    [SerializeField][Tooltip("Dyanamic events (none core) that must have either occurred or not occurred (and)")] Pair<DynamicEvent, bool>[] dynamicEventConditions; // TODO: Expand to include ors and parantheses
    [SerializeField][Tooltip("Static core events that must have either occurred or not occurred (and)")] Pair<StaticEvent, bool>[] staticEventConditions;
    [SerializeField][Tooltip("Static core events that must have occured exactly n times (and)")] Pair<StaticEvent, int>[] staticEventCountConditions;
    [SerializeField][Tooltip("Dynamic events (none core) that must have occured exactly n times (and)")] Pair<DynamicEvent, int>[] dynamicEventCountConditions;

    public bool IsMet()
    {
        foreach (var eventCond in dynamicEventConditions)
        {
            bool hasOccurred = EventLedger.Instance.HasOccurred(new GameEvent(eventCond.Head.EventName));
            if ((hasOccurred && !eventCond.Tail) || (!hasOccurred && eventCond.Tail)) return invert;
        }
        foreach (var eventCond in staticEventConditions)
        {
            bool hasOccurred = EventLedger.Instance.HasOccurred(eventCond.Head);
            if ((hasOccurred && !eventCond.Tail) || (!hasOccurred && eventCond.Tail)) return invert;
        }
        foreach (var eventCountCond in dynamicEventCountConditions)
        {
            int count = EventLedger.Instance.GetEventCount(new GameEvent(eventCountCond.Head.EventName));
            if (count != eventCountCond.Tail) return invert;
        }
        foreach (var eventCountCond in staticEventCountConditions)
        {
            int count = EventLedger.Instance.GetEventCount(eventCountCond.Head);
            if (count != eventCountCond.Tail) return invert;
        }
        return !invert;
    }
}