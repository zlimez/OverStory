using Abyss.EventSystem;
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
        UpdateBagUI();
    }

    void OnEnable()
    {
        if (GameManager.Instance == null)
            EventManager.StartListening(SystemEvents.SystemsReady, InitUpdateBagUI);
        else
        {
            level = GameManager.Instance.Inventory.Level;
            UpdateBagUI();
            GameManager.Instance.Inventory.MaterialCollection.OnItemChanged += UpdateBagUI;
        }
    }

    // NOTE: TO SUPPORT DEV FLOW WHERE BASESCENEMANAGER IS USED TO LOAD MASTER AFTER SCENE IN EDITOR
    void InitUpdateBagUI(object input = null)
    {
        level = GameManager.Instance.Inventory.Level;
        UpdateBagUI();
        GameManager.Instance.Inventory.MaterialCollection.OnItemChanged += UpdateBagUI;
        EventManager.StopListening(SystemEvents.SystemsReady, InitUpdateBagUI);
    }

    void OnDisable()
    {
        if (GameManager.Instance != null) GameManager.Instance.Inventory.MaterialCollection.OnItemChanged -= UpdateBagUI;
    }

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

    public void UpdateLevel(int le)
    {
        level = le;
        UpdateBagUI();
    }
    public void UpdateButton()
    {
        if (showConsumables) ConsumablesButtonImage.sprite = ConsumablesButtonActive[level - 1];
        else ConsumablesButtonImage.sprite = ConsumablesButtonInactive[level - 1];
        if (showWeapons) WeaponsButtonImage.sprite = WeaponsButtonActive[level - 1];
        else WeaponsButtonImage.sprite = WeaponsButtonInactive[level - 1];
        if (showMaterials) MaterialsButtonImage.sprite = MaterialsButtonActive[level - 1];
        else MaterialsButtonImage.sprite = MaterialsButtonInactive[level - 1];
        if (showFarmables) FarmablesButtonImage.sprite = FarmablesButtonActive[level - 1];
        else FarmablesButtonImage.sprite = FarmablesButtonInactive[level - 1];
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
            if (itemStack.Data.itemType != ItemType.Organs && itemStack.Data.itemType != ItemType.Consumables && itemStack.Data.itemType != ItemType.Weapons && itemStack.Data.itemType != ItemType.Materials && itemStack.Data.itemType != ItemType.Farmables && itemStack.Data.itemType != ItemType.QuestItems) continue;
            if (showConsumables && itemStack.Data.itemType != ItemType.Organs && itemStack.Data.itemType != ItemType.Consumables) continue;
            if (showWeapons && itemStack.Data.itemType != ItemType.Weapons) continue;
            if (showMaterials && itemStack.Data.itemType != ItemType.Materials) continue;
            if (showFarmables && itemStack.Data.itemType != ItemType.Farmables) continue;


            if (itemStack.Count <= 0) continue;
            GameObject slot = Instantiate(SlotPrefab, scrollViewContent.transform);

            SlotController slotController = slot.GetComponent<SlotController>();
            if (slotController != null)
                slotController.InitializeSlot(itemStack, level);
            else Debug.LogError("SlotController 组件未找到!");
        }
    }
}
