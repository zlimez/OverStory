using System;
using UnityEngine;

public class Inventory
{
    public int Level = 1;
    public Action<Item> OnItemInspected;
    // TODO: Different class of items in different collection, e.g. Organs in one, materials in another
    public Collection MaterialCollection { get; private set; } = new();

    public void AddTo(Item item, int quantity = 1)
    {
        Debug.Log($"{item.itemName} added to Inventory");
        MaterialCollection.Add(item, quantity);
    }
}
