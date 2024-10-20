using Abyss.EventSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon")]
public class WeaponItem : Item
{
    [Header("Attributes")]
    public float damage;
    public float radius;

    protected override void Awake()
    {
        itemUsedEvent = new GameEvent($"{PlayEvents.WeaponEquipped}: ${itemName}");
        canUseFromInventory = false;
        isConsumable = false;
    }

    public override void Use()
    {
        base.Use();
        EventManager.InvokeEvent(PlayEvents.WeaponEquipped, this);
    }
}
