using UnityEngine;

[CreateAssetMenu(menuName = "Item/Spell")]
public class SpellItem : Item
{
    public Sprite iconInactive;
    public int rottenFleshNeeded;
    public Item rottenFlesh;

    public bool CanCast => GameManager.Instance.Inventory.MaterialCollection.StockOf(rottenFlesh) >= rottenFleshNeeded;

    void Awake()
    {
        canUseFromInventory = false;
        isConsumable = false;
        itemType = ItemType.Spells;
    }
}
