using UnityEngine;
using Abyss.EventSystem;

[CreateAssetMenu(menuName = "Item/General")]
public class Item : ScriptableObject
{
    public string itemName;
    [TextArea(3, 5)] public string description;
    public Sprite icon;
    public Sprite itemImage;
    public GameObject itemPrefab;
    public bool canUseFromInventory = true; // Some items are auto consumed when interacting with target
    public bool isConsumable = true; // Items like map or files are not consumable and thus cannot decrease in amount
    public bool isAcceptableToFara = false;
    public bool isAcceptableToHakem = false;
    public int valueToFara, valueToHakem;

    public ItemType itemType;
    protected GameEvent itemUsedEvent;

    protected virtual void Awake() => itemUsedEvent = new GameEvent($"{itemName} used");

    public virtual void Use()
    {
        // Non consumables will be inspected instead via zoom box
        if (!isConsumable)
            GameManager.Instance.Inventory.OnItemInspected?.Invoke(this);
        EventManager.InvokeEvent(itemUsedEvent);
    }

    public override bool Equals(object other)
    {
        if (other is Item item)
            return item.itemName == itemName;
        return false;
    }

    public override int GetHashCode() => itemName.GetHashCode();
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
