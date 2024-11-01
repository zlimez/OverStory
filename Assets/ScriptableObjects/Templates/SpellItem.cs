using Abyss.EventSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Spell")]
public class SpellItem : Item
{
    // public float damage;
    // public float radius;
    public Sprite iconInactive;

    void Awake()
    {
        canUseFromInventory = false;
        isConsumable = false;
        itemType = ItemType.Spells;
    }

    public override void Use()
    {
        base.Use();
        // EventManager.InvokeEvent(PlayEvents.SpellEquipped, this);
    }
}
