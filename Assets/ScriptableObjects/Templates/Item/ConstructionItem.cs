using System.Collections.Generic;
using Tuples;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Construction")]
public class ConstructionItem : Item
{
    public List<RefPair<Item, int>> materials = new(2);
    [Tooltip("Time required when construction level is 1")] public float baseBuildTime;
    public int Durability = 3;

    protected override void OnValidate()
    {
        base.OnValidate();
        canUseFromInventory = false;
        isConsumable = false;
        isAcceptableToFara = false;
        isAcceptableToHakem = false;
        valueToFara = 0;
        valueToHakem = 0;
        itemType = ItemType.Constructions;
    }
}
