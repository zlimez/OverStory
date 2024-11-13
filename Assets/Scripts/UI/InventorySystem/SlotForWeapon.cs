using Abyss.EventSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotForWeapon : MonoBehaviour
{
    public WeaponItem item;
    public Image itemIcon;
    public GameObject backgroundPanel;

    public GameObject tooltipPanel; // Reference to the tooltip panel
    public TextMeshProUGUI tooltipText;

    public Button slotButton;
    public GameObject contextMenu; // The context menu UI (to display options like Equip, Discard)
    public GameObject ButtonPrefab;
    public EventTrigger eventTrigger;

    void Start()
    {
        slotButton.onClick.AddListener(ShowContextMenu);
        CloseTooltip();
        CloseContextMenu();
    }

    void OnEnable()
    {
        if (GameManager.Instance == null)
            EventManager.StartListening(SystemEvents.SystemsReady, InitUpdateWeapon);
        else
        {
            item = GameManager.Instance.PlayerPersistence.WeaponItem;
            UpdateSlot();
            EventManager.StartListening(PlayEvents.WeaponEquipped, EquipWeapon);
        }
    }

    void InitUpdateWeapon(object input = null)
    {
        item = GameManager.Instance.PlayerPersistence.WeaponItem;
        UpdateSlot();
        EventManager.StartListening(PlayEvents.WeaponEquipped, EquipWeapon);
        EventManager.StopListening(SystemEvents.SystemsReady, InitUpdateWeapon);
    }
    void OnDisable()
    {
        EventManager.StopListening(PlayEvents.WeaponEquipped, EquipWeapon);
    }

    public void ShowTooltip()
    {
        tooltipPanel.SetActive(true);
        // need to change
        tooltipText.text = item.itemName + ":\n\n" + item.description;
        //
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

        AddOptionToContextMenu("Unequip", UnequipItem);
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

    // Method to unequip the item (for Weapon)
    void UnequipItem()
    {
        Debug.Log("Unequipping " + item.itemName);
        EventManager.InvokeEvent(PlayEvents.WeaponUnequipped, item);
        item = null;
        UpdateSlot();
        CloseContextMenu();
    }

    void EquipWeapon(object input)
    {
        item = (WeaponItem) input;
        UpdateSlot();
    }

    void UpdateSlot()
    {
        CloseTooltip();
        CloseContextMenu();
        if (item == null)
        {
            itemIcon.gameObject.SetActive(false);
            slotButton.interactable = false;
            eventTrigger.enabled = false;
            
        }
        else
        {
            itemIcon.sprite = item.icon;
            itemIcon.gameObject.SetActive(true);
            slotButton.interactable = true;
            eventTrigger.enabled = true;
        }
    }

    
}
