namespace Abyss.EventSystem
{
    public static class SystemEventCollection
    {
        public static readonly StaticEvent SystemsReady;
        public static readonly StaticEvent SceneTransitWithMasterStart;
        public static readonly StaticEvent SceneTransitWithMasterDone;
        public static readonly StaticEvent Transition;


        static SystemEventCollection()
        {
            SystemsReady = StaticEvent.SystemsReady;
            SceneTransitWithMasterStart = StaticEvent.SceneTransitWithMasterStart;
            SceneTransitWithMasterDone = StaticEvent.SceneTransitWithMasterDone;
            Transition = StaticEvent.SceneTransition;
        }
    }

    public static class UIEventCollection
    {
        public static readonly StaticEvent BlackIn;
        public static readonly StaticEvent BlackOut;
        public static readonly StaticEvent PrepToTeleport;
        public static readonly StaticEvent OpenInventory;
        public static readonly StaticEvent DialogStarted;

        static UIEventCollection()
        {
            BlackIn = StaticEvent.BlackIn;
            PrepToTeleport = StaticEvent.PrepToTeleport;
            OpenInventory = StaticEvent.OpenInventory;
            BlackOut = StaticEvent.BlackOut;
            DialogStarted = StaticEvent.DialogStarted;
        }
    }

    public static class PlayEventCollection
    {
        public static readonly StaticEvent PlayerDeath;
        public static readonly StaticEvent InteractableEntered;
        public static readonly StaticEvent InteractableExited;
        public static readonly StaticEvent PlayerHealthChange;

        static PlayEventCollection()
        {
            PlayerDeath = StaticEvent.PlayerDeath;
            PlayerHealthChange = StaticEvent.PlayerHealthChange;
            InteractableEntered = StaticEvent.InteractableEntered;
            InteractableExited = StaticEvent.InteractableExited;
        }
    }
}