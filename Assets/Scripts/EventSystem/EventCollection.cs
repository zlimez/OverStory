namespace Abyss.EventSystem
{
    public static class SystemEventCollection
    {
        public static readonly StaticEvent GameManagerReady;
        public static readonly StaticEvent TransitionWithMaster;
        public static readonly StaticEvent TransitionWithMasterCompleted;
        public static readonly StaticEvent Transition;


        static SystemEventCollection()
        {
            GameManagerReady = StaticEvent.GameManagerReady;
            TransitionWithMaster = StaticEvent.SceneTransitWithMaster;
            TransitionWithMasterCompleted = StaticEvent.SceneTransitWithMasterDone;
            Transition = StaticEvent.SceneTransition;
        }
    }

    public static class UIEventCollection
    {
        public static readonly StaticEvent CurtainDrawn;
        public static readonly StaticEvent CurtainOpen;
        public static readonly StaticEvent PrepToTeleport;
        public static readonly StaticEvent OpenInventory;
        public static readonly StaticEvent DialogStarted;

        static UIEventCollection()
        {
            CurtainDrawn = StaticEvent.CurtainDrawn;
            PrepToTeleport = StaticEvent.PrepToTeleport;
            OpenInventory = StaticEvent.OpenInventory;
            CurtainOpen = StaticEvent.CurtainOpen;
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