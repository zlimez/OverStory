using System;
using System.Collections.Generic;
using Abyss.EventSystem;
using Abyss.Player;
using TMPro;
using Tuples;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour
{
    [SerializeField] GameObject craftingPanel;
    [SerializeField] GameObject topSlot, topCover;
    [SerializeField] List<GameObject> bottomSlots = new(2), bottomCover = new(2);
    [SerializeField] GameObject scrollViewContent;
    [SerializeField] GameObject slotPrefab;

    [SerializeField] Button craftButton;
    [SerializeField] Sprite craftButtonInactive, craftButtonActive;
    [SerializeField] Image craftButtonImage;


    public bool IsCraftingOpen { get; private set; } = false;

    BlueprintItem _chosenBlueprint;

    void Start() => craftingPanel.SetActive(false);

    void OnEnable() => EventManager.StartListening(PlayEvents.CraftingPostEntered, OpenCrafting);
    void OnDisable() => EventManager.StopListening(PlayEvents.CraftingPostEntered, OpenCrafting);

    public void CloseCrafting()
    {
        Stop();
        EventManager.StopListening(UIEvents.SelectItem, Select);
        GameManager.Instance.UI.Close();
    }

    void Stop()
    {
        _chosenBlueprint = null;
        IsCraftingOpen = false;
        craftingPanel.SetActive(false);
    }

    public void OpenCrafting(object input)
    {
        if (!GameManager.Instance.UI.Open(UiController.Type.Craft, Stop)) return;
        _chosenBlueprint = null;
        UpdateCraftingPanel();
        IsCraftingOpen = true;
        craftingPanel.SetActive(true);
        EventManager.StartListening(UIEvents.SelectItem, Select);
    }

    private void Select(object arg)
    {
        _chosenBlueprint = (BlueprintItem)arg;
        UpdateCraftingArea();
    }

    private void UpdateCraftingPanel()
    {
        UpdateBag();
        UpdateCraftingArea();

    }

    private void UpdateCraftingArea()
    {
        bool canCraft = true;
        topCover.SetActive(false);
        Item topItem;
        List<RefPair<Item, int>> materials;
        if (_chosenBlueprint != null)
        {
            topItem = _chosenBlueprint.objectItem;
            materials = _chosenBlueprint.materials;
        }
        else
        {
            topItem = null;
            materials = null;
            canCraft = false;
        }

        // TopSlot
        Transform spriteObject = topSlot.transform.Find("ItemIcon");
        if (spriteObject != null)
        {
            Image nestedImage = spriteObject.GetComponent<Image>();
            if (topItem != null)
            {
                RectTransform iconRectTransform = nestedImage.GetComponent<RectTransform>();
                if (topItem.itemType == ItemType.Spells) iconRectTransform.sizeDelta = new Vector2(42.49596f, 42.49596f);
                else iconRectTransform.sizeDelta = new Vector2(25.0f, 25.0f);

                nestedImage.sprite = topItem.icon;
                nestedImage.gameObject.SetActive(true);
            }
            else nestedImage.gameObject.SetActive(false);
        }
        Transform textObject = topSlot.transform.Find("acount");
        if (textObject != null)
        {
            TextMeshProUGUI nestedText = textObject.GetComponent<TextMeshProUGUI>();
            nestedText.text = "";
        }
        if (topItem != null && topSlot.TryGetComponent<SlotForLearning>(out var slotForLearning))
        {
            Countable<Item> newItemStack = new(topItem, 1);
            slotForLearning.itemStack = newItemStack;
        }


        // Bottom Slots
        if (materials != null && materials.Count > bottomSlots.Count) Debug.LogError("Do not have enough Bottom Slots!");
        for (int i = 0; i < bottomSlots.Count; i++)
        {
            RefPair<Item, int> item;
            if (materials == null || i >= materials.Count) item = null;
            else item = materials[i];

            Transform bottomSpriteObject = bottomSlots[i].transform.Find("ItemIcon");
            if (bottomSpriteObject != null)
            {
                Image nestedImage = bottomSpriteObject.GetComponent<Image>();
                if (item != null)
                {
                    nestedImage.sprite = item.Head.icon;
                    nestedImage.gameObject.SetActive(true);
                }
                else nestedImage.gameObject.SetActive(false);
            }

            Transform bottomTextObject = bottomSlots[i].transform.Find("acount");
            if (bottomTextObject != null)
            {
                TextMeshProUGUI nestedText = bottomTextObject.GetComponent<TextMeshProUGUI>();
                if (item != null)
                {
                    int needCount = item.Tail;
                    int haveCount = GameManager.Instance.Inventory.MaterialCollection.StockOf(item.Head);
                    if (haveCount < needCount)
                    {
                        canCraft = false;
                        bottomCover[i].SetActive(true);
                        topCover.SetActive(true);
                    }
                    else bottomCover[i].SetActive(false);
                    nestedText.text = haveCount.ToString() + "/" + needCount.ToString();
                }
                else
                {
                    bottomCover[i].SetActive(false);
                    nestedText.text = "";
                }
            }

            if (item != null && bottomSlots[i].TryGetComponent<SlotForLearning>(out var bottomSlotForLearning))
            {
                Countable<Item> newItemStack = new(item.Head, 1);
                bottomSlotForLearning.itemStack = newItemStack;
            }

        }

        // Buttom
        SetCraftButton(canCraft);
    }

    private void UpdateBag()
    {
        foreach (Transform child in scrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var itemStack in GameManager.Instance.Inventory.MaterialCollection.Items)
        {
            if (itemStack.Data.itemType != ItemType.Blueprints) continue;
            // if (itemStack.Data is BlueprintItem derivedData && derivedData.objectItem.itemType == ItemType.Constructions) continue;
            if (itemStack.Count <= 0) continue;
            GameObject slot = Instantiate(slotPrefab, scrollViewContent.transform);

            SlotForLearning slotController = slot.GetComponent<SlotForLearning>();
            if (slotController != null) slotController.InitializeSlot(itemStack);
            else Debug.LogError("SlotController 组件未找到!");
        }
    }


    public void CraftOnClick()
    {
        Item objectItem = _chosenBlueprint.objectItem;
        List<RefPair<Item, int>> materials = _chosenBlueprint.materials;
        foreach (var itemStock in materials) GameManager.Instance.Inventory.MaterialCollection.RemoveStock(itemStock.Head, itemStock.Tail);
        GameManager.Instance.Inventory.MaterialCollection.Add(objectItem);
        if (_chosenBlueprint.objectItem.itemType == ItemType.Spells) _chosenBlueprint = null;
        UpdateCraftingPanel();
    }

    private void SetCraftButton(bool state)
    {
        craftButtonImage.sprite = state ? craftButtonActive : craftButtonInactive;
        craftButton.GetComponent<Button>().interactable = state;
    }

}



