using UnityEngine;

namespace Abyss.EventSystem
{
    [CreateAssetMenu(menuName = "Dynamic Event")]
    public class DynamicEvent : ScriptableObject
    {
        public string EventName;

        void OnValidate()
        {
            if (EventName == "") EventName = name;
        }
    }
}
