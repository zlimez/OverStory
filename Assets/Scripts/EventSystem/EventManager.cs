using System.Collections.Generic;
using UnityEngine;
using System;

namespace Abyss.EventSystem
{
    /// <summary>
    /// Manages core events such as mode and character switch, game-wide events, save game, etc.
    /// </summary>
    public static class EventManager
    {
        private static readonly Dictionary<GameEvent, Action<object>> EventTable = new();
        private static readonly Queue<GameEvent> SceneTransitionQueuedEvents = new();

        public static void Subscribe(GameEvent gameEvent, Action<object> listener)
        {
            if (!EventTable.TryAdd(gameEvent, listener))
                EventTable[gameEvent] += listener;
        }
        public static void Subscribe(NamedEvent gameEvent, Action<object> listener) => Subscribe(new GameEvent(gameEvent.ToString()), listener);

        public static void Unsubscribe(GameEvent gameEvent, Action<object> listener)
        {
            if (!EventTable.ContainsKey(gameEvent)) return;
            EventTable[gameEvent] -= listener;

            if (EventTable[gameEvent] == null)
                EventTable.Remove(gameEvent);
        }
        public static void Unsubscribe(NamedEvent gameEvent, Action<object> listener) =>
            Unsubscribe(new GameEvent(gameEvent.ToString()), listener);

        public static void StopListeningAll(GameEvent gameEvent) => EventTable.Remove(gameEvent);

        public static void StopListeningAll(NamedEvent gameEvent) =>
            StopListeningAll(new GameEvent(gameEvent.ToString()));

        public static void QueueEvent(GameEvent gameEvent) => SceneTransitionQueuedEvents.Enqueue(gameEvent);
        public static void QueueEvent(NamedEvent gameEvent) => QueueEvent(new GameEvent(gameEvent.ToString()));

        public static void InvokeQueueEvents()
        {
            foreach (GameEvent gameEvent in SceneTransitionQueuedEvents)
            {
#if UNITY_EDITOR
                Debug.Log($"Queued event {gameEvent.EventName} invoked");
#endif
                InvokeEvent(gameEvent);
            }
            SceneTransitionQueuedEvents.Clear();
        }

        public static void InvokeEvent(GameEvent gameEvent, object inputParam = null)
        {
            // Debug.Log($"{gameEvent.EventName} invoked");
            if (EventTable.TryGetValue(gameEvent, out var action))
                action?.Invoke(inputParam);
        }

        public static void InvokeEvent(NamedEvent gameEvent, object inputParam = null) =>
            InvokeEvent(new GameEvent(gameEvent.ToString()), inputParam);
    }
}