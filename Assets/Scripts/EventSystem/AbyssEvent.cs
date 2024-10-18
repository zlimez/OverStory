namespace Abyss.EventSystem
{
    // WARNING: Do not change the order of the enum values or their assigned integer values
    // after development has started, as it may cause issues with saved data and other parts
    // of the code that rely on these values. If you need to add new values, append them to
    // the end of the list and assign them new unique integer values.
    public enum StaticEvent
    {
        NoEvent = 0,
        SystemsReady = 1,
        SceneTransitStart = 2,
        SceneTransitDone = 3,
        SceneTransitPrep = 4,
        BlackIn = 5,
        BlackOut = 6,
        PrepToTeleport = 7,
        OpenInventory = 8,
        ItemUsed = 9,
        DialogStarted = 10,
        PlayerDeath = 11,
        ObjectPickedUp = 12,
        ObjectPutDown = 13,
        InteractableEntered = 14,
        InteractableExited = 15,
        PlayerHealthChange = 16,
        PlayerPurityChange = 17,
        DraggedItem = 18,
        UpdateNPCInventory = 19,
        TimeBcastEvent = 20,
        EnemyPopManagerReady = 21,
        TradePostEntered = 22,
    }


    public static class DynamicEvent { }
}
