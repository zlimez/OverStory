using System.Collections.Generic;
using Abyss.EventSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBagUI : MonoBehaviour
{
    public Button ConsumablesButton;
    public Button WeaponsButton;
    public Button MaterialsButton;
    public Button FarmablesButton;
    public GameObject scrollViewContent;
    public GameObject SlotPrefab;
    private Collection playerInventory;

    public Sprite ConsumablesButtonInactive;
    public Sprite ConsumablesButtonActive;
    public Image ConsumablesButtonImage;
    public Sprite WeaponsButtonInactive;
    public Sprite WeaponsButtonActive;
    public Image WeaponsButtonImage;
    public Sprite MaterialsButtonInactive;
    public Sprite MaterialsButtonActive;
    public Image MaterialsButtonImage;
    public Sprite FarmablesButtonInactive;
    public Sprite FarmablesButtonActive;
    public Image FarmablesButtonImage;

    private bool showConsumables = false;
    private bool showWeapons = false;
    private bool showMaterials = false;
    private bool showFarmables = false;

    void Start()
    {
        UpdateBagUI();
    }

    void OnEnable()
    {
        if (GameManager.Instance == null)
            EventManager.StartListening(SystemEventCollection.SystemsReady, InitUpdateBagUI);
        else
        {
            UpdateBagUI();
            GameManager.Instance.Inventory.MaterialCollection.OnItemChanged += UpdateBagUI;
        }
    }

    // NOTE: TO SUPPORT DEV FLOW WHERE BASESCENEMANAGER IS USED TO LOAD MASTER AFTER SCENE IN EDITOR
    void InitUpdateBagUI(object input = null)
    {
        UpdateBagUI();
        GameManager.Instance.Inventory.MaterialCollection.OnItemChanged += UpdateBagUI;
        EventManager.StopListening(SystemEventCollection.SystemsReady, InitUpdateBagUI);
    }

    // void OnDisable() => GameManager.Instance.Inventory.MaterialCollection.OnItemChanged -= UpdateBagUI;

    public void ToggleShowConsumables()
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
    public void ToggleShowWeapons()
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
    public void ToggleShowMaterials()
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
    public void ToggleShowFarmables()
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

    public void UpdateButton()
    {
        if (showConsumables) ConsumablesButtonImage.sprite = ConsumablesButtonActive;
        else ConsumablesButtonImage.sprite = ConsumablesButtonInactive;
        if (showWeapons) WeaponsButtonImage.sprite = WeaponsButtonActive;
        else WeaponsButtonImage.sprite = WeaponsButtonInactive;
        if (showMaterials) MaterialsButtonImage.sprite = MaterialsButtonActive;
        else MaterialsButtonImage.sprite = MaterialsButtonInactive;
        if (showFarmables) FarmablesButtonImage.sprite = FarmablesButtonActive;
        else FarmablesButtonImage.sprite = FarmablesButtonInactive;
    }
    public void UpdateBagUI()
    {
        UpdateButton();

        foreach (Transform child in scrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }

        playerInventory = GameManager.Instance.Inventory.MaterialCollection;
        foreach (var itemStack in playerInventory.Items)
        {
            if (showConsumables && itemStack.Data.itemType != ItemType.Organs && itemStack.Data.itemType != ItemType.Consumables) continue;
            if (showWeapons && itemStack.Data.itemType != ItemType.Weapons) continue;
            if (showMaterials && itemStack.Data.itemType != ItemType.Materials) continue;
            if (showFarmables && itemStack.Data.itemType != ItemType.Farmables) continue;


            if (itemStack.Count <= 0) continue;
            GameObject slot = Instantiate(SlotPrefab, scrollViewContent.transform);

            SlotForTrading slotController = slot.GetComponent<SlotForTrading>();
            if (slotController != null)
            {
                slotController.InitializeSlot(itemStack);
            }
            else
            {
                Debug.LogError("SlotController 组件未找到!");
            }
        }
    }
}
