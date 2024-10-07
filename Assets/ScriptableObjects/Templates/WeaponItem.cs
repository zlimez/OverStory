using Abyss.EventSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon")]
public class WeaponItem : Item
{
    [Header("Attributes")]
    public float damage;
    public float radius;
    public readonly static string WeaponEquippedPrefix = "Weapon equipped";

    protected override void Awake()
    {
        itemUsedEvent = new GameEvent($"{WeaponEquippedPrefix}: ${itemName}");
        canUseFromInventory = false;
        isConsumable = false;
    }

    public override void Use()
    {
        base.Use();
        EventManager.InvokeEvent(new GameEvent(WeaponEquippedPrefix), this);
    }
}
