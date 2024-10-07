using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BagUI : MonoBehaviour
{
    public Button ConsumablesButton;
    public Button WeaponsButton;
    public Button MaterialsButton;
    public Button FarmablesButton;
    public GameObject scrollViewContent;
    public GameObject SlotPrefab;
    private Collection playerInventory; 

    public int level = 1; 
    public Sprite[] ConsumablesButtonInactive;
    public Sprite[] ConsumablesButtonActive;
    public Image ConsumablesButtonImage; 
    public Sprite[] WeaponsButtonInactive;
    public Sprite[] WeaponsButtonActive;
    public Image WeaponsButtonImage; 
    public Sprite[] MaterialsButtonInactive;
    public Sprite[] MaterialsButtonActive;
    public Image MaterialsButtonImage; 
    public Sprite[] FarmablesButtonInactive;
    public Sprite[] FarmablesButtonActive;
    public Image FarmablesButtonImage; 

    private bool showConsumables = false;
    private bool showWeapons = false;
    private bool showMaterials = false;
    private bool showFarmables = false;

    void Start()
    {
        ConsumablesButton.onClick.AddListener(ToggleShowConsumables);
        WeaponsButton.onClick.AddListener(ToggleShowWeapons);
        MaterialsButton.onClick.AddListener(ToggleShowMaterials);
        FarmablesButton.onClick.AddListener(ToggleShowFarmables);

        // Initial update to show all items
        UpdateBagUI();
    }

    void OnEnable()
    {
        UpdateBagUI();
        Inventory.Instance.MaterialCollection.onItemChanged += UpdateBagUI;
    }
    void OnDisable()
    {
        Inventory.Instance.MaterialCollection.onItemChanged -= UpdateBagUI;
    }

    void ToggleShowConsumables()
    {
        showConsumables = !showConsumables;
        if (showConsumables)
        {
            showWeapons = false;
            showMaterials = false;
            showFarmables = false;
        }
        UpdateBagUI();
    }
    void ToggleShowWeapons()
    {
        showWeapons = !showWeapons;
        if (showWeapons)
        {
            showConsumables = false;
            showMaterials = false;
            showFarmables = false;
        }
        UpdateBagUI();
    }
    void ToggleShowMaterials()
    {
        showMaterials = !showMaterials;
        if (showMaterials)
        {
            showConsumables = false;
            showWeapons = false;
            showFarmables = false;
        }
        UpdateBagUI();
    }
    void ToggleShowFarmables()
    {
        showFarmables = !showFarmables;
        if (showFarmables)
        {
            showConsumables = false;
            showWeapons = false;
            showMaterials = false;
        }
        UpdateBagUI();
    }

    public void UpdateLevel(int le)
    {
        level = le;
        UpdateBagUI();
    }
    public void UpdateButton()
    {
        if(showConsumables) ConsumablesButtonImage.sprite = ConsumablesButtonActive[level-1];
        else ConsumablesButtonImage.sprite = ConsumablesButtonInactive[level-1];
        if(showWeapons) WeaponsButtonImage.sprite = WeaponsButtonActive[level-1];
        else WeaponsButtonImage.sprite = WeaponsButtonInactive[level-1];
        if(showMaterials) MaterialsButtonImage.sprite = MaterialsButtonActive[level-1];
        else MaterialsButtonImage.sprite = MaterialsButtonInactive[level-1];
        if(showFarmables) FarmablesButtonImage.sprite = FarmablesButtonActive[level-1];
        else FarmablesButtonImage.sprite = FarmablesButtonInactive[level-1];
    }
    public void UpdateBagUI()
    {
        UpdateButton();

        foreach (Transform child in scrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }

        playerInventory = Inventory.Instance.MaterialCollection;
        foreach (var itemStack in playerInventory.Items)
        {
            if (showConsumables && itemStack.Data.itemType != ItemType.Organs && itemStack.Data.itemType != ItemType.Consumables) continue;
            if (showWeapons && itemStack.Data.itemType != ItemType.Weapons) continue;
            if (showMaterials && itemStack.Data.itemType != ItemType.Materials) continue;
            if (showFarmables && itemStack.Data.itemType != ItemType.Farmables) continue;


            if (itemStack.Count <= 0) continue;
            GameObject slot = Instantiate(SlotPrefab, scrollViewContent.transform);
            
            SlotController slotController = slot.GetComponent<SlotController>();
            if (slotController != null)
            {
                slotController.InitializeSlot(itemStack, level);
            }
            else
            {
                Debug.LogError("SlotController 组件未找到!");
            }
        }
    }
}
