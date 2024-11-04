using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LargeSlotController : MonoBehaviour
{
    public ConstructionItem blueprintItem;
    // public Item item;
    public Image itemIcon;
    public TextMeshProUGUI name;
    public TextMeshProUGUI description;
    public List<Image> materialIcons;
    public List<TextMeshProUGUI> materialCounts;

    public Sprite[] SlotBG;
    public Image SlotBGImage;

    // public GameObject tooltipPanel;
    // public TextMeshProUGUI tooltipText;

    public void InitializeSlot(Item newItem, int level)
    {
        if (newItem is ConstructionItem derivedData && derivedData.itemType == ItemType.Constructions)
        {
            blueprintItem = derivedData;
            itemIcon.sprite = blueprintItem.icon;
            name.text = blueprintItem.name;
            description.text = blueprintItem.description;
            for (int i = 0; i < materialIcons.Count; i++)
            {
                if (i >= blueprintItem.materials.Count)
                {
                    materialIcons[i].gameObject.SetActive(false);
                    materialCounts[i].text = "";
                }
                else
                {
                    materialIcons[i].sprite = blueprintItem.materials[i].Head.icon;
                    materialIcons[i].gameObject.SetActive(true);
                    materialCounts[i].text = "x" + blueprintItem.materials[i].Tail.ToString();
                }
            }
            SlotBGImage.sprite = SlotBG[level - 1];
        }
        else return;
    }
    // void Start()
    // {
    //     slotButton.onClick.AddListener(ShowContextMenu);
    //     CloseTooltip();
    // }

    // public void ShowTooltip()
    // {
    //     tooltipPanel.SetActive(true);
    //     tooltipText.text = itemStack.Data.description;
    // }

    // public void CloseTooltip()
    // {
    //     tooltipPanel.SetActive(false);
    // }
}
