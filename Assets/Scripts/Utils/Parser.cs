using System;
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

        public static Settings.Scene GetSceneFromText(string text)
        {
            if (Enum.TryParse(text, out Settings.Scene parsedScene))
                return parsedScene;
            else return Settings.Scene.None;
        }
    }
}