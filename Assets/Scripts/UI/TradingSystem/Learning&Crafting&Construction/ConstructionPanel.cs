using System.Collections;
using System.Collections.Generic;
using Abyss.EventSystem;
using TMPro;
using Tuples;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionSystem : MonoBehaviour
{
    [SerializeField] GameObject constructionPanel;
    [SerializeField] GameObject topSlot, topCover;
    [SerializeField] List<GameObject> bottomSlots = new(2), bottomCover = new(2);

    [SerializeField] Button buildButton;
    [SerializeField] Sprite[] buildInactive, buildActive;
    [SerializeField] Image buildImage;

    [SerializeField] float[] buildTimeMod = new float[3] { 1.0f, 0.75f, 0.6f };
    [SerializeField] Sprite[] backgroundImages;
    [SerializeField] Image constructionBackground;
    [SerializeField] Sprite[] areaImages;
    [SerializeField] Image areaCover;
    [SerializeField] Sprite[] slotImages;
    [SerializeField] Image[] slots;

    [SerializeField] GameObject progressBar;
    [SerializeField] Image barFill;

    ConstructionItem _constructionItem;
    public bool IsPanelOpen { get; private set; } = false;
    public bool IsBuilt = false;
    public bool IsBuilding { get; private set; } = false;
    bool _sufficeToBuild = false;

    public void InitializePanel(ConstructionItem constructionItem, Vector3 position)
    {
        _constructionItem = constructionItem;
        if (_constructionItem != null)
        {
            UpdateConstructionPanel();
            Vector3 adjustedPosition = position + new Vector3(0, -7.0f, 0);
            RectTransform panelRectTransform = constructionPanel.GetComponent<RectTransform>();
            panelRectTransform.position = adjustedPosition;
            progressBar.GetComponent<RectTransform>().position = adjustedPosition + new Vector3(0, 4f, 0);
        }
    }

    public void TryOpenPanel()
    {
        if (IsPanelOpen || IsBuilding || IsBuilt || !GameManager.Instance.Inventory.MaterialCollection.Contains(_constructionItem)) return;
        UpdateConstructionPanel();
        IsPanelOpen = true;
        constructionPanel.SetActive(true);
        EventManager.StartListening(PlayEvents.BuildEnd, SetBuildButton);
    }

    public void ClosePanel()
    {
        constructionPanel.SetActive(false);
        IsPanelOpen = false;
        EventManager.StopListening(PlayEvents.BuildEnd, SetBuildButton);
    }

    private void UpdateConstructionPanel()
    {
        UpdateImage(GameManager.Instance.PlayerPersistence.DroneLevel - 1);

        _sufficeToBuild = true;
        topCover.SetActive(false);
        Item topItem;
        List<RefPair<Item, int>> materials;
        if (_constructionItem != null)
        {
            topItem = _constructionItem;
            materials = _constructionItem.materials;
        }
        else
        {
            topItem = null;
            materials = null;
            _sufficeToBuild = false;
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
                        _sufficeToBuild = false;
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
        SetBuildButton();
    }


    public void TryBuild(Transform buildPt, Transform hoverPt, GameObject before)
    {
        if (!_sufficeToBuild || IsBuilding || GameManager.Instance.PlayerPersistence.IsBuilding || IsBuilt || !GameManager.Instance.Inventory.MaterialCollection.Contains(_constructionItem)) return;
        GameManager.Instance.PlayerPersistence.IsBuilding = true;
        IsBuilding = true;

        List<RefPair<Item, int>> materials = _constructionItem.materials;
        foreach (var itemStock in materials) GameManager.Instance.Inventory.MaterialCollection.RemoveStock(itemStock.Head, itemStock.Tail);

        EventManager.InvokeEvent(PlayEvents.BuildStart, hoverPt); // Currently only consumed by drone
        StartCoroutine(BuildWorks(buildPt, before));
        UpdateConstructionPanel();
        ClosePanel();
    }

    IEnumerator BuildWorks(Transform buildPt, GameObject before)
    {
        progressBar.SetActive(true);

        float elapsedTime = 0, doneTime = _constructionItem.baseBuildTime * buildTimeMod[GameManager.Instance.PlayerPersistence.DroneLevel - 1];
        while (elapsedTime < doneTime)
        {
            elapsedTime += Time.deltaTime;
            barFill.fillAmount = elapsedTime / doneTime;
            yield return null;
        }
        IsBuilding = false;
        IsBuilt = true;
        GameManager.Instance.PlayerPersistence.IsBuilding = false;

        var building = Instantiate(_constructionItem.itemPrefab, buildPt.position, Quaternion.identity);
        building.GetComponent<Construct>().Initialize(this, _constructionItem.Durability);
        building.transform.SetParent(buildPt);

        EventManager.InvokeEvent(PlayEvents.BuildEnd);
        before.SetActive(false);
        progressBar.SetActive(false);
    }

    private void SetBuildButton(object input = null)
    {
        bool canBuild = _sufficeToBuild && !IsBuilding && !GameManager.Instance.PlayerPersistence.IsBuilding;
        buildImage.sprite = canBuild ? buildActive[GameManager.Instance.PlayerPersistence.DroneLevel - 1] : buildInactive[GameManager.Instance.PlayerPersistence.DroneLevel - 1];
        buildButton.GetComponent<Button>().interactable = canBuild;
    }

    void UpdateImage(int order)
    {
        Debug.Log("level: " + order);
        if (order >= 0 && order < backgroundImages.Length)
        {
            constructionBackground.sprite = backgroundImages[order];
            areaCover.sprite = areaImages[order];
            foreach (Image slot in slots) slot.sprite = slotImages[order];
        }
    }
}
