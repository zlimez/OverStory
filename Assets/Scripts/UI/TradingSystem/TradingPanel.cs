using System;
using System.Collections.Generic;
using Abyss.EventSystem;
using Abyss.Player;
using TMPro;
using Tuples;
using UnityEngine;
using UnityEngine.UI;

public class TradingSystem : MonoBehaviour
{
    [SerializeField] Tribe tribe;
    [SerializeField] GameObject tradingPanel;
    [SerializeField] TradingArea topArea = new(1);
    [SerializeField] TradingArea bottomArea = new(5);
    [SerializeField] GameObject playerSlots;
    [SerializeField] GameObject npcSlots;
    [SerializeField] GameObject topSlot;
    [SerializeField] GameObject bottomSlots;
    [SerializeField] GameObject scrollViewContent;
    [SerializeField] GameObject slotPrefab;
    [SerializeField] Image valueBarImage;

    [SerializeField] Button bargainButton;
    [SerializeField] Button tradeButton;
    [SerializeField] Sprite bargainButtonInactive;
    [SerializeField] Sprite bargainButtonActive;
    [SerializeField] Image bargainButtonImage;
    [SerializeField] Sprite tradeButtonInactive;
    [SerializeField] Sprite tradeButtonActive;
    [SerializeField] Image tradeButtonImage;

    [SerializeField] NPCBagUI npcBagUI;

    public bool IsTradingOpen { get; private set; } = false;
    public Tribe Tribe => tribe;

    // private readonly bool bargainButtonState = false;
    // private readonly bool tradeButtonState = false;
    private bool BargainFailed = false;

    PlayerAttr _playerAttr;
    Tribe _tribe;

    void Start() => tradingPanel.SetActive(false);

    void OnEnable() => EventManager.StartListening(PlayEvents.TradePostEntered, OpenTrading);
    void OnDisable() => EventManager.StopListening(PlayEvents.TradePostEntered, OpenTrading);

    public void CloseTrading()
    {
        ClearArea();
        UpdateTradingArea();
        IsTradingOpen = false;
        tradingPanel.SetActive(false);
        EventManager.StopListening(UIEvents.DraggedItem, DragEnd);
    }

    public void OpenTrading(object input)
    {
        (Tribe tribe, PlayerAttr playerAttr, Collection itemCollection) = ((Tribe, PlayerAttr, Collection))input;
        if (Tribe != tribe) return;

        npcBagUI.Init(itemCollection);
        _playerAttr = playerAttr;
        _tribe = tribe;
        ClearArea();
        UpdateTradingArea();
        IsTradingOpen = true;
        tradingPanel.SetActive(true);
        EventManager.StartListening(UIEvents.DraggedItem, DragEnd);
    }

    void DragEnd(object args)
    {
        if (args is DragEventArgs dragArgs)
        {
            Vector2 originalPosition = dragArgs.OriginalPosition;
            Vector2 destinationPosition = dragArgs.DestinationPosition;
            Item item = dragArgs.Item;

            AreaType original = Position2AreaType(originalPosition);
            AreaType destination = Position2AreaType(destinationPosition);

            Debug.Log($"Item dragged from {original} to {destination}");

            if (IsValidDarg(original, destination)) ChangeItemPosition(original, destination, item);
            else Debug.Log("Invalid Darg.");
        }
        else Debug.LogWarning("Event args are not of type DragEventArgs.");
    }

    private void ChangeItemPosition(AreaType original, AreaType destination, Item item)
    {
        Debug.Log("Item changed.");

        if (destination == AreaType.Top)
        {
            if (topArea.tag != original) ClearArea();
            if (topArea.tag == AreaType.None) MarkTopArea(original);

            if (original == AreaType.Player) ChangePlayerInventory(item, -1);
            else if (original == AreaType.NPC) ChangeNPCInventory(item, -1);
            topArea.AddItem(item);
        }
        else if (original == AreaType.Top)
        {
            if (destination == topArea.tag)
            {
                topArea.RemoveItem(item);
                if (destination == AreaType.Player) ChangePlayerInventory(item, 1);
                else if (destination == AreaType.NPC) ChangeNPCInventory(item, 1);
            }
        }
        else if (destination == AreaType.Bottom)
        {
            if ((_tribe == Tribe.Fara && item.isAcceptableToFara) || (_tribe == Tribe.Hakem && item.isAcceptableToHakem))
            {
                if (bottomArea.tag == AreaType.None) MarkBottomArea(original);
                if (bottomArea.tag == original)
                {
                    if (original == AreaType.Player) ChangePlayerInventory(item, -1);
                    else if (original == AreaType.NPC) ChangeNPCInventory(item, -1);
                    bottomArea.AddItem(item);
                }
            }
        }
        else if (original == AreaType.Bottom)
        {
            if (bottomArea.tag == destination)
            {
                bottomArea.RemoveItem(item);
                if (destination == AreaType.Player) ChangePlayerInventory(item, 1);
                else if (destination == AreaType.NPC) ChangeNPCInventory(item, 1);
            }
        }
        UpdateTradingArea();
    }

