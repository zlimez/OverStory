using System;
using System.Collections.Generic;

public class Collection
{
    private int _capacity;
    public List<Countable<Item>> Items { get; private set; }
    public Dictionary<Item, Countable<Item>> ItemsTable { get; private set; }
    public Action onItemChanged;
    public Action<Item> onNewItemAdded;

    public Collection(List<Countable<Item>> items = null, int capacity = 10)
    {
        this.Items = items ?? new List<Countable<Item>>();
        this.ItemsTable = new Dictionary<Item, Countable<Item>>();
        this._capacity = capacity;

        foreach (Countable<Item> itemStack in this.Items)
            ItemsTable.Add(itemStack.Data, itemStack);
    }

    public bool Add(Item item, int quantity = 1)
    {
        if (IsFull) return false;

        if (ItemsTable.ContainsKey(item))
            ItemsTable[item].AddToStock(quantity);
        else
        {
            // New item
            Countable<Item> itemStack = new(item, quantity);
            ItemsTable.Add(item, itemStack);
            Items.Add(itemStack);
            onNewItemAdded?.Invoke(item);
        }

        onItemChanged?.Invoke();
        return true;
    }

    public bool IsFull => _capacity != -1 && Items.Count >= _capacity;
    public int Size => Items.Count;
    public bool IsEmpty => Size == 0;

    public bool Contains(Item item)
    {
        return ItemsTable.ContainsKey(item);
    }

    public int StockOf(Item item)
    {
        if (ItemsTable.ContainsKey(item))
            return ItemsTable[item].Count;
        return 0;
    }

    public void RemoveItem(Item item)
    {
        Items.Remove(ItemsTable[item]);
        ItemsTable.Remove(item);
    }

    public bool UseItem(Item item, int quantity = 1)
    {
        if (ItemsTable.ContainsKey(item) && ItemsTable[item].Count >= quantity)
        {
            for (int i = 0; i < quantity; i++)
                item.Use();

            bool noneLeft = item.isConsumable && ItemsTable[item].RemoveStock(quantity);
            if (noneLeft)
                RemoveItem(item);

            onItemChanged?.Invoke();
            return true;
        }
        return false;
    }

    public bool DiscardItem(Item item, int quantity = 1)
    {
        if (ItemsTable.ContainsKey(item) && ItemsTable[item].Count >= quantity)
        {
            bool noneLeft = ItemsTable[item].RemoveStock(quantity);
            if (noneLeft)
                RemoveItem(item);

            onItemChanged?.Invoke();
            return true;
        }
        return false;
    }
}
