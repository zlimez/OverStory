using UnityEngine;
using Abyss.EventSystem;

[CreateAssetMenu(menuName = "Item")]
public class Item : ScriptableObject
{
    public string itemName;
    [TextArea(3, 5)] public string description;
    public Sprite icon;
    public Sprite itemImage;
    public bool canUseFromInventory = true; // Some items are auto consumed when interacting with target
    public bool isConsumable = true; // Items like map or files are not consumable and thus cannot decrease in amount

    public ItemType itemType;
    protected GameEvent itemUsedEvent;

    protected virtual void Awake()
    {
        itemUsedEvent = new GameEvent($"{itemName} used");
    }

    public virtual void Use()
    {
        // Non consumables will be inspected instead via zoom box
        if (!isConsumable)
            Inventory.Instance.onItemInspected?.Invoke(this);
        EventManager.InvokeEvent(itemUsedEvent);
    }

    public override bool Equals(object other)
    {
        if (other is Item item)
            return item.itemName == itemName;
        return false;
    }

    public override int GetHashCode()
    {
        return itemName.GetHashCode();
    }
}

public enum ItemType
{
    Organs,
    Consumables,
    Weapons,
    Materials,
    Farmables,
    Spells,
    Blueprints,
    Constructions
}
