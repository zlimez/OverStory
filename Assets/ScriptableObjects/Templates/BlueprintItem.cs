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

    void Awake()
    {
        canUseFromInventory = false;
        isConsumable = false;
        itemType = ItemType.Blueprints;
    }
}
