using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CollectionEvent : UnityEvent<Collection> { }

[System.Serializable]
public class ItemWithCount
{
    public Item item;
    public int count;

    public ItemWithCount(Item item, int count)
    {
        this.item = item;
        this.count = count;
    }
}

public class NPCInventoryInitializer : MonoBehaviour
{
    public Collection NPCItems = new(null, 999);
    public CollectionEvent InitNPCInventory;
    public List<ItemWithCount> initialItems;

    private void Start() => InitializeInventory();

    public void InitializeInventory()
    {
        foreach (var itemStack in initialItems)
            NPCItems.Add(itemStack.item, itemStack.count);

        InitNPCInventory.Invoke(NPCItems);
    }
}
