using System.Collections.Generic;
using Abyss.Player;
using Tuples;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Construction")]
public class ConstructionItem : Item
{
    public Item objectItem;
    public List<RefPair<Item, int>> materials = new(2);

    void Awake()
    {
        canUseFromInventory = false;
        isConsumable = false;
        itemType = ItemType.Constructions;
    }
}
