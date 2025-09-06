namespace Abyss.EventSystem
{
    // WARNING: Do not change the order of the enum values or their assigned integer values
    // after development has started, as it may cause issues with saved data and other parts
    // of the code that rely on these values. If you need to add new values, append them to
    // the end of the list and assign them new unique integer values.
    [System.Serializable]
    public enum NamedEvent
    {
        NoEvent,
        SystemsReady, EnemyPopManagerReady, LedgerReady,
        SceneTransitStart, SceneTransitDone, SceneTransitPrep,
        BlackIn, BlackOut,

        PlayerDeath, Respawn, GetArm,
        InteractableEntered, InteractableExited,
        PlayerHealthChange, PurityChange, ActionPurityChange, FriendlinessPurityChange,
        PlayerIntelChange, PlayerItemChange,
        DraggedItem, UpdateNpcInventory, SelectItem,

        TradePostEntered, TradePostExited, LearningPostEntered, LearningPostExited, CraftingPostEntered,
        WeaponEquipped, WeaponUnequipped, SpellChange,
        LureUsed, LurePlaced,
        RestStart, InRest, RestEnd,
        TimeBCast,
        BuildStart, BuildEnd,
        PlayerSpeak, PlayerSpriteFlip,
        Message, ChangeCameraBg,
        TutorialDisplay, TutorialClose,
    }
}
