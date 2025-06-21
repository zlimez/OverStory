using UnityEngine;

namespace Abyss.EventSystem
{
    [CreateAssetMenu(menuName = "Event Object")]
    public class DynamicEvent : ScriptableObject
    {
        public string EventName;

        void OnValidate() { if (EventName == "") EventName = name; }
    }
}
