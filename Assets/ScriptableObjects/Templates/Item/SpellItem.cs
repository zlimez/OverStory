using UnityEngine;

[CreateAssetMenu(menuName = "Item/Spell")]
public class SpellItem : Item
{
    public Sprite iconInactive;
    public int rottenFleshNeeded;
    public Item rottenFlesh;

    public bool CanCast => GameManager.Instance.Inventory.MaterialCollection.StockOf(rottenFlesh) >= rottenFleshNeeded;

    protected override void OnValidate()
    {
        base.OnValidate();
        canUseFromInventory = false;
        isConsumable = false;
        isAcceptableToFara = false;
        isAcceptableToHakem = false;
        valueToFara = 0;
        valueToHakem = 0;
        itemType = ItemType.Spells;
    }
}
