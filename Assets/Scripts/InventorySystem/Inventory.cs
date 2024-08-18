using System;
using UnityEngine;
using System.Collections.Generic;
using Chronellium.EventSystem;

public class Inventory
{
    public static Inventory Instance;
    public static GameEvent inventoryAssignedEvent = new GameEvent("New inventory assigned");
    public Action<Item> onItemInspected;
    public Collection NormalCollection { get; private set; }
    public Collection ScannedCollection { get; private set; }
    private int defaultScannedCollectionCapacity = 4;
    public Func<Collection, Item, int, bool> normalAddRule = (collection, item, quantity) => true;
    // Each slot in scanned collection can only contain one item of stock count 1
    public Func<Collection, Item, int, bool> scanAddRule = (collection, item, quantity) => !collection.Contains(item) && quantity == 1;

    // Used to determine inventory state after travelling to the past, normal collection reverts, scanned collection persists
    public static Inventory Merge(Inventory pastInventory, Inventory preLeapInventory)
    {
        Collection normalCollection = pastInventory.NormalCollection.GetCopy();
        Collection scannedCollection = preLeapInventory.ScannedCollection.GetCopy();
        List<Item> duplicatesInScannedCollection = new List<Item>();
        foreach (KeyValuePair<Item, Countable<Item>> item in scannedCollection.itemsTable)
        {
            if (normalCollection.Contains(item.Key))
            {
                duplicatesInScannedCollection.Add(item.Key);
            }
        }

        foreach (Item duplicateItem in duplicatesInScannedCollection)
        {
            scannedCollection.RemoveItem(duplicateItem);
        }
        Debug.Log($"Post merge inventory normal collection {normalCollection.Size()} scanned collection {scannedCollection.Size()}");
        return new Inventory(normalCollection, scannedCollection);
    }

    public static void AssignNewInventory(Inventory newInventory)
    {
        Debug.Log("New inventory assigned");
        Instance = newInventory;
        EventManager.InvokeEvent(inventoryAssignedEvent);
    }

    public Inventory(Collection normalCollection = null, Collection scannedCollection = null)
    {
        NormalCollection = normalCollection ?? new Collection(normalAddRule);
        ScannedCollection = scannedCollection ?? new Collection(scanAddRule, null, defaultScannedCollectionCapacity);
    }

    // FIXME: Add to selected collection if possible, if not add to the other collection
    public void AddTo(bool isNormal, Item item, int quantity = 1)
    {
        Debug.Log($"{item.itemName} added to Inventory");
        if (isNormal)
        {
            NormalCollection.Add(item, quantity);
        }
        else
        {
            ScannedCollection.Add(item, quantity);
        }
    }

    public bool Contains(Item item)
    {
        return NormalCollection.Contains(item) || ScannedCollection.Contains(item);
    }

    public int AmountOf(Item item)
    {
        return NormalCollection.StockOf(item) + ScannedCollection.StockOf(item);
    }
}
