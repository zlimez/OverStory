using DeepBreath.ReplaySystem;

namespace Chronellium.EventSystem
{
    // WARNING: Do not change the order of the enum values or their assigned integer values
    // after development has started, as it may cause issues with saved data and other parts
    // of the code that rely on these values. If you need to add new values, append them to
    // the end of the list and assign them new unique integer values.
    public enum StaticEvent
    {
        NoEvent = 0,
        Core_SwitchToOtherWorld = 1,
        Core_SwitchToRealWorld = 2,
        Core_OutOfBreath = 3,
        Core_LowBreath = 4,
        Core_GameManagerReady = 5,
        Core_TransitionWithMaster = 6,
        Core_TransitionWithMasterCompleted = 7,
        Core_Transition = 8,
        Core_InteractableEntered = 9,
        Common_CurtainFullyDrawn = 10,
        Common_PrepToTeleport = 11,
        Common_OpenInventory = 12,
        Common_CurtainFullyOpen = 13,
        Common_ItemUsed = 14,
        Common_DialogStarted = 15,
        Common_PlayerPositionMoved = 56,
        Common_PlayerChangeDirection = 57,
        Common_ObjectPickedUp = 63,
        Common_ObjectPutDown = 64,
        Common_GrabStateChanged = 65,
        Core_ResetPuzzle = 99,
    }


    public static class DynamicEvent
    {
        public readonly static string ReplayCompleteEventPrefix = "Replay completed for ";
        public readonly static GameEvent GhostReplayCompleted = new GameEvent(ReplayCompleteEventPrefix + "SwimCharacter (Ghost)");
        public readonly static GameEvent EngagedWindmill = new GameEvent("Engaged windmill");
        public readonly static GameEvent DisengagedWindmill = new GameEvent("Disengaged windmill");
        public readonly static GameEvent CableReached = new GameEvent("Cable seat reached end");
    }
}
