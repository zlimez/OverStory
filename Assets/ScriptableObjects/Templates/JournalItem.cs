using System.Collections.Generic;
using Abyss.Player;
using Tuples;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Journal")]
public class JournalItem : Item
{
    public JournalType journalType;

    void Awake()
    {
        canUseFromInventory = false;
        isConsumable = false;
        itemType = ItemType.Journal;
    }

    public enum JournalType
    {
        Bestiary,
        Tribe
    }
}
