using System;
using System.Collections.Generic;
using Abyss.EventSystem;
using Tuples;
using UnityEngine;

/// <summary>
/// Enable or disable a game object based on an event.
/// </summary>
public class EventEnable : MonoBehaviour
{
    [SerializeField] Pair<DynamicEvent, GameObject>[] enableDEventObjectPairs;
    [SerializeField] Pair<DynamicEvent, GameObject>[] disableDEventObjectPairs;
    [SerializeField] Pair<StaticEvent, GameObject>[] enableSEventObjectPairs;
    [SerializeField] Pair<StaticEvent, GameObject>[] disableSEventObjectPairs;
    List<Pair<GameEvent, Action<object>>> _eventActions = new();

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
            EventManager.StartListening(pair.Head, pair.Tail);
    }

    void OnDisable()
    {
        foreach (var pair in _eventActions)
            EventManager.StopListening(pair.Head, pair.Tail);
    }
}
