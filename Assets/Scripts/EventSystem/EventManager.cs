using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace Abyss.EventSystem
{
    /// <summary>
    /// Manages core events such as mode and character switch, game-wide events, save game, etc.
    /// </summary>
    public static class EventManager
    {
        private static readonly Dictionary<GameEvent, UnityEvent<object>> eventTable = new Dictionary<GameEvent, UnityEvent<object>>();
        private static readonly Dictionary<GameEvent, int> eventListenerCountTable = new Dictionary<GameEvent, int>();
        private static readonly Queue<GameEvent> sceneTransitionQueuedEvents = new Queue<GameEvent>();

        public static void StartListening(GameEvent gameEvent, UnityAction<object> listener)
        {
            if (eventTable.TryGetValue(gameEvent, out UnityEvent<object> thisEvent))
            {
                thisEvent.AddListener(listener);
                eventListenerCountTable[gameEvent]++;
            }
            else
            {
                thisEvent = new UnityEvent<object>();
                thisEvent.AddListener(listener);
                eventTable.Add(gameEvent, thisEvent);
                eventListenerCountTable.Add(gameEvent, 1);
            }
        }
        public static void StartListening(StaticEvent gameEvent, UnityAction<object> listener) => StartListening(new GameEvent(gameEvent.ToString()), listener);

        public static void StopListening(GameEvent gameEvent, UnityAction<object> listener)
        {
            if (eventTable.TryGetValue(gameEvent, out UnityEvent<object> thisEvent))
            {
                thisEvent.RemoveListener(listener);
                eventListenerCountTable[gameEvent]--;

                if (eventListenerCountTable[gameEvent] == 0)
                {
                    eventListenerCountTable.Remove(gameEvent);
                    eventTable.Remove(gameEvent);
                }
            }
        }
        public static void StopListening(StaticEvent gameEvent, UnityAction<object> listener) => StopListening(new GameEvent(gameEvent.ToString()), listener);

        public static void StopListeningAll(GameEvent gameEvent)
        {
            if (eventTable.TryGetValue(gameEvent, out UnityEvent<object> thisEvent))
                thisEvent.RemoveAllListeners();
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
            // Debug.Log($"{gameEvent.EventName} invoked");
            if (eventTable.TryGetValue(gameEvent, out UnityEvent<object> thisEvent))
                thisEvent.Invoke(inputParam);
        }

        public static void InvokeEvent(StaticEvent gameEvent, object inputParam = null) => InvokeEvent(new GameEvent(gameEvent.ToString()), inputParam);
    }
}