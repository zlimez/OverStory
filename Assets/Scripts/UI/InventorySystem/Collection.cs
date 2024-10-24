using System;
using System.Collections.Generic;

public class Collection
{
    private readonly int _capacity;
    public List<Countable<Item>> Items { get; private set; } // Used to preserve order
    public Dictionary<Item, Countable<Item>> ItemsTable { get; private set; }
    public Action OnItemChanged;
    public int ItemCount { get; private set; } = 0;

    public Collection(List<Countable<Item>> items = null, int capacity = 10)
    {
        Items = items ?? new List<Countable<Item>>();
        ItemsTable = new Dictionary<Item, Countable<Item>>();
        _capacity = capacity;

        foreach (Countable<Item> itemStack in Items)
            ItemsTable.Add(itemStack.Data, itemStack);
    }

    public bool Add(Item item, int quantity = 1)
    {
        if (IsFull) return false;

        ItemCount += quantity;
        if (ItemsTable.ContainsKey(item))
            ItemsTable[item].AddToStock(quantity);
        else
        {
            // New item
            Countable<Item> itemStack = new(item, quantity);
            ItemsTable.Add(item, itemStack);
            Items.Add(itemStack);
        }

        OnItemChanged?.Invoke();
        return true;
    }

    public bool IsFull => _capacity != -1 && Items.Count >= _capacity;
    public int Size => Items.Count;
    public bool IsEmpty => Size == 0;

    public bool Contains(Item item) => ItemsTable.ContainsKey(item);

    public int StockOf(Item item)
    {
        if (ItemsTable.ContainsKey(item))
            return ItemsTable[item].Count;
        return 0;
    }

    public void Clear()
    {
        Items.Clear();
        ItemsTable.Clear();
        OnItemChanged?.Invoke();
    }

    public void Remove(Item item)
    {
        Items.Remove(ItemsTable[item]);
        ItemsTable.Remove(item);
    }

    public void RanRemoveN(int n)
    {
        Random ran = new();
        for (int i = 0; i < Items.Count - 1; i++)
        {
            int rmCnt = Math.Min(n, (int)(ran.NextDouble() * Items[i].Count));
            n -= rmCnt;
            RemoveStock(Items[i].Data, rmCnt);
            if (n == 0) break;
        }
        if (n > 0) RemoveStock(Items[Items.Count - 1].Data, n);
        OnItemChanged?.Invoke();
    }

    public bool UseItem(Item item, int quantity = 1)
    {
        if (ItemsTable.ContainsKey(item) && ItemsTable[item].Count >= quantity)
        {
            for (int i = 0; i < quantity; i++)
                item.Use();

            bool noneLeft = item.isConsumable && ItemsTable[item].RemoveStock(quantity);
            if (noneLeft)
                Remove(item);

            OnItemChanged?.Invoke();
            return true;
        }
        return false;
    }

    public bool RemoveStock(Item item, int quantity = 1)
    {
        if (ItemsTable.ContainsKey(item) && ItemsTable[item].Count >= quantity)
        {
            bool noneLeft = ItemsTable[item].RemoveStock(quantity);
            if (noneLeft)
                Remove(item);

            OnItemChanged?.Invoke();
            return true;
        }
        return false;
    }
}
