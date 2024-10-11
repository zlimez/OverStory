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

        /// <summary>
        /// Starts listening to the specified game event and adds a listener to it.
        /// </summary>
        /// <param name="gameEvent">The game event to listen to.</param>
        /// <param name="listener">The listener to add.</param>
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

        public static void StartListening(StaticEvent gameEvent, UnityAction<object> listener)
        {
            StartListening(new GameEvent(gameEvent.ToString()), listener);
        }

        /// <summary>
        /// Stops listening to the specified game event and removes a single specified listener from it.
        /// </summary>
        /// <param name="gameEvent">The game event to stop listening to.</param>
        /// <param name="listener">The listener to remove.</param>
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

        public static void StopListening(StaticEvent gameEvent, UnityAction<object> listener)
        {
            StopListening(new GameEvent(gameEvent.ToString()), listener);
        }

        /// <summary>
        /// Removes all listeners from the specified game event.
        /// </summary>
        /// <param name="gameEvent">The game event to remove listeners from.</param>
        public static void StopListeningAll(GameEvent gameEvent)
        {
            if (eventTable.TryGetValue(gameEvent, out UnityEvent<object> thisEvent))
                thisEvent.RemoveAllListeners();
        }

        public static void StopListeningAll(StaticEvent gameEvent)
        {
            StopListeningAll(new GameEvent(gameEvent.ToString()));
        }

        /// <summary>
        /// Queues the specified game event.
        /// </summary>
        /// <param name="gameEvent">The game event to queue.</param>
        public static void QueueEvent(GameEvent gameEvent)
        {
            sceneTransitionQueuedEvents.Enqueue(gameEvent);
        }

        public static void QueueEvent(StaticEvent gameEvent)
        {
            QueueEvent(new GameEvent(gameEvent.ToString()));
        }

        /// <summary>
        /// Invokes all queued game events.
        /// </summary>
        public static void InvokeQueueEvents()
        {
            foreach (GameEvent gameEvent in sceneTransitionQueuedEvents)
            {
                Debug.Log($"Queued event {gameEvent.EventName} invoked");
                InvokeEvent(gameEvent);
            }
            sceneTransitionQueuedEvents.Clear();
        }

        /// <summary>
        /// Invokes the specified game event.
        /// </summary>
        /// <param name="gameEvent">The game event to invoke.</param>
        /// <param name="inputParam">Optional input parameter for the event.</param>
        public static void InvokeEvent(GameEvent gameEvent, object inputParam = null)
        {
            Debug.Log($"{gameEvent.EventName} invoked");
            if (eventTable.TryGetValue(gameEvent, out UnityEvent<object> thisEvent))
                thisEvent.Invoke(inputParam);
        }

        public static void InvokeEvent(StaticEvent gameEvent, object inputParam = null)
        {
            InvokeEvent(new GameEvent(gameEvent.ToString()), inputParam);
        }
    }
}