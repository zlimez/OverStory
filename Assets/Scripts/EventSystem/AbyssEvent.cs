namespace Abyss.EventSystem
{
    // WARNING: Do not change the order of the enum values or their assigned integer values
    // after development has started, as it may cause issues with saved data and other parts
    // of the code that rely on these values. If you need to add new values, append them to
    // the end of the list and assign them new unique integer values.
    public enum StaticEvent
    {
        NoEvent = 0,
        Core_GameManagerReady = 1,
        Core_TransitionWithMaster = 2,
        Core_TransitionWithMasterCompleted = 3,
        Core_Transition = 4,
        Core_InteractableEntered = 5,
        Common_CurtainDrawn = 6,
        Common_PrepToTeleport = 7,
        Common_OpenInventory = 8,
        Common_CurtainOpen = 9,
        Common_ItemUsed = 10,
        Common_DialogStarted = 11,
        Common_PlayerPositionMoved = 12,
        Common_PlayerDeath = 13,
        Common_ObjectPickedUp = 14,
        Common_ObjectPutDown = 15,
    }


    public static class DynamicEvent { }
}
