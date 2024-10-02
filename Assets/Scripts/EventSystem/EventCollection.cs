using System.Collections.Generic;

namespace Abyss.EventSystem
{
    public static class CoreEventCollection
    {
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
        }
    }

    public static class CommonEventCollection
    {
        public static readonly StaticEvent CurtainDrawn;
        public static readonly StaticEvent CurtainOpen;
        public static readonly StaticEvent PrepToTeleport;
        public static readonly StaticEvent OpenInventory;
        public static readonly StaticEvent ForcedRewind;
        public static readonly StaticEvent DialogStarted;
        public static readonly StaticEvent PlayerMoved;

        static CommonEventCollection()
        {
            CurtainDrawn = StaticEvent.Common_CurtainDrawn;
            PrepToTeleport = StaticEvent.Common_PrepToTeleport;
            OpenInventory = StaticEvent.Common_OpenInventory;
            CurtainOpen = StaticEvent.Common_CurtainOpen;
            DialogStarted = StaticEvent.Common_DialogStarted;
            PlayerMoved = StaticEvent.Common_PlayerPositionMoved;
        }
    }
}