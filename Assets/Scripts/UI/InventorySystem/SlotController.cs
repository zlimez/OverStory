using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotController : MonoBehaviour
{
    public Countable<Item> itemStack;
    public Image itemIcon;
    public TextMeshProUGUI itemCount;
    public GameObject backgroundPanel;

    public Sprite[] SlotBG;
    public Image SlotBGImage;

    public GameObject tooltipPanel; // Reference to the tooltip panel
    public TextMeshProUGUI tooltipText;

    public Button slotButton;
    public GameObject contextMenu; // The context menu UI (to display options like Equip, Discard)
    public GameObject ButtonPrefab;

    public void InitializeSlot(Countable<Item> newItem, int level)
    {
        itemStack = newItem;
        if (itemStack != null)
        {
            itemIcon.sprite = itemStack.Data.icon;
            itemCount.text = itemStack.Count.ToString();
            SlotBGImage.sprite = SlotBG[level - 1];
        }
    }
    void Start()
    {
        slotButton.onClick.AddListener(ShowContextMenu);
        CloseTooltip();
        CloseContextMenu();
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

    void ShowContextMenu()
    {
        contextMenu.SetActive(true);
        backgroundPanel.SetActive(true);
        ClearContextMenuOptions();

        // Determine item type and add appropriate options
        if (itemStack.Data.itemType == ItemType.Weapons) AddOptionToContextMenu("Equip", EquipItem);
        if (itemStack.Data.canUseFromInventory) AddOptionToContextMenu("Use", UseItem);
        AddOptionToContextMenu("Discard", DiscardItem);
    }

    public void CloseContextMenu()
    {
        contextMenu.SetActive(false);
        backgroundPanel.SetActive(false);
    }

    void ClearContextMenuOptions()
    {
        foreach (Transform child in contextMenu.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void AddOptionToContextMenu(string optionName, UnityEngine.Events.UnityAction action)
    {
        GameObject newOption = Instantiate(ButtonPrefab, contextMenu.transform);
        TextMeshProUGUI text = newOption.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.text = optionName;
        Button button = newOption.GetComponent<Button>();
        button.onClick.AddListener(action);
    }

    // Method to equip the item (for Weapon)
    void EquipItem()
    {
        Debug.Log("Equipping " + itemStack.Data.itemName);
        GameManager.Instance.Inventory.MaterialCollection.UseItem(itemStack.Data);
        GameManager.Instance.Inventory.MaterialCollection.RemoveStock(itemStack.Data);
        CloseContextMenu();
    }

    // Method to use the item
    void UseItem()
    {
        Debug.Log("Using " + itemStack.Data.itemName);
        GameManager.Instance.Inventory.MaterialCollection.UseItem(itemStack.Data);
        CloseContextMenu();
    }

    // Method to discard the item
    void DiscardItem()
    {
        Debug.Log("Discarding " + itemStack.Data.itemName);
        DropItemInScene(itemStack.Data);
        GameManager.Instance.Inventory.MaterialCollection.RemoveStock(itemStack.Data);
        CloseContextMenu();
    }

    void DropItemInScene(Item item)
    {
        if (item.itemPrefab != null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            Vector3 dropPosition = player.transform.position + player.transform.right * 4f;


            Instantiate(item.itemPrefab, dropPosition, Quaternion.identity);
        }
    }
}
