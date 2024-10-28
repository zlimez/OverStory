using System;
using System.Collections.Generic;
using Abyss.EventSystem;
using Abyss.Player;
using TMPro;
using Tuples;
using UnityEngine;
using UnityEngine.UI;

public class LearningSystem : MonoBehaviour
{
    [SerializeField] Tribe tribe;
    [SerializeField] GameObject learningPanel;
    // [SerializeField] TradingArea topArea = new(1);
    // [SerializeField] TradingArea bottomArea = new(5);
    // [SerializeField] GameObject playerSlots;
    // [SerializeField] GameObject npcSlots;
    // [SerializeField] GameObject topSlot;
    // [SerializeField] GameObject bottomSlots;
    // [SerializeField] GameObject scrollViewContent;
    // [SerializeField] GameObject slotPrefab;
    // [SerializeField] Image valueBarImage;

    // [SerializeField] Button tradeButton;
    // [SerializeField] Sprite tradeButtonInactive;
    // [SerializeField] Sprite tradeButtonActive;
    // [SerializeField] Image tradeButtonImage;

    [SerializeField] Collection npcBagUI;

    public bool IsLearningOpen { get; private set; } = false;
    public Tribe Tribe => tribe;

    // private readonly bool bargainButtonState = false;
    // private readonly bool tradeButtonState = false;
    // private bool BargainFailed = false;

    Tribe _tribe;

    void Start() => learningPanel.SetActive(false);

    void OnEnable() => EventManager.StartListening(PlayEvents.LearningPostEntered, OpenLearning);
    void OnDisable() => EventManager.StopListening(PlayEvents.LearningPostEntered, OpenLearning);

    public void CloseLearning()
    {
        // ClearArea();
        // UpdateTradingArea();
        IsLearningOpen = false;
        learningPanel.SetActive(false);
    }

    public void OpenLearning(object input)
    {
        (Tribe tribe, Collection itemCollection) = ((Tribe, Collection))input;
        if (Tribe != tribe) return;

        npcBagUI = itemCollection;
        _tribe = tribe;
        // ClearArea();
        // UpdateTradingArea();
        IsLearningOpen = true;
        learningPanel.SetActive(true);
    }



    // private void UpdateTradingArea()
    // {
    //     //TopSlot
    //     Transform spriteObject = topSlot.transform.Find("ItemIcon");
    //     if (spriteObject != null)
    //     {
    //         Image nestedImage = spriteObject.GetComponent<Image>();
    //         if (topArea.Items.Count > 0)
    //         {
    //             nestedImage.sprite = topArea.Items[0].Head.icon;
    //             nestedImage.gameObject.SetActive(true);
    //         }
    //         else nestedImage.gameObject.SetActive(false);
    //     }
    //     Transform textObject = topSlot.transform.Find("acount");
    //     if (textObject != null)
    //     {
    //         TextMeshProUGUI nestedText = textObject.GetComponent<TextMeshProUGUI>();
    //         if (topArea.Items.Count > 0) nestedText.text = topArea.Items[0].Tail.ToString();
    //         else nestedText.text = "";
    //     }
    //     if (topSlot.TryGetComponent<SlotForTrading>(out var slotForTrading))
    //     {
    //         if (topArea.Items.Count > 0)
    //         {
    //             Countable<Item> newItemStack = new(topArea.Items[0].Head, topArea.Items[0].Tail);
    //             slotForTrading.itemStack = newItemStack;
    //         }
    //         else slotForTrading.itemStack = null;
    //     }


    //     //Bottom Slots
    //     foreach (Transform child in scrollViewContent.transform)
    //     {
    //         Destroy(child.gameObject);
    //     }

    //     foreach (var itemStack in bottomArea.Items)
    //     {
    //         if (itemStack.Tail <= 0) continue;
    //         GameObject slot = Instantiate(slotPrefab, scrollViewContent.transform);

    //         if (slot.TryGetComponent<SlotForTrading>(out var slotController))
    //         {
    //             Countable<Item> newItemStack = new(itemStack.Head, itemStack.Tail);
    //             slotController.InitializeSlot(newItemStack);
    //         }
    //     }

    //     //Value Bar
    //     //1:0.85
    //     float topVal = topArea.TotalValue(_tribe);
    //     float bottomVal = bottomArea.TotalValue(_tribe);
    //     float proportion = 0;
    //     if (topVal == 0 || bottomVal == 0) valueBarImage.fillAmount = 0;
    //     else
    //     {
    //         proportion = bottomVal / topVal;
    //         float propBar = proportion * 0.85f;
    //         if (propBar > 1) valueBarImage.fillAmount = 1;
    //         else valueBarImage.fillAmount = propBar;
    //     }
    //     //Button
    //     if (topArea.tag == AreaType.NPC)
    //     {
    //         if (proportion == 0)
    //         {
    //             SetBargainButton(false);
    //             SetTradeButton(false);
    //         }
    //         else if (proportion < 1)
    //         {
    //             SetBargainButton(!BargainFailed);
    //             SetTradeButton(false);
    //         }
    //         else
    //         {
    //             SetBargainButton(false);
    //             SetTradeButton(true);
    //         }
    //     }
    //     else if (topArea.tag == AreaType.Player)
    //     {
    //         if (proportion == 0)
    //         {
    //             SetBargainButton(false);
    //             SetTradeButton(false);
    //         }
    //         else if (proportion > 1)
    //         {
    //             SetBargainButton(!BargainFailed);
    //             SetTradeButton(false);
    //         }
    //         else
    //         {
    //             SetBargainButton(false);
    //             SetTradeButton(true);
    //         }
    //     }
    //     else
    //     {
    //         SetBargainButton(false);
    //         SetTradeButton(false);
    //     }

    // }

    // public void TradeOnClick()
    // {
    //     BargainFailed = false;
    //     TradeDone();
    //     UpdateTradingArea();
    // }

    // private void SetTradeButton(bool state)
    // {
    //     tradeButtonImage.sprite = state ? tradeButtonActive : tradeButtonInactive;
    //     tradeButton.GetComponent<Button>().interactable = state;
    // }

    // private void ChangePlayerInventory(Item item, int count = 1)
    // {
    //     if (count > 0) GameManager.Instance.Inventory.Add(item, count);
    //     if (count < 0) GameManager.Instance.Inventory.MaterialCollection.RemoveStock(item, -count);
    // }
    // private void ChangeNPCInventory(Item item, int count = 1)
    // {
    //     RefPair<Item, int> itemWithCount = new(item, count);
    //     EventManager.InvokeEvent(UIEvents.UpdateNPCInventory, itemWithCount);
    // }

    // private void TradeDone()
    // {
    //     topArea.ReverseClear();
    //     bottomArea.ReverseClear();
    // }
    // private void ClearArea()
    // {
    //     topArea.Clear();
    //     bottomArea.Clear();
    // }

}