    private void UpdateTradingArea()
    {
        //TopSlot
        Transform spriteObject = topSlot.transform.Find("ItemIcon");
        if (spriteObject != null)
        {
            Image nestedImage = spriteObject.GetComponent<Image>();
            if (topArea.Items.Count > 0)
            {
                nestedImage.sprite = topArea.Items[0].Head.icon;
                nestedImage.gameObject.SetActive(true);
            }
            else nestedImage.gameObject.SetActive(false);
        }
        Transform textObject = topSlot.transform.Find("acount");
        if (textObject != null)
        {
            TextMeshProUGUI nestedText = textObject.GetComponent<TextMeshProUGUI>();
            if (topArea.Items.Count > 0) nestedText.text = topArea.Items[0].Tail.ToString();
            else nestedText.text = "";
        }
        if (topSlot.TryGetComponent<SlotForTrading>(out var slotForTrading))
        {
            if (topArea.Items.Count > 0)
            {
                Countable<Item> newItemStack = new(topArea.Items[0].Head, topArea.Items[0].Tail);
                slotForTrading.itemStack = newItemStack;
            }
            else slotForTrading.itemStack = null;
        }


        //Bottom Slots
        foreach (Transform child in scrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var itemStack in bottomArea.Items)
        {
            if (itemStack.Tail <= 0) continue;
            GameObject slot = Instantiate(slotPrefab, scrollViewContent.transform);

            if (slot.TryGetComponent<SlotForTrading>(out var slotController))
            {
                Countable<Item> newItemStack = new(itemStack.Head, itemStack.Tail);
                slotController.InitializeSlot(newItemStack);
            }
        }

        //Value Bar
        //1:0.85
        float topVal = topArea.TotalValue(_tribe);
        float bottomVal = bottomArea.TotalValue(_tribe);
        float proportion = 0;
        if (topVal == 0 || bottomVal == 0) valueBarImage.fillAmount = 0;
        else
        {
            proportion = bottomVal / topVal;
            float propBar = proportion * 0.85f;
            if (propBar > 1) valueBarImage.fillAmount = 1;
            else valueBarImage.fillAmount = propBar;
        }
        //Button
        if (topArea.tag == AreaType.NPC)
        {
            if (proportion == 0)
            {
                SetBargainButton(false);
                SetTradeButton(false);
            }
            else if (proportion < 1)
            {
                SetBargainButton(!BargainFailed);
                SetTradeButton(false);
            }
            else
            {
                SetBargainButton(false);
                SetTradeButton(true);
            }
        }
        else if (topArea.tag == AreaType.Player)
        {
            if (proportion == 0)
            {
                SetBargainButton(false);
                SetTradeButton(false);
            }
            else if (proportion > 1)
            {
                SetBargainButton(!BargainFailed);
                SetTradeButton(false);
            }
            else
            {
                SetBargainButton(false);
                SetTradeButton(true);
            }
        }
        else
        {
            SetBargainButton(false);
            SetTradeButton(false);
        }

    }

    public void BargainOnClick()
    {
        float topVal = topArea.TotalValue(_tribe);
        float bottomVal = bottomArea.TotalValue(_tribe);
        float proportion = bottomVal / topVal;
        // Discount% = purity% - 60%；
        float purity = _playerAttr.Purity / 100f;
        if (topArea.tag == AreaType.NPC)
        {
            if (proportion + purity - 0.6f < 1) BargainFailed = true;
            else SetTradeButton(true);
            SetBargainButton(false);
        }
        else if (topArea.tag == AreaType.Player)
        {
            if (proportion - purity + 0.6f > 1) BargainFailed = true;
            else SetTradeButton(true);
            SetBargainButton(false);
        }

    }
    public void TradeOnClick()
    {
        BargainFailed = false;
        TradeDone();
        UpdateTradingArea();
    }

    private void SetBargainButton(bool state)
    {
        bargainButtonImage.sprite = state ? bargainButtonActive : bargainButtonInactive;
        bargainButton.GetComponent<Button>().interactable = state;

    }
    private void SetTradeButton(bool state)
    {
        tradeButtonImage.sprite = state ? tradeButtonActive : tradeButtonInactive;
        tradeButton.GetComponent<Button>().interactable = state;
    }

    private void ChangePlayerInventory(Item item, int count = 1)
    {
        if (count > 0) GameManager.Instance.Inventory.Add(item, count);
        if (count < 0) GameManager.Instance.Inventory.MaterialCollection.RemoveStock(item, -count);
    }
    private void ChangeNPCInventory(Item item, int count = 1)
    {
        RefPair<Item, int> itemWithCount = new(item, count);
        EventManager.InvokeEvent(UIEvents.UpdateNPCInventory, itemWithCount);
    }

