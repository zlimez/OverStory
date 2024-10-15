using System;
using System.Collections.Generic;
using Abyss.EventSystem;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TradingSystem : MonoBehaviour
{
    public NPCType NPC;
    public GameObject tradingPanel;
    public Button CloseButton;
    private bool isTradingOpen = false;

    public TradingArea TopArea = new TradingArea(1);
    public TradingArea BottomArea= new TradingArea(5);
    public GameObject PlayerSlots;
    public GameObject NPCSlots;
    public GameObject TopSlot;
    public GameObject BottomSlots;
    public GameObject scrollViewContent;
    public GameObject SlotPrefab;
    public Image ValueBarImage;

    public Button BargainButton;
    public Button TradeButton;
    public Sprite BargainButtonInactive;
    public Sprite BargainButtonActive;
    public Image BargainButtonImage;
    public Sprite TradeButtonInactive;
    public Sprite TradeButtonActive;
    public Image TradeButtonImage;

    private bool BargainButtonState = false;
    private bool TradeButtonState = false;
    private bool BargainFailed = false;

    void Start()
    {
        tradingPanel.SetActive(false);
        // UpDateTradingArea();
    }

    void OnEnable()
    {
        EventManager.StartListening(UIEventCollection.DragedItem, DragEnd);
    }

    void OnDisable()
    {
        EventManager.StopListening(UIEventCollection.DragedItem, DragEnd);
    }

    public void ToggleTrading()
    {
        ClearArea();
        UpDateTradingArea();
        isTradingOpen = !isTradingOpen;
        tradingPanel.SetActive(isTradingOpen);
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

            if(isValidDarg(original, destination)) ChangeItemPosition(original, destination, item);
            else Debug.Log("Invalid Darg.");
        }
        else
        {
            Debug.LogWarning("Event args are not of type DragEventArgs.");
        }
    }

    private void ChangeItemPosition(AreaType original, AreaType destination, Item item)
    {
        Debug.Log("Item changed.");

        if(destination == AreaType.Top)
        {
            if(TopArea.tag != original) ClearArea();
            if(TopArea.tag == AreaType.None) MarkTopArea(original);

            if(original == AreaType.Player) changePlayerInventory(item, -1);
            else if(original == AreaType.NPC) changeNPCInventory(item, -1);
            TopArea.AddItem(item);
        }
        else if(original == AreaType.Top)
        {
            if(destination == TopArea.tag)
            {
                TopArea.RemoveItem(item);
                if(destination == AreaType.Player) changePlayerInventory(item, 1);
                else if(destination == AreaType.NPC) changeNPCInventory(item, 1);
            }
        }
        else if(destination == AreaType.Bottom)
        {
            if((NPC == NPCType.Fara && item.isAcceptableToFara) || (NPC == NPCType.Hakem && item.isAcceptableToHakem))
            {
                if(BottomArea.tag == AreaType.None) MarkBottomArea(original);
                if(BottomArea.tag == original)
                {
                    if(original == AreaType.Player) changePlayerInventory(item, -1);
                    else if(original == AreaType.NPC) changeNPCInventory(item, -1);
                    BottomArea.AddItem(item);
                }
            }
        }
        else if(original == AreaType.Bottom)
        {
            if(BottomArea.tag == destination)
            {
                BottomArea.RemoveItem(item);
                if(destination == AreaType.Player) changePlayerInventory(item, 1);
                else if(destination == AreaType.NPC) changeNPCInventory(item, 1);
            }
        }
        UpDateTradingArea();
    }

    private void UpDateTradingArea()
    {
        //TopSlot
        Transform spriteObject = TopSlot.transform.Find("ItemIcon");
        if (spriteObject != null)
        {
            Image nestedImage = spriteObject.GetComponent<Image>();
            if (TopArea.Items.Count > 0)
            {
                nestedImage.sprite = TopArea.Items[0].item.icon;
                nestedImage.gameObject.SetActive(true);
            }
            else nestedImage.gameObject.SetActive(false);
        }
        Transform textObject = TopSlot.transform.Find("acount");
        if (textObject != null)
        {
            TextMeshProUGUI nestedText = textObject.GetComponent<TextMeshProUGUI>();
            if (TopArea.Items.Count > 0) nestedText.text = TopArea.Items[0].count.ToString();
            else nestedText.text = "";
        }
        SlotForTrading slotForTrading = TopSlot.GetComponent<SlotForTrading>();
        if (slotForTrading != null)
        {
            if (TopArea.Items.Count > 0) 
            {
                Countable<Item> newItemStack = new(TopArea.Items[0].item, TopArea.Items[0].count);
                slotForTrading.itemStack = newItemStack;
            }
            else slotForTrading.itemStack = null;
        }
        

        //Bottom Slots
        foreach (Transform child in scrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var itemStack in BottomArea.Items)
        {
            if (itemStack.count <= 0) continue;
            GameObject slot = Instantiate(SlotPrefab, scrollViewContent.transform);

            SlotForTrading slotController = slot.GetComponent<SlotForTrading>();
            if (slotController != null)
            {
                Countable<Item> newItemStack = new(itemStack.item, itemStack.count);
                slotController.InitializeSlot(newItemStack);
            }
        }

        //Value Bar
        //1:0.85
        int topVal = TopArea.totalValue();
        int bottomVal = BottomArea.totalValue();
        float proportion = 0;
        if(topVal == 0 || bottomVal == 0) ValueBarImage.fillAmount = 0;
        else
        {
            proportion = (float)bottomVal / (float)topVal;
            float propBar = proportion * 0.85f;
            if(propBar > 1) ValueBarImage.fillAmount = 1;
            else ValueBarImage.fillAmount = propBar;
        }
        //Button
        if(TopArea.tag == AreaType.NPC)
        {
            if(proportion == 0)
            {
                SetBargainButton(false);
                SetTradeButton(false);
            }
            else if(proportion < 1)
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
        else if(TopArea.tag == AreaType.Player)
        {
            if(proportion == 0)
            {
                SetBargainButton(false);
                SetTradeButton(false);
            }
            else if(proportion > 1)
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
        
    }

    public void BargainOnClick()
    {
        int topVal = TopArea.totalValue();
        int bottomVal = BottomArea.totalValue();
        float proportion = (float)bottomVal / (float)topVal;
        // Discount% = purity% - 60%；
        float purity = GameManager.Instance.PlayerPersistence.PlayerAttr.Purity / 100f;
        if(TopArea.tag == AreaType.NPC)
        {
            if(proportion + purity - 0.6f < 1) BargainFailed = true;
            else SetTradeButton(true);
            SetBargainButton(false);
        }
        else if(TopArea.tag == AreaType.Player)
        {
            if(proportion - purity + 0.6f > 1) BargainFailed = true;
            else SetTradeButton(true);
            SetBargainButton(false);
        }

    }
    public void TradeOnClick()
    {
        BargainFailed = false;
        TradeDone();
        UpDateTradingArea();
    }

    private void SetBargainButton(bool state)
    {
        BargainButtonImage.sprite = state ? BargainButtonActive : BargainButtonInactive;
        BargainButton.GetComponent<Button>().interactable = state;

    }
    private void SetTradeButton(bool state)
    {
        TradeButtonImage.sprite = state ? TradeButtonActive : TradeButtonInactive;
        TradeButton.GetComponent<Button>().interactable = state;
    }

    private void changePlayerInventory(Item item, int count = 1)
    {
        if(count > 0) GameManager.Instance.Inventory.MaterialCollection.Add(item, count);
        if(count < 0) GameManager.Instance.Inventory.MaterialCollection.DiscardItem(item, -count);
    }
    private void changeNPCInventory(Item item, int count = 1)
    {
        ItemWithCount itemWithCount = new(item, count);
        EventManager.InvokeEvent(UIEventCollection.ChangeNPCInventory, itemWithCount);
    }

    private void TradeDone()
    {
        TopArea.ReverseClear();
        BottomArea.ReverseClear();
    }
    private void ClearArea()
    {
        TopArea.Clear();
        BottomArea.Clear();
    }

    private void MarkTopArea(AreaType tag)
    {
        if (tag == AreaType.Player)
        {
            TopArea.tag = AreaType.Player;
            BottomArea.tag = AreaType.NPC;
        }
        else if (tag == AreaType.NPC)
        {
            TopArea.tag = AreaType.NPC;
            BottomArea.tag = AreaType.Player;
        }
        else Debug.Log("Tag error.");
    }

    private void MarkBottomArea(AreaType tag)
    {
        if (tag == AreaType.Player)
        {
            BottomArea.tag = AreaType.Player;
            TopArea.tag = AreaType.NPC;
        }
        else if (tag == AreaType.NPC)
        {
            BottomArea.tag = AreaType.NPC;
            TopArea.tag = AreaType.Player;
        }
        else Debug.Log("Tag error.");
    }

    private bool isValidDarg(AreaType original, AreaType destination)
    {
        if((original == AreaType.Player || original == AreaType.NPC) && (destination == AreaType.Player || destination == AreaType.NPC))
            return false;
        if((original == AreaType.Top || original == AreaType.Bottom) && (destination == AreaType.Top || destination == AreaType.Bottom))
            return false;
        return true;
    }

    AreaType Position2AreaType(Vector2 position)
    {
        if (IsWithinBounds(position, TopSlot)) return AreaType.Top;
        if (IsWithinBounds(position, BottomSlots)) return AreaType.Bottom;
        if (IsWithinBounds(position, PlayerSlots)) return AreaType.Player;
        if (IsWithinBounds(position, NPCSlots)) return AreaType.NPC;
        return AreaType.None;
    }

    bool IsWithinBounds(Vector2 position, GameObject targetGameObject)
    {
        RectTransform rectTransform = targetGameObject.GetComponent<RectTransform>();
        if (rectTransform == null) return false;

        Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
        Camera cam = null;
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            cam = canvas.worldCamera; 
        }

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
    public List<ItemWithCount> Items = new();
    public AreaType tag = AreaType.None;
    public int capability;

    public TradingArea(int cap)
    {
        this.capability = cap;
    }

    public int totalValue()
    {
        int value=0;
        foreach (var itemStack in this.Items)
        {
            value += itemStack.item.value * itemStack.count ;
        }
        if (this.capability == 1 && this.tag == AreaType.Player) value = value / 2;
        return value;
    }

    public void AddItem(Item item, int quantity = 1)
    {
        var existingItem = this.Items.Find(x => x.item == item);
        if (existingItem != null)
        {
            existingItem.count += quantity;
            return;
        }
        if (this.Items.Count >= this.capability)
        {
            ItemWithCount lastItem = this.Items[this.Items.Count - 1];

            if(this.tag == AreaType.Player) GameManager.Instance.Inventory.AddTo(lastItem.item, lastItem.count);
            else if(this.tag == AreaType.NPC)  EventManager.InvokeEvent(UIEventCollection.ChangeNPCInventory, lastItem);
            this.Items.RemoveAt(this.Items.Count - 1);
        }
        ItemWithCount itemStack = new(item, quantity);
        this.Items.Add(itemStack);
            
    }

    public void RemoveItem(Item item, int quantity = 1)
    {
        var existingItem = this.Items.Find(x => x.item == item);
        if (existingItem != null)
        {
            existingItem.count -= quantity;
            if (existingItem.count <= 0)
            {
                this.Items.Remove(existingItem);
            }
            
        } 
    }

    public void Clear()
    {
        foreach (ItemWithCount item in this.Items)
        {
            if(this.tag == AreaType.Player) GameManager.Instance.Inventory.AddTo(item.item, item.count);
            else if(this.tag == AreaType.NPC)  EventManager.InvokeEvent(UIEventCollection.ChangeNPCInventory, item);
        }
        this.Items.Clear();
        this.tag = AreaType.None;
    }

    public void ReverseClear()
    {
        foreach (ItemWithCount item in this.Items)
        {
            if(this.tag == AreaType.NPC) GameManager.Instance.Inventory.AddTo(item.item, item.count);
            else if(this.tag == AreaType.Player)  EventManager.InvokeEvent(UIEventCollection.ChangeNPCInventory, item);
        }
        this.Items.Clear();
        this.tag = AreaType.None;
    }
    
}

public enum NPCType
{
    Fara,
    Hakem
}
