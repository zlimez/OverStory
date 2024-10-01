using System;
using UnityEngine;
using Abyss.EventSystem;

public class Inventory
{
    public static Inventory Instance;
    public static GameEvent inventoryAssignedEvent = new("Inventory assigned");
    public Action<Item> onItemInspected;
    // Different class of items in different collection, e.g. Organs in one, materials in another
    public Collection MaterialCollection { get; private set; }

    public static void AssignInventory(Inventory newInventory)
    {
        Instance = newInventory;
        EventManager.InvokeEvent(inventoryAssignedEvent);
    }

    public Inventory()
    {
        MaterialCollection = new Collection();
    }

    public void AddTo(Item item, int quantity = 1)
    {
        Debug.Log($"{item.itemName} added to Inventory");
        MaterialCollection.Add(item, quantity);
    }
}
