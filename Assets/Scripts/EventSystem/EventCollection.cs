namespace Abyss.EventSystem
{
    public static class SystemEventCollection
    {
        public static readonly StaticEvent SystemsReady;
        public static readonly StaticEvent SceneTransitStart;
        public static readonly StaticEvent SceneTransitDone;
        public static readonly StaticEvent SceneTransitPrep;


        static SystemEventCollection()
        {
            SystemsReady = StaticEvent.SystemsReady;
            SceneTransitStart = StaticEvent.SceneTransitStart;
            SceneTransitDone = StaticEvent.SceneTransitDone;
            SceneTransitPrep = StaticEvent.SceneTransitPrep;
        }
    }

    public static class UIEventCollection
    {
        public static readonly StaticEvent BlackIn;
        public static readonly StaticEvent BlackOut;
        public static readonly StaticEvent PrepToTeleport;
        public static readonly StaticEvent OpenInventory;
        public static readonly StaticEvent DialogStarted;
        public static readonly StaticEvent DragedItem;
        public static readonly StaticEvent ChangeNPCInventory;

        static UIEventCollection()
        {
            BlackIn = StaticEvent.BlackIn;
            PrepToTeleport = StaticEvent.PrepToTeleport;
            OpenInventory = StaticEvent.OpenInventory;
            BlackOut = StaticEvent.BlackOut;
            DialogStarted = StaticEvent.DialogStarted;
            DragedItem = StaticEvent.DragedItem;
            ChangeNPCInventory = StaticEvent.ChangeNPCInventory;
        }
    }

    public static class PlayEventCollection
    {
        public static readonly StaticEvent PlayerDeath;
        public static readonly StaticEvent InteractableEntered;
        public static readonly StaticEvent InteractableExited;
        public static readonly StaticEvent PlayerHealthChange;
        public static readonly StaticEvent PlayerPurityChange;

        static PlayEventCollection()
        {
            PlayerDeath = StaticEvent.PlayerDeath;
            PlayerHealthChange = StaticEvent.PlayerHealthChange;
            PlayerPurityChange = StaticEvent.PlayerPurityChange;
            InteractableEntered = StaticEvent.InteractableEntered;
            InteractableExited = StaticEvent.InteractableExited;
        }
    }
}