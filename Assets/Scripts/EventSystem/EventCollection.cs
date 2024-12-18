namespace Abyss.EventSystem
{
    public static class SystemEvents
    {
        public static readonly StaticEvent SystemsReady;
        public static readonly StaticEvent LedgerReady;
        public static readonly StaticEvent SceneTransitStart;
        public static readonly StaticEvent SceneTransitDone;
        public static readonly StaticEvent SceneTransitPrep;
        public static readonly StaticEvent TimeBcastEvent;
        public static readonly StaticEvent EnemyPopManagerReady;


        static SystemEvents()
        {
            SystemsReady = StaticEvent.SystemsReady;
            LedgerReady = StaticEvent.LedgerReady;
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
        public static readonly StaticEvent DraggedItem;
        public static readonly StaticEvent SelectItem;
        public static readonly StaticEvent UpdateNPCInventory;

        static UIEvents()
        {
            BlackIn = StaticEvent.BlackIn;
            BlackOut = StaticEvent.BlackOut;
            DraggedItem = StaticEvent.DraggedItem;
            UpdateNPCInventory = StaticEvent.UpdateNPCInventory;
            SelectItem = StaticEvent.SelectItem;
        }
    }

    public static class PlayEvents
    {
        public static readonly StaticEvent PlayerDeath;
        public static readonly StaticEvent InteractableEntered;
        public static readonly StaticEvent InteractableExited;
        public static readonly StaticEvent PlayerIntelligenceChange;
        public static readonly StaticEvent PlayerHealthChange;
        public static readonly StaticEvent PlayerPurityChange;
        public static readonly StaticEvent PlayerActionPurityChange;
        public static readonly StaticEvent PlayerFriendlinessPurityChange;
        public static readonly StaticEvent TradePostEntered;
        public static readonly StaticEvent LearningPostEntered;
        public static readonly StaticEvent CraftingPostEntered;
        public static readonly StaticEvent WeaponEquipped;
        public static readonly StaticEvent WeaponUnequipped;
        public static readonly StaticEvent RestStart;
        public static readonly StaticEvent InRest;
        public static readonly StaticEvent RestEnd;
        public static readonly StaticEvent SpellEquippedStateChange;
        public static readonly StaticEvent Respawn;
        public static readonly StaticEvent BuildStart;
        public static readonly StaticEvent BuildEnd;
        public static readonly StaticEvent LureUsed;
        public static readonly StaticEvent LurePlaced;
        public static readonly StaticEvent PlayerSpeak;
        public static readonly StaticEvent PlayerSpeakFlip;
        public static readonly StaticEvent PlayerItemChange;
        public static readonly StaticEvent Message;



        static PlayEvents()
        {
            PlayerDeath = StaticEvent.PlayerDeath;
            PlayerIntelligenceChange = StaticEvent.PlayerIntelligenceChange;
            PlayerHealthChange = StaticEvent.PlayerHealthChange;
            PlayerPurityChange = StaticEvent.PlayerPurityChange;
            PlayerActionPurityChange = StaticEvent.PlayerActionPurityChange;
            PlayerFriendlinessPurityChange = StaticEvent.PlayerFriendlinessPurityChange;
            InteractableEntered = StaticEvent.InteractableEntered;
            InteractableExited = StaticEvent.InteractableExited;
            TradePostEntered = StaticEvent.TradePostEntered;
            LearningPostEntered = StaticEvent.LearningPostEntered;
            CraftingPostEntered = StaticEvent.CraftingPostEntered;
            WeaponEquipped = StaticEvent.WeaponEquipped;
            WeaponUnequipped = StaticEvent.WeaponUnequipped;
            RestStart = StaticEvent.RestStart;
            InRest = StaticEvent.InRest;
            RestEnd = StaticEvent.RestEnd;
            SpellEquippedStateChange = StaticEvent.SpellEquippedStateChange;
            Respawn = StaticEvent.Respawn;
            BuildStart = StaticEvent.BuildStart;
            BuildEnd = StaticEvent.BuildEnd;
            LureUsed = StaticEvent.LureUsed;
            LurePlaced = StaticEvent.LurePlaced;
            PlayerSpeak = StaticEvent.PlayerSpeak;
            PlayerSpeakFlip = StaticEvent.PlayerSpeakFlip;
            PlayerItemChange = StaticEvent.PlayerItemChange;
            Message = StaticEvent.Message;
        }
    }
}