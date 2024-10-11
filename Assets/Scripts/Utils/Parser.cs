using System;
using UnityEngine;
using Abyss.SceneSystem;
using Abyss.EventSystem;

namespace Abyss.Utils
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

        public static AbyssScene getSceneFromText(String text)
        {
            if (Enum.TryParse(text, out AbyssScene parsedScene))
            {
                return parsedScene;
            }
            else
            {
                Debug.Log("Invalid scene text: " + text);
                return AbyssScene.None;
            }
        }
    }
}