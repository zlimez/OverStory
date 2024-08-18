using System;
using UnityEngine;
using Chronellium.SceneSystem;
using Chronellium.EventSystem;

namespace Chronellium.Utils
{
    class Parser
    {
        public static StaticEvent getStaticEventFromText(String text)
        {
            if (Enum.TryParse(text, out StaticEvent parsedEvent))
            {
                return parsedEvent;
            }
            else
            {
                Debug.Log("Invalid event text: " + text);
                return StaticEvent.NoEvent;
            }
        }

        public static ChronelliumScene getSceneFromText(String text)
        {
            if (Enum.TryParse(text, out ChronelliumScene parsedScene))
            {
                return parsedScene;
            }
            else
            {
                Debug.Log("Invalid scene text: " + text);
                return ChronelliumScene.None;
            }
        }
    }
}