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
    [SerializeField] GameObject topSlot, topCover;
    [SerializeField] List<GameObject> bottomSlots = new(2), bottomCover = new(2);
    [SerializeField] GameObject scrollViewContent;
    [SerializeField] GameObject slotPrefab;

    [SerializeField] Button learnButton;
    [SerializeField] Sprite learnButtonInactive, learnButtonActive;
    [SerializeField] Image learnButtonImage;

    [SerializeField] Collection npcBag;

    public bool IsLearningOpen { get; private set; } = false;
    public Tribe Tribe => tribe;

    BlueprintItem _chosenBlueprint;

    void Start() => learningPanel.SetActive(false);

    void OnEnable() => EventManager.StartListening(PlayEvents.LearningPostEntered, OpenLearning);
    void OnDisable() => EventManager.StopListening(PlayEvents.LearningPostEntered, OpenLearning);

    public void CloseLearning()
    {
        Stop();
        EventManager.StopListening(UIEvents.SelectItem, Select);
        GameManager.Instance.UI.Close();
    }

    void Stop()
    {
        _chosenBlueprint = null;
        IsLearningOpen = false;
        learningPanel.SetActive(false);
    }

    public void OpenLearning(object input)
    {
        (Tribe tribe, Collection itemCollection) = ((Tribe, Collection))input;
        if (Tribe != tribe) return;
        if (!GameManager.Instance.UI.Open(UiController.Type.Learn, Stop)) return;

        npcBag = itemCollection;
        _chosenBlueprint = null;
        UpdateLearningPanel();
        IsLearningOpen = true;
        learningPanel.SetActive(true);
        EventManager.StartListening(UIEvents.SelectItem, Select);
    }

    private void Select(object arg)
    {
        _chosenBlueprint = (BlueprintItem)arg;
        UpdateLearningArea();
    }

    private void UpdateLearningPanel()
    {
        UpdateNPCBag();
        UpdateLearningArea();

    }

    private void UpdateLearningArea()
    {
        bool canLearn = true;
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
            canLearn = false;
        }
        topCover.SetActive(false);

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

                // check the prerequisites
                List<Item> prerequisiteItems = _chosenBlueprint.prerequisiteItems;
                PlayerAttr prerequisiteAttr = _chosenBlueprint.prerequisiteAttr;
                foreach (var preItem in prerequisiteItems)
                {
                    if (!GameManager.Instance.Inventory.MaterialCollection.Contains(preItem)) canLearn = false;
                }
                if (!prerequisiteAttr.IsLessThanOrEqual(GameManager.Instance.PlayerPersistence.PlayerAttr)) canLearn = false;
                topCover.SetActive(!canLearn);
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
                        canLearn = false;
                        bottomCover[i].SetActive(true);
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
        SetLearnButton(canLearn);
    }

    private void UpdateNPCBag()
    {
        foreach (Transform child in scrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var itemStack in npcBag.Items)
        {
            if (itemStack.Count <= 0) continue;
            GameObject slot = Instantiate(slotPrefab, scrollViewContent.transform);

            if (slot.TryGetComponent<SlotForLearning>(out var slotController)) slotController.InitializeSlot(itemStack);
            else Debug.LogError("SlotController 组件未找到!");
        }
    }


    public void LearnOnClick()
    {
        Item objectItem = _chosenBlueprint.objectItem;
        string msg = "You learned " + objectItem.itemType.ToString() + ": \"" + objectItem.itemName + "\".";
        EventManager.InvokeEvent(PlayEvents.Message, msg);

        List<RefPair<Item, int>> materials = _chosenBlueprint.materials;
        foreach (var itemStock in materials) GameManager.Instance.Inventory.MaterialCollection.RemoveStock(itemStock.Head, itemStock.Tail);
        GameManager.Instance.Inventory.MaterialCollection.Add(objectItem);
        npcBag.Remove(_chosenBlueprint);
        if (_chosenBlueprint.objectItem.itemType == ItemType.Spells) _chosenBlueprint = null;
        UpdateLearningPanel();
    }

    private void SetLearnButton(bool state)
    {
        learnButtonImage.sprite = state ? learnButtonActive : learnButtonInactive;
        learnButton.GetComponent<Button>().interactable = state;
    }
}



