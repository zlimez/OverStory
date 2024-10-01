using UnityEngine;
using UnityEngine.UI;
using Abyss.EventSystem;

public class InventorySlot : MonoBehaviour
{
    public Countable<Item> ItemPile;
    public Collection RefCollection;
    public Image Icon;
    public GameObject BG;
    public Text Amount;
    public Button SlotButton;

    public void Deselect()
    {
        BG.SetActive(false);
    }

    public void Select()
    {
        BG.SetActive(true);
    }

    public void SetItem(Countable<Item> newItemStack)
    {
        ItemPile = newItemStack;

        Icon.sprite = newItemStack.Data.icon;
        Icon.preserveAspect = true;
        Amount.text = newItemStack.Count.ToString();

        Icon.enabled = true;
        Amount.enabled = true;
        SlotButton.interactable = true;
    }

    public void ClearSlot()
    {
        ItemPile = null;

        Icon.sprite = null;
        Amount.text = "";

        Icon.enabled = false;
        Amount.enabled = false;
        SlotButton.interactable = false;
    }

    public void UseItem()
    {
        if (ItemPile != null && ItemPile.Data.canUseFromInventory)
            RefCollection.UseItem(ItemPile.Data);
    }
}
