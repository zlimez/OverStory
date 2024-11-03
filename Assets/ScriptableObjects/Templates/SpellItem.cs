using UnityEngine;

[CreateAssetMenu(menuName = "Item/Spell")]
public class SpellItem : Item
{
    public Sprite iconInactive;

    void Awake()
    {
        canUseFromInventory = false;
        isConsumable = false;
        itemType = ItemType.Spells;
    }
}
