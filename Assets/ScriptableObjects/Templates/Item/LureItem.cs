using Abyss.EventSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "New Lure", menuName = "Item/Lure")]
public class Lure : Item
{
    public SpecyAttr specy;
    public float radius;

    protected override void OnValidate()
    {
        base.OnValidate();
        isConsumable = true;
        canUseFromInventory = true;
        isAcceptableToFara = false;
        isAcceptableToHakem = false;
        valueToFara = 0;
        valueToHakem = 0;
        itemType = ItemType.Consumables;
    }

    public override void Use() => EventManager.InvokeEvent(PlayEvents.LureUsed, (radius, specy.specyName));
}