    private void TradeDone()
    {
        topArea.ReverseClear();
        bottomArea.ReverseClear();
    }
    private void ClearArea()
    {
        topArea.Clear();
        bottomArea.Clear();
    }

    private void MarkTopArea(AreaType tag)
    {
        if (tag == AreaType.Player)
        {
            topArea.tag = AreaType.Player;
            bottomArea.tag = AreaType.NPC;
        }
        else if (tag == AreaType.NPC)
        {
            topArea.tag = AreaType.NPC;
            bottomArea.tag = AreaType.Player;
        }
        else Debug.Log("Tag error.");
    }

    private void MarkBottomArea(AreaType tag)
    {
        if (tag == AreaType.Player)
        {
            bottomArea.tag = AreaType.Player;
            topArea.tag = AreaType.NPC;
        }
        else if (tag == AreaType.NPC)
        {
            bottomArea.tag = AreaType.NPC;
            topArea.tag = AreaType.Player;
        }
        else Debug.Log("Tag error.");
    }

    private bool IsValidDarg(AreaType original, AreaType destination)
    {
        if ((original == AreaType.Player || original == AreaType.NPC) && (destination == AreaType.Player || destination == AreaType.NPC))
            return false;
        if ((original == AreaType.Top || original == AreaType.Bottom) && (destination == AreaType.Top || destination == AreaType.Bottom))
            return false;
        return true;
    }

    AreaType Position2AreaType(Vector2 position)
    {
        if (IsWithinBounds(position, topSlot)) return AreaType.Top;
        if (IsWithinBounds(position, bottomSlots)) return AreaType.Bottom;
        if (IsWithinBounds(position, playerSlots)) return AreaType.Player;
        if (IsWithinBounds(position, npcSlots)) return AreaType.NPC;
        return AreaType.None;
    }

    bool IsWithinBounds(Vector2 position, GameObject targetGameObject)
    {
        RectTransform rectTransform = targetGameObject.GetComponent<RectTransform>();
        if (rectTransform == null) return false;

        Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
        Camera cam = null;
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            cam = canvas.worldCamera;

        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, position, cam);
    }

}

public enum AreaType
{
    Player,
    NPC,
    Top,
    Bottom,
    None
}

public class TradingArea
{
    public List<RefPair<Item, int>> Items = new();
    public AreaType tag = AreaType.None;
    public int capability;

    public TradingArea(int cap) => capability = cap;

    public float TotalValue(Tribe npcType)
    {
        float value = 0;
        foreach (var itemStack in Items)
        {
            if (npcType == Tribe.Fara) value += itemStack.Head.valueToFara * itemStack.Tail;
            else if (npcType == Tribe.Hakem) value += itemStack.Head.valueToHakem * itemStack.Tail;
        }

        if (capability == 1 && tag == AreaType.Player) value /= 2.0f;
        return value;
    }

    public void AddItem(Item item, int quantity = 1)
    {
        var existingItem = Items.Find(x => x.Head == item);
        if (existingItem != null)
        {
            existingItem.Tail += quantity;
            return;
        }
        if (Items.Count >= capability)
        {
            var lastItem = Items[^1];

            if (tag == AreaType.Player) GameManager.Instance.Inventory.Add(lastItem.Head, lastItem.Tail);
            else if (tag == AreaType.NPC) EventManager.InvokeEvent(UIEvents.UpdateNPCInventory, lastItem);
            Items.RemoveAt(Items.Count - 1);
        }
        RefPair<Item, int> itemStack = new(item, quantity);
        Items.Add(itemStack);

    }

    public void RemoveItem(Item item, int quantity = 1)
    {
        var existingItem = Items.Find(x => x.Head == item);
        if (existingItem != null)
        {
            existingItem.Tail -= quantity;
            if (existingItem.Tail <= 0)
                Items.Remove(existingItem);
        }
    }

    public void Clear()
    {
        foreach (var item in Items)
        {
            if (tag == AreaType.Player) GameManager.Instance.Inventory.Add(item.Head, item.Tail);
            else if (tag == AreaType.NPC) EventManager.InvokeEvent(UIEvents.UpdateNPCInventory, item);
        }
        Items.Clear();
        tag = AreaType.None;
    }

    public void ReverseClear()
    {
        foreach (var item in Items)
        {
            if (tag == AreaType.NPC) GameManager.Instance.Inventory.Add(item.Head, item.Tail);
            else if (tag == AreaType.Player) EventManager.InvokeEvent(UIEvents.UpdateNPCInventory, item);
        }
        Items.Clear();
        tag = AreaType.None;
    }

}

[Serializable]
public enum Tribe
{
    Fara,
    Hakem
}
