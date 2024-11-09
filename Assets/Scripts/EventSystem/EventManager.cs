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
        private static readonly Dictionary<GameEvent, Action<object>> eventTable = new();
        private static readonly Queue<GameEvent> sceneTransitionQueuedEvents = new();

        public static void StartListening(GameEvent gameEvent, Action<object> listener)
        {
            if (eventTable.ContainsKey(gameEvent))
                eventTable[gameEvent] += listener;
            else eventTable.Add(gameEvent, listener);
        }
        public static void StartListening(StaticEvent gameEvent, Action<object> listener) => StartListening(new GameEvent(gameEvent.ToString()), listener);

        public static void StopListening(GameEvent gameEvent, Action<object> listener)
        {
            if (eventTable.ContainsKey(gameEvent))
            {
                eventTable[gameEvent] -= listener;

                if (eventTable[gameEvent] == null)
                    eventTable.Remove(gameEvent);
            }
        }
        public static void StopListening(StaticEvent gameEvent, Action<object> listener) => StopListening(new GameEvent(gameEvent.ToString()), listener);

        public static void StopListeningAll(GameEvent gameEvent)
        {
            if (eventTable.ContainsKey(gameEvent))
                eventTable.Remove(gameEvent);
        }
        public static void StopListeningAll(StaticEvent gameEvent) => StopListeningAll(new GameEvent(gameEvent.ToString()));

        public static void QueueEvent(GameEvent gameEvent) => sceneTransitionQueuedEvents.Enqueue(gameEvent);
        public static void QueueEvent(StaticEvent gameEvent) => QueueEvent(new GameEvent(gameEvent.ToString()));

        public static void InvokeQueueEvents()
        {
            foreach (GameEvent gameEvent in sceneTransitionQueuedEvents)
            {
                Debug.Log($"Queued event {gameEvent.EventName} invoked");
                InvokeEvent(gameEvent);
            }
            sceneTransitionQueuedEvents.Clear();
        }

        public static void InvokeEvent(GameEvent gameEvent, object inputParam = null)
        {
            Debug.Log($"{gameEvent.EventName} invoked");
            if (eventTable.ContainsKey(gameEvent))
                eventTable[gameEvent]?.Invoke(inputParam);
        }

        public static void InvokeEvent(StaticEvent gameEvent, object inputParam = null) => InvokeEvent(new GameEvent(gameEvent.ToString()), inputParam);
    }
}