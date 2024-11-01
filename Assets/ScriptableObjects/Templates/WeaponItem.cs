using Abyss.EventSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Weapon")]
public class WeaponItem : Item
{
    [Header("Attributes")]
    public float Damage;
    public float Radius;

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
