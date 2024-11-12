using System.Data.Common;
using Abyss.EventSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotForLearning : MonoBehaviour
{
    public Countable<Item> itemStack;
    public Image itemIcon;
    public TextMeshProUGUI itemCount;


    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;

    public void InitializeSlot(Countable<Item> newItem)
    {
        itemStack = newItem;
        if (itemStack != null)
        {
            // RectTransform iconRectTransform = itemIcon.GetComponent<RectTransform>();
            // if (itemStack.Data.itemType == ItemType.Spells) iconRectTransform.sizeDelta = new Vector2(42.49596f, 42.49596f);
            itemIcon.sprite = itemStack.Data.icon;
            // itemCount.text = itemStack.Count.ToString();
            itemCount.text = "";
        }
    }
    void Start()
    {
        CloseTooltip();
    }

    public void ShowTooltip()
    {
        if (tooltipPanel != null && tooltipText != null && itemStack != null)
        {
            tooltipPanel.SetActive(true);
            // need to change
            tooltipText.text = itemStack.Data.itemName + ": " + itemStack.Data.description;
            //
        }
    }

    public void CloseTooltip()
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }


    public void OnClick()
    {
        EventManager.InvokeEvent(UIEvents.SelectItem, itemStack.Data);
    } 


}
