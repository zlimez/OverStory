using System.Collections.Generic;
using Abyss.EventSystem;
using Tuples;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Blueprint")]
public class BlueprintItem : Item
{
    // public float damage;
    // public float radius;
    public Item objectItem;
    public List<RefPair<Item, int>> materials = new List<RefPair<Item, int>>(2);

    void Awake()
    {
        canUseFromInventory = false;
        isConsumable = false;
        itemType = ItemType.Blueprints;
    }

    public override void Use()
    {
        base.Use();
        // EventManager.InvokeEvent(PlayEvents.SpellEquipped, this);
    }
}
