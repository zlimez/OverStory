using System;
using Abyss.EventSystem;

namespace Abyss.Utils
{
    class Parser
    {
        public static NamedEvent GetStaticEventFromText(string text)
        {
            if (Enum.TryParse(text, out NamedEvent parsedEvent))
                return parsedEvent;
            else return NamedEvent.NoEvent;
        }

        public static Settings.Scene GetSceneFromText(string text)
        {
            if (Enum.TryParse(text, out Settings.Scene parsedScene))
                return parsedScene;
            else return Settings.Scene.None;
        }
    }
}