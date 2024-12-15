namespace Abyss.EventSystem
{
    // WARNING: Do not change the order of the enum values or their assigned integer values
    // after development has started, as it may cause issues with saved data and other parts
    // of the code that rely on these values. If you need to add new values, append them to
    // the end of the list and assign them new unique integer values.
    [System.Serializable]
    public enum StaticEvent
    {
        NoEvent = 0,
        SystemsReady = 1,
        EnemyPopManagerReady = 21,

        SceneTransitStart = 2,
        SceneTransitDone = 3,
        SceneTransitPrep = 4,

        BlackIn = 5,
        BlackOut = 6,

        PlayerDeath = 11,
        Respawn = 29,

        InteractableEntered = 14,
        InteractableExited = 15,

        PlayerHealthChange = 16,
        PlayerPurityChange = 17,

        DraggedItem = 18,
        UpdateNPCInventory = 19,
        SelectItem = 31,

        TradePostEntered = 22,
        LearningPostEntered = 30,
        CraftingPostEntered = 32,

        WeaponEquipped = 23,
        WeaponUnequipped = 24,
        SpellEquippedStateChange = 26,

        LureUsed = 37,
        LurePlaced = 38,

        RestStart = 25,
        InRest = 36,
        RestEnd = 35,

        PlayerActionPurityChange = 27,
        PlayerFriendlinessPurityChange = 28,

        TimeBcastEvent = 20,

        BuildStart = 33,
        BuildEnd = 34,
        PlayerSpeak = 39,
        PlayerSpeakFlip = 40,
        LedgerReady = 41,
        PlayerIntelligenceChange = 42,
        PlayerItemChange = 43,
        Message = 44,
        ChangeCameraBG = 45,
    }
}
