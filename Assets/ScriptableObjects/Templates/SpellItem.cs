using UnityEngine;

[CreateAssetMenu(menuName = "Item/Spell")]
public class SpellItem : Item
{
    public Sprite iconInactive;
    public GameObject SpellPrefab;

    void Awake()
    {
        canUseFromInventory = false;
        isConsumable = false;
        itemType = ItemType.Spells;
    }
}
