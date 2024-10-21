using Abyss.EventSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell")]
public class SpellItem : Item
{
    // public float damage;
    // public float radius;
    public Sprite iconInactive;

    protected override void Awake()
    {
        canUseFromInventory = false;
        isConsumable = false;
    }

    public override void Use()
    {
        base.Use();
        // EventManager.InvokeEvent(PlayEvents.SpellEquipped, this);
    }
}
