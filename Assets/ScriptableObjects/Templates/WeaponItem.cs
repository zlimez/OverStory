using Abyss.EventSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Weapon")]
public class WeaponItem : Item
{
    [Header("Attributes")]
    public float damage;
    public float radius;

    void Awake()
    {
        canUseFromInventory = false;
        isConsumable = false;
    }

    public override void Use()
    {
        base.Use();
        EventManager.InvokeEvent(PlayEvents.WeaponEquipped, this);
    }
}
