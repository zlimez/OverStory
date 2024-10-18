namespace Abyss.EventSystem
{
    public static class SystemEvents
    {
        public static readonly StaticEvent SystemsReady;
        public static readonly StaticEvent SceneTransitStart;
        public static readonly StaticEvent SceneTransitDone;
        public static readonly StaticEvent SceneTransitPrep;
        public static readonly StaticEvent TimeBcastEvent;
        public static readonly StaticEvent EnemyPopManagerReady;


        static SystemEvents()
        {
            SystemsReady = StaticEvent.SystemsReady;
            SceneTransitStart = StaticEvent.SceneTransitStart;
            SceneTransitDone = StaticEvent.SceneTransitDone;
            SceneTransitPrep = StaticEvent.SceneTransitPrep;
            TimeBcastEvent = StaticEvent.TimeBcastEvent;
            EnemyPopManagerReady = StaticEvent.EnemyPopManagerReady;
        }
    }

    public static class UIEvents
    {
        public static readonly StaticEvent BlackIn;
        public static readonly StaticEvent BlackOut;
        public static readonly StaticEvent PrepToTeleport;
        public static readonly StaticEvent OpenInventory;
        public static readonly StaticEvent DialogStarted;
        public static readonly StaticEvent DraggedItem;
        public static readonly StaticEvent ChangeNPCInventory;
        public static readonly StaticEvent OpenTrading;

        static UIEvents()
        {
            BlackIn = StaticEvent.BlackIn;
            PrepToTeleport = StaticEvent.PrepToTeleport;
            OpenInventory = StaticEvent.OpenInventory;
            BlackOut = StaticEvent.BlackOut;
            DialogStarted = StaticEvent.DialogStarted;
            DraggedItem = StaticEvent.DraggedItem;
            ChangeNPCInventory = StaticEvent.ChangeNPCInventory;
            OpenTrading = StaticEvent.OpenTrading;
        }
    }

    public static class PlayEvents
    {
        public static readonly StaticEvent PlayerDeath;
        public static readonly StaticEvent InteractableEntered;
        public static readonly StaticEvent InteractableExited;
        public static readonly StaticEvent PlayerHealthChange;
        public static readonly StaticEvent PlayerPurityChange;

        static PlayEvents()
        {
            PlayerDeath = StaticEvent.PlayerDeath;
            PlayerHealthChange = StaticEvent.PlayerHealthChange;
            PlayerPurityChange = StaticEvent.PlayerPurityChange;
            InteractableEntered = StaticEvent.InteractableEntered;
            InteractableExited = StaticEvent.InteractableExited;
        }
    }
}