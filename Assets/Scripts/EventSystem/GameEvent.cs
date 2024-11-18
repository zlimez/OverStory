using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Abyss.EventSystem
{
    [Serializable]
    public struct GameEvent
    {
        public static GameEvent NoEvent { get; private set; } = new GameEvent(string.Empty);
        [Tooltip("The related static event, if any.")]
        public StaticEvent RelatedStaticEvent;

        public string EventName
        {
            readonly get { return RelatedStaticEvent != StaticEvent.NoEvent ? RelatedStaticEvent.ToString() : _eventName; }
            set { _eventName = value; }
        }
        [Tooltip("Specify this only if it is a dynamic event. Otherwise, it will be ignored.")]
        [FormerlySerializedAs("EventName")]
        [SerializeField]
        private string _eventName;

        public GameEvent(string eventName, StaticEvent relatedStaticEvent = StaticEvent.NoEvent)
        {
            if (relatedStaticEvent == StaticEvent.NoEvent)
            {
                RelatedStaticEvent = StaticEvent.NoEvent;
                _eventName = eventName;
                return;
            }

            RelatedStaticEvent = relatedStaticEvent;
            _eventName = relatedStaticEvent.ToString();
        }

        public override readonly bool Equals(object obj) => obj is GameEvent otherEvent && EventName.Equals(otherEvent.EventName);
        public override readonly int GetHashCode() => EventName.GetHashCode();
        public static bool operator ==(GameEvent left, GameEvent right) => left.EventName == right.EventName;
        public static bool operator !=(GameEvent left, GameEvent right) => !(left == right);

        public override readonly string ToString()
        {
            if (RelatedStaticEvent != StaticEvent.NoEvent)
                return $"{RelatedStaticEvent} ({EventName})";
            else return EventName;
        }
    }
}