using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotControllerNoMenu : MonoBehaviour
{
    public Countable<Item> itemStack;
    public Image itemIcon;
    public TextMeshProUGUI itemCount;


    public GameObject tooltipPanel; // Reference to the tooltip panel
    public TextMeshProUGUI tooltipText;

    public Button slotButton;

    public void InitializeSlot(Countable<Item> newItem)
    {
        itemStack = newItem;
        if (itemStack != null)
        {
            itemIcon.sprite = itemStack.Data.icon;
            itemCount.text = itemStack.Count.ToString();
        }
    }
    void Start()
    {
        CloseTooltip();
    }

    public void ShowTooltip()
    {
        tooltipPanel.SetActive(true);
        tooltipText.text = itemStack.Data.description;
    }

    public void CloseTooltip()
    {
        tooltipPanel.SetActive(false);
    }


}
