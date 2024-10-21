using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Abyss.Utils;
using DataStructures;

namespace Abyss.EventSystem
{
    /// <summary> Stores a list of events that have occurred thus far for the flattened time travel inclusive timeline and the commonly defined past. </summary>
    public class EventLedger : StaticInstance<EventLedger>
    {
        public Dictionary<GameEvent, int> PastEvents { get; private set; }
        public Dictionary<GameEvent, int> EventRecencyTable { get; private set; }
        public int Counter { get; private set; }

        private RQueue<GameEvent> recentEvents;
        [SerializeField] int RecentEventsSize = 5;

        protected override void Awake()
        {
            base.Awake();
            PastEvents = new Dictionary<GameEvent, int>();
            EventRecencyTable = new Dictionary<GameEvent, int>();
            recentEvents = new RQueue<GameEvent>();
            Counter = 0;
        }

        public void ClearRecentEventCache() => recentEvents.RemoveAll();

        public bool IsMostRecent(GameEvent gameEvent)
        {
            GameEvent latestEvent = recentEvents.Last();
            return latestEvent != GameEvent.NoEvent && latestEvent == gameEvent;
        }

        public bool IsMostRecent(StaticEvent gameEvent) => IsMostRecent(new GameEvent(gameEvent.ToString()));

        public bool IsRecent(GameEvent gameEvent) => recentEvents.Contains(gameEvent);
        public bool IsRecent(StaticEvent gameEvent) => IsRecent(new GameEvent(gameEvent.ToString()));

        public void AddToRecent(GameEvent gameEvent)
        {
            recentEvents.Enqueue(gameEvent);
            if (recentEvents.Count() > RecentEventsSize)
                recentEvents.Dequeue();
        }
        public void AddToRecent(StaticEvent gameEvent) => AddToRecent(new GameEvent(gameEvent.ToString()));

        /// <summary> Returns the most recent event from the given list of events </summary>
        public GameEvent GetMostRecentEvent(params GameEvent[] events)
        {
            GameEvent mostRecentEvent = GameEvent.NoEvent;
            int mostRecentTime = -1;
            foreach (GameEvent gameEvent in events)
            {
                if (EventRecencyTable.ContainsKey(gameEvent) && EventRecencyTable[gameEvent] > mostRecentTime)
                {
                    mostRecentEvent = gameEvent;
                    mostRecentTime = EventRecencyTable[gameEvent];
                }
            }
            return mostRecentEvent;
        }

        public StaticEvent GetMostRecentEvent(params StaticEvent[] events) => Parser.GetStaticEventFromText(GetMostRecentEvent(events.Select(s => new GameEvent(s.ToString())).ToArray()).EventName);

        public void RecordEvent(GameEvent gameEvent, bool isSilent = true)
        {
            IncEventCount(PastEvents, gameEvent);

            AddToRecent(gameEvent);
            EventRecencyTable[gameEvent] = Counter;
            Counter++;

            if (!isSilent)
                EventManager.InvokeEvent(gameEvent);
        }

        public void RecordEvent(StaticEvent gameEvent, bool isSilent = false) => RecordEvent(new GameEvent(gameEvent.ToString()), isSilent);

        public void RemoveEvent(GameEvent gameEvent)
        {
            PastEvents.Remove(gameEvent);
            EventRecencyTable.Remove(gameEvent);
            recentEvents.Remove(gameEvent);
        }

        public void RemoveEvent(StaticEvent gameEvent) => RemoveEvent(new GameEvent(gameEvent.ToString()));

        public int GetEventCount(GameEvent gameEvent) => PastEvents.ContainsKey(gameEvent) ? PastEvents[gameEvent] : 0;
        public int GetEventCount(StaticEvent gameEvent) => this.GetEventCount(new GameEvent(gameEvent.ToString()));

        public bool HasOccurred(GameEvent gameEvent) => this.GetEventCount(gameEvent) > 0;
        public bool HasOccurred(StaticEvent gameEvent) => HasOccurred(new GameEvent(gameEvent.ToString()));


        /// <summary>
        /// Increments the occurrence count of a game event in the given dictionary.
        /// </summary>
        /// <param name="eventDict">The dictionary containing the event count.</param>
        /// <param name="gameEvent">The game event to increment.</param>
        private void IncEventCount(Dictionary<GameEvent, int> eventDict, GameEvent gameEvent)
        {
            if (eventDict.ContainsKey(gameEvent))
                eventDict[gameEvent]++;
            else eventDict.Add(gameEvent, 1);
        }

        public void IncEventCount(Dictionary<StaticEvent, int> eventDict, StaticEvent gameEvent) => IncEventCount(eventDict.ToDictionary(kvp => new GameEvent(kvp.Key.ToString()), kvp => kvp.Value), new GameEvent(gameEvent.ToString()));
    }
}