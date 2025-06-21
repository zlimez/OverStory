namespace Abyss.EventSystem
{
    public static class SystemEvents
    {
        public static readonly NamedEvent SystemsReady;
        public static readonly NamedEvent LedgerReady;
        public static readonly NamedEvent SceneTransitStart;
        public static readonly NamedEvent SceneTransitDone;
        public static readonly NamedEvent SceneTransitPrep;
        public static readonly NamedEvent TimeBcastEvent;
        public static readonly NamedEvent EnemyPopManagerReady;
        public static readonly NamedEvent ChangeCameraBG;


        static SystemEvents()
        {
            SystemsReady = NamedEvent.SystemsReady;
            LedgerReady = NamedEvent.LedgerReady;
            SceneTransitStart = NamedEvent.SceneTransitStart;
            SceneTransitDone = NamedEvent.SceneTransitDone;
            SceneTransitPrep = NamedEvent.SceneTransitPrep;
            TimeBcastEvent = NamedEvent.TimeBcast;
            EnemyPopManagerReady = NamedEvent.EnemyPopManagerReady;
            ChangeCameraBG = NamedEvent.ChangeCameraBG;
        }
    }

    public static class UIEvents
    {
        public static readonly NamedEvent BlackIn, BlackOut;
        public static readonly NamedEvent DraggedItem, SelectItem;
        public static readonly NamedEvent UpdateNPCInventory;
        public static readonly NamedEvent TutorialDisplay, TutorialClose, Message;


        static UIEvents()
        {
            BlackIn = NamedEvent.BlackIn;
            BlackOut = NamedEvent.BlackOut;
            DraggedItem = NamedEvent.DraggedItem;
            UpdateNPCInventory = NamedEvent.UpdateNPCInventory;
            SelectItem = NamedEvent.SelectItem;
            TutorialDisplay = NamedEvent.TutorialDisplay;
            TutorialClose = NamedEvent.TutorialClose;
            Message = NamedEvent.Message;
        }
    }

    public static class PlayEvents
    {
        public static readonly NamedEvent PlayerDeath, Respawn;
        public static readonly NamedEvent InteractableEntered, InteractableExited;
        public static readonly NamedEvent PlayerIntelChange;
        public static readonly NamedEvent PlayerHealthChange;
        public static readonly NamedEvent PurityChange, ActionPurityChange, FriendlinessPurityChange;
        public static readonly NamedEvent TradePostEntered, TradePostExited;
        public static readonly NamedEvent LearningPostEntered, LearningPostExited;
        public static readonly NamedEvent CraftingPostEntered;
        public static readonly NamedEvent WeaponEquipped, WeaponUnequipped, SpellChange;
        public static readonly NamedEvent RestStart, InRest, RestEnd; public static readonly NamedEvent BuildStart, BuildEnd;
        public static readonly NamedEvent LureUsed, LurePlaced;
        public static readonly NamedEvent PlayerSpeak, PlayerSpriteFlip, PlayerItemChange;
        public static readonly NamedEvent GetArm;


        static PlayEvents()
        {
            PlayerDeath = NamedEvent.PlayerDeath;
            PlayerIntelChange = NamedEvent.PlayerIntelChange;
            PlayerHealthChange = NamedEvent.PlayerHealthChange;
            PurityChange = NamedEvent.PurityChange;
            ActionPurityChange = NamedEvent.ActionPurityChange;
            FriendlinessPurityChange = NamedEvent.FriendlinessPurityChange;
            InteractableEntered = NamedEvent.InteractableEntered;
            InteractableExited = NamedEvent.InteractableExited;
            TradePostEntered = NamedEvent.TradePostEntered;
            TradePostExited = NamedEvent.TradePostExited;
            LearningPostEntered = NamedEvent.LearningPostEntered;
            LearningPostExited = NamedEvent.LearningPostExited;
            CraftingPostEntered = NamedEvent.CraftingPostEntered;
            WeaponEquipped = NamedEvent.WeaponEquipped;
            WeaponUnequipped = NamedEvent.WeaponUnequipped;
            RestStart = NamedEvent.RestStart;
            InRest = NamedEvent.InRest;
            RestEnd = NamedEvent.RestEnd;
            SpellChange = NamedEvent.SpellChange;
            Respawn = NamedEvent.Respawn;
            BuildStart = NamedEvent.BuildStart;
            BuildEnd = NamedEvent.BuildEnd;
            LureUsed = NamedEvent.LureUsed;
            LurePlaced = NamedEvent.LurePlaced;
            PlayerSpeak = NamedEvent.PlayerSpeak;
            PlayerSpriteFlip = NamedEvent.PlayerSpriteFlip;
            PlayerItemChange = NamedEvent.PlayerItemChange;
        }
    }
}