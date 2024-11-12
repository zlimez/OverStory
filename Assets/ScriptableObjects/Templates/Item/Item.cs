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
    [SerializeField] protected DynamicEvent itemUsedEvent;

    public ItemType itemType;

    protected virtual void OnValidate()
    {
        if (itemName == null || itemName == "")
            itemName = name;
    }

    public virtual void Use()
    {
        // Non consumables will be inspected instead via zoom box
        if (!isConsumable)
            GameManager.Instance.Inventory.OnItemInspected?.Invoke(this);
        if (itemUsedEvent != null) EventManager.InvokeEvent(new GameEvent(itemUsedEvent.EventName));
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
    Constructions,
    Journal,
    QuestItems
}
