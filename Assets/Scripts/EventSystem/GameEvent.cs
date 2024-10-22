using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Abyss.EventSystem
{
    // [CreateAssetMenu(menuName = "Dynamic Event")]
    // public class DynamicEvent : ScriptableObject
    // {
    //     public string EventName;
    // }

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
                this.RelatedStaticEvent = StaticEvent.NoEvent;
                this._eventName = eventName;
                return;
            }

            this.RelatedStaticEvent = relatedStaticEvent;
            this._eventName = relatedStaticEvent.ToString();
        }

        public override readonly bool Equals(object obj)
        {
            return obj is GameEvent otherEvent && EventName.Equals(otherEvent.EventName);
        }

        public override readonly int GetHashCode()
        {
            return EventName.GetHashCode();
        }

        public static bool operator ==(GameEvent left, GameEvent right)
        {
            return left.EventName == right.EventName;
        }

        public static bool operator !=(GameEvent left, GameEvent right)
        {
            return !(left == right);
        }

        public override readonly string ToString()
        {
            if (RelatedStaticEvent != StaticEvent.NoEvent)
                return $"{RelatedStaticEvent} ({EventName})";
            else return EventName;
        }
    }
}