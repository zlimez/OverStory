using System.Collections.Generic;
using Abyss.EventSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotForSpell : MonoBehaviour
{
    public List<string> KeyBorad;
    public SpellItem item;
    public Image itemIcon;
    public GameObject backgroundPanel;

    public GameObject tooltipPanel; // Reference to the tooltip panel
    public TextMeshProUGUI tooltipText;

    public Button slotButton;
    public GameObject contextMenu; // The context menu UI (to display options like Equip, Discard)
    public GameObject ButtonPrefab;

    private bool[] index = new bool[3];

    // public void InitializeSlot(Countable<Item> newItem, int level)
    // {
    //     itemStack = newItem;
    //     if (itemStack != null)
    //     {
    //         itemIcon.sprite = itemStack.Data.icon;
    //         SlotBGImage.sprite = SlotBG[level - 1];
    //     }
    // }
    void Start()
    {
        slotButton.onClick.AddListener(ShowContextMenu);
        CloseTooltip();
        CloseContextMenu();
    }

    public void ShowTooltip()
    {
        tooltipPanel.SetActive(true);
        tooltipText.text = item.description;
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

        FindSpellItemIndex();
        if (!index[0]) AddOptionToContextMenu(KeyBorad[0], Equip0);
        if (!index[1]) AddOptionToContextMenu(KeyBorad[1], Equip1);
        if (!index[2]) AddOptionToContextMenu(KeyBorad[2], Equip2);
        if (index[0] || index[1] || index[2]) AddOptionToContextMenu("Unequip", Unequip);
    }

    public void FindSpellItemIndex()
    {
        SpellItem[] SpellItems = GameManager.Instance.PlayerPersistence.SpellItems;
        for (int i = 0; i < SpellItems.Length; i++)
        {
            if (SpellItems[i] == item) index[i] = true;
            else index[i] = false;
            
        }
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

    void Equip0()
    {
        Debug.Log("Equip " + item.itemName + " to " + KeyBorad[0]);
        GameManager.Instance.PlayerPersistence.SpellItems[0] = item;
        EventManager.InvokeEvent(PlayEvents.SpellEquippedStateChange);
        CloseContextMenu();
    }
    void Equip1()
    {
        Debug.Log("Equip " + item.itemName + " to " + KeyBorad[1]);
        GameManager.Instance.PlayerPersistence.SpellItems[1] = item;
        EventManager.InvokeEvent(PlayEvents.SpellEquippedStateChange);
        CloseContextMenu();
    }
    void Equip2()
    {
        Debug.Log("Equip " + item.itemName + " to " + KeyBorad[2]);
        GameManager.Instance.PlayerPersistence.SpellItems[2] = item;
        EventManager.InvokeEvent(PlayEvents.SpellEquippedStateChange);
        CloseContextMenu();
    }
    void Unequip()
    {
        Debug.Log("Unequipping " + item.itemName);
        for (int i = 0; i < 3; i++)
        {
            if (index[i]) GameManager.Instance.PlayerPersistence.SpellItems[i] = null;
        }
        EventManager.InvokeEvent(PlayEvents.SpellEquippedStateChange);
        CloseContextMenu();
    }

    
}
