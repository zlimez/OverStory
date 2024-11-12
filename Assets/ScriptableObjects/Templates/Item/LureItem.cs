using Abyss.EventSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "New Lure", menuName = "Item/Lure")]
public class Lure : Item
{
    public SpecyAttr specy;
    public float radius;

    void OnValidate()
    {
        isConsumable = true;
        canUseFromInventory = true;
        isAcceptableToFara = false;
        isAcceptableToHakem = false;
        itemType = ItemType.Consumables;
    }

    public override void Use() => EventManager.InvokeEvent(PlayEvents.LureUsed, (radius, specy.specyName));
}
