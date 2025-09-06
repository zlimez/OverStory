using System;
using System.Collections.Generic;
using Abyss.EventSystem;
using Utils.Tuples;
using UnityEngine;

/// <summary>
/// Enable or disable a game object based on an event.
/// </summary>
public class EventEnable : MonoBehaviour
{
    [SerializeField] private Pair<DynamicEvent, GameObject>[] enableDEventObjectPairs;
    [SerializeField] private Pair<DynamicEvent, GameObject>[] disableDEventObjectPairs;
    [SerializeField] private Pair<NamedEvent, GameObject>[] enableSEventObjectPairs;
    [SerializeField] private Pair<NamedEvent, GameObject>[] disableSEventObjectPairs;
    private readonly List<Pair<GameEvent, Action<object>>> _eventActions = new();

    void Awake()
    {
        foreach (var pair in enableDEventObjectPairs)
            _eventActions.Add(new Pair<GameEvent, Action<object>>(new GameEvent(pair.Head.EventName), (object input) => pair.Tail.SetActive(true)));

        foreach (var pair in disableDEventObjectPairs)
            _eventActions.Add(new Pair<GameEvent, Action<object>>(new GameEvent(pair.Head.EventName), (object input) => pair.Tail.SetActive(false)));

        foreach (var pair in enableSEventObjectPairs)
            _eventActions.Add(new Pair<GameEvent, Action<object>>(new GameEvent(pair.Head.ToString()), (object input) => pair.Tail.SetActive(true)));

        foreach (var pair in disableSEventObjectPairs)
            _eventActions.Add(new Pair<GameEvent, Action<object>>(new GameEvent(pair.Head.ToString()), (object input) => pair.Tail.SetActive(false)));
    }

    void OnEnable()
    {
        foreach (var pair in _eventActions)
            EventManager.Subscribe(pair.Head, pair.Tail);
    }

    void OnDisable()
    {
        foreach (var pair in _eventActions)
            EventManager.Unsubscribe(pair.Head, pair.Tail);
    }
}
