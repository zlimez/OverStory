using UnityEngine;

[CreateAssetMenu(menuName = "Item/Journal")]
public class JournalItem : Item
{
    public JournalType journalType;

    protected override void OnValidate()
    {
        base.OnValidate();
        canUseFromInventory = false;
        isConsumable = false;
        isAcceptableToFara = false;
        isAcceptableToHakem = false;
        valueToFara = 0;
        valueToHakem = 0;
        itemType = ItemType.Journal;
    }

    public enum JournalType
    {
        Bestiary,
        Tribe,
        Deposit
    }
}
