using UnityEngine;
using System.Collections.Generic;

namespace Abyss.EventSystem
{
    public class EventTestSuite : MonoBehaviour
    {
        public List<bool> SelectedStaticEvents = new List<bool>();
        public List<string> CustomGameEvents = new List<string>();

        private void Awake()
        {
            for (int i = 0; i < System.Enum.GetValues(typeof(StaticEvent)).Length; i++)
            {
                SelectedStaticEvents.Add(false);
            }
        }
    }
}