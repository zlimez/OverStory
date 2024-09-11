using UnityEngine;
using UnityEngine.UI;
using Abyss.EventSystem;

public class InventorySlot : MonoBehaviour
{
    public Countable<Item> itemStack;
    public Collection referencedCollection;
    public Image icon;
    public GameObject BG;
    public Text amount;
    public Button slotButton;

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
        itemStack = newItemStack;

        icon.sprite = newItemStack.Data.icon;
        icon.preserveAspect = true;
        amount.text = newItemStack.Count.ToString();

        icon.enabled = true;
        amount.enabled = true;
        slotButton.interactable = true;
    }

    public void ClearSlot()
    {
        itemStack = null;

        icon.sprite = null;
        amount.text = "";

        icon.enabled = false;
        amount.enabled = false;
        slotButton.interactable = false;
    }

    public void UseItem()
    {
        if (itemStack != null)
        {
            GameEvent specificItemUseEvent = new GameEvent(Item.ItemUsedPrefix + itemStack.Data.itemName);
            // Debug.Log($"Using {itemStack.Data.itemName}");
            referencedCollection.UseItem(itemStack.Data);
            EventManager.InvokeEvent(CommonEventCollection.ItemUsed);
            EventManager.InvokeEvent(specificItemUseEvent);
        }
    }
}
