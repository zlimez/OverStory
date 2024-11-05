using System.Collections.Generic;
using Abyss.EventSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JournalUI : MonoBehaviour
{
    public GameObject scrollViewContent;
    public GameObject SlotPrefab;
    private Collection playerInventory;

    public int level = 1;
    public Sprite[] BestiaryButtonInactive;
    public Sprite[] BestiaryButtonActive;
    public Image BestiaryButtonImage;
    public Sprite[] TribeButtonInactive;
    public Sprite[] TribeButtonActive;
    public Image TribeButtonImage;
    private bool showBestiary = false;
    private bool showTribe = false;

    void Start()
    {
        UpdateJournalUI();
    }

    void OnEnable()
    {
        if (GameManager.Instance == null)
            EventManager.StartListening(SystemEvents.SystemsReady, InitUpdateJournalUI);
        else
        {
            level = GameManager.Instance.Inventory.Level;
            UpdateJournalUI();
            GameManager.Instance.Inventory.MaterialCollection.OnItemChanged += UpdateJournalUI;
        }
    }

    // NOTE: TO SUPPORT DEV FLOW WHERE BASESCENEMANAGER IS USED TO LOAD MASTER AFTER SCENE IN EDITOR
    void InitUpdateJournalUI(object input = null)
    {
        level = GameManager.Instance.Inventory.Level;
        UpdateJournalUI();
        GameManager.Instance.Inventory.MaterialCollection.OnItemChanged += UpdateJournalUI;
        EventManager.StopListening(SystemEvents.SystemsReady, InitUpdateJournalUI);
    }

    void OnDisable() => GameManager.Instance.Inventory.MaterialCollection.OnItemChanged -= UpdateJournalUI;



    public void UpdateLevel(int le)
    {
        level = le;
        UpdateJournalUI();
    }

    public void UpdateJournalUI()
    {
        UpdateButton();

        foreach (Transform child in scrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }

        playerInventory = GameManager.Instance.Inventory.MaterialCollection;
        foreach (var itemStack in playerInventory.Items)
        {
            if (itemStack.Data is JournalItem journalItem)
            {
                if (itemStack.Count <= 0) continue;
                if (showBestiary && journalItem.journalType != JournalItem.JournalType.Bestiary) continue;
                if (showTribe && journalItem.journalType != JournalItem.JournalType.Tribe) continue;
                GameObject slot = Instantiate(SlotPrefab, scrollViewContent.transform);

                LargeSlotController slotController = slot.GetComponent<LargeSlotController>();
                if (slotController != null)
                {
                    slotController.InitializeSlot(itemStack.Data, level);
                }
                else
                {
                    Debug.LogError("LargeSlotController 组件未找到!");
                }
            }
        }
    }

    public void ToggleShowBestiary()
    {
        showBestiary = !showBestiary;
        if (showBestiary) showTribe = false;
        UpdateJournalUI();
    }
    public void ToggleShowTribe()
    {
        showTribe = !showTribe;
        if (showTribe) showBestiary = false;
        UpdateJournalUI();
    }

    public void UpdateButton()
    {
        if (showBestiary) BestiaryButtonImage.sprite = BestiaryButtonActive[level - 1];
        else BestiaryButtonImage.sprite = BestiaryButtonInactive[level - 1];
        if (showTribe) TribeButtonImage.sprite = TribeButtonActive[level - 1];
        else TribeButtonImage.sprite = TribeButtonInactive[level - 1];
    }
}
