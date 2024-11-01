using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Inventory
{
    public int Level = 1;
    public bool Enabled = false;
    public Action<Item> OnItemInspected;
    // TODO: Different class of items in different collection, e.g. Organs in one, materials in another
    public Collection MaterialCollection { get; private set; } = new();
    int _itemCount = 0;

    public void Add(Item item, int quantity = 1)
    {
        Debug.Log($"{item.itemName} added to Inventory");
        MaterialCollection.Add(item, quantity);
        _itemCount += quantity;
    }

    public void Clear()
    {
        MaterialCollection.Clear();
    }

    public void RanRemovePortion(float frac)
    {
        Assert.IsTrue(frac <= 1);
        int remCnt = (int)(frac * _itemCount);
        // now only one collection so remove all from material collection
        MaterialCollection.RanRemoveN(remCnt);
    }
}
