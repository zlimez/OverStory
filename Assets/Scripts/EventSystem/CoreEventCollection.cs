using System.Collections.Generic;

namespace Chronellium.EventSystem
{
    public static class CoreEventCollection
    {
        public static readonly HashSet<StaticEvent> UtilityEvents;

        public static readonly StaticEvent GameManagerReady;
        public static readonly StaticEvent TransitionWithMaster;
        public static readonly StaticEvent TransitionWithMasterCompleted;
        public static readonly StaticEvent Transition;
        public static readonly StaticEvent InteractableEntered;

        static CoreEventCollection()
        {
            GameManagerReady = StaticEvent.Core_GameManagerReady;
            TransitionWithMaster = StaticEvent.Core_TransitionWithMaster;
            TransitionWithMasterCompleted = StaticEvent.Core_TransitionWithMasterCompleted;
            Transition = StaticEvent.Core_Transition;
            InteractableEntered = StaticEvent.Core_InteractableEntered;

            UtilityEvents = new HashSet<StaticEvent>
            {
                GameManagerReady, TransitionWithMasterCompleted, Transition, TransitionWithMaster
            };
        }
    }

    public static class CommonEventCollection
    {
        public static readonly HashSet<StaticEvent> UtilityEvents;

        public static readonly StaticEvent CurtainFullyDrawn;
        public static readonly StaticEvent CurtainFullyOpen;
        public static readonly StaticEvent PrepToTeleport;
        public static readonly StaticEvent OpenInventory;
        public static readonly StaticEvent ItemUsed;
        public static readonly StaticEvent ForcedRewind;
        public static readonly StaticEvent DialogStarted;
        public static readonly StaticEvent PlayerMoved;

        static CommonEventCollection()
        {
            CurtainFullyDrawn = StaticEvent.Common_CurtainFullyDrawn;
            PrepToTeleport = StaticEvent.Common_PrepToTeleport;
            OpenInventory = StaticEvent.Common_OpenInventory;
            CurtainFullyOpen = StaticEvent.Common_CurtainFullyOpen;
            ItemUsed = StaticEvent.Common_ItemUsed;
            DialogStarted = StaticEvent.Common_DialogStarted;
            PlayerMoved = StaticEvent.Common_PlayerPositionMoved;

            UtilityEvents = new HashSet<StaticEvent>
            {
                CurtainFullyDrawn, CurtainFullyOpen, PrepToTeleport, OpenInventory
            };
        }
    }
}