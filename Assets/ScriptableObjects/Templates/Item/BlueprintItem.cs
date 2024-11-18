using System.Collections.Generic;
using Abyss.Player;
using Tuples;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Blueprint")]
public class BlueprintItem : Item
{
    public Item objectItem;
    public List<RefPair<Item, int>> materials = new(2);

    public List<Item> prerequisiteItems;
    public PlayerAttr prerequisiteAttr;
    // FIXME: This is temporary hack, all blueprint should be craftable, but such will cos as spell and construction art are considered blueprint now they will show wrongly in the crafting panel
    public bool isCraftable = true;

    protected override void OnValidate()
    {
        base.OnValidate();
        canUseFromInventory = false;
        isConsumable = false;
        isAcceptableToFara = false;
        isAcceptableToHakem = false;
        valueToFara = 0;
        valueToHakem = 0;
        itemType = ItemType.Blueprints;
    }
}
