using System.Collections.Generic;
using Abyss.EventSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpellUI : MonoBehaviour
{
    private Collection playerInventory;
    public Image[] equippedSlots = new Image[3];
    public List<GameObject> spellSlots;


    void Start()
    {
        UpdateSpellUI();
    }

    void OnEnable()
    {
        if (GameManager.Instance == null)
            EventManager.StartListening(SystemEvents.SystemsReady, InitUpdateSpellUI);
        else
        {
            UpdateSpellUI();
            EventManager.StartListening(PlayEvents.SpellEquippedStateChange, UpdateSelectedArea);
        }
    }

    // NOTE: TO SUPPORT DEV FLOW WHERE BASESCENEMANAGER IS USED TO LOAD MASTER AFTER SCENE IN EDITOR
    void InitUpdateSpellUI(object input = null)
    {
        UpdateSpellUI();
        EventManager.StopListening(SystemEvents.SystemsReady, InitUpdateSpellUI);
        EventManager.StartListening(PlayEvents.SpellEquippedStateChange, UpdateSelectedArea);
    }

    void OnDisable() => EventManager.StopListening(PlayEvents.SpellEquippedStateChange, UpdateSelectedArea);

    public void UpdateSpellUI()
    {

        UpdateSelectedArea();
        UpdateSpellTree();
    }

    public void UpdateSelectedArea(object input = null)
    {
        SpellItem[] SpellItems = GameManager.Instance.PlayerPersistence.SpellItems;
        for (int i = 0; i < 3; i++)
        {
            if (SpellItems[i] != null)
            {
                equippedSlots[i].sprite = SpellItems[i].icon;
                equippedSlots[i].gameObject.SetActive(true);
            }
            else equippedSlots[i].gameObject.SetActive(false);
            
        }
    }

    public void UpdateSpellTree()
    {
        playerInventory = GameManager.Instance.Inventory.MaterialCollection;
        foreach (var spellSlot in spellSlots)
        {
            SpellItem spell = spellSlot.GetComponent<SlotForSpell>().item;
            if (spell != null)
            {

                Transform spriteObject = spellSlot.transform.Find("ItemIcon");
                Image icon = spriteObject.GetComponent<Image>();
                if (playerInventory.Contains(spell)) 
                {
                    icon.sprite = spell.icon;
                    spellSlot.GetComponent<Button>().interactable = true;
                    spellSlot.GetComponent<EventTrigger>().enabled = true;
                    // spellSlot.GetComponent<SlotForSpell>().enabled = true;
                }
                else 
                {
                    icon.sprite = spell.iconInactive;
                    spellSlot.GetComponent<Button>().interactable = false;
                    spellSlot.GetComponent<EventTrigger>().enabled = false;
                    // spellSlot.GetComponent<SlotForSpell>().enabled = false;
                }
            }
            else Debug.LogError("Spell is not assigned!");
            
        }
    }
}
