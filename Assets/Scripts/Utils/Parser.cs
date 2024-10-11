using System;
using Abyss.SceneSystem;
using Abyss.EventSystem;

namespace Abyss.Utils
{
    class Parser
    {
        public static StaticEvent GetStaticEventFromText(string text)
        {
            if (Enum.TryParse(text, out StaticEvent parsedEvent))
                return parsedEvent;
            else return StaticEvent.NoEvent;
        }

        public static AbyssScene GetSceneFromText(string text)
        {
            if (Enum.TryParse(text, out AbyssScene parsedScene))
                return parsedScene;
            else return AbyssScene.None;
        }
    }
}