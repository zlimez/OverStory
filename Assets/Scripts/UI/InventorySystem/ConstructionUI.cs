using Abyss.EventSystem;
using UnityEngine;

public class ConstructionUI : MonoBehaviour
{
    public GameObject scrollViewContent;
    public GameObject SlotPrefab;
    private Collection playerInventory;

    public int level = 1;

    void Start() => UpdateConstructionUI();

    void OnEnable()
    {
        if (GameManager.Instance == null)
            EventManager.StartListening(SystemEvents.SystemsReady, InitUpdateConstructionUI);
        else
        {
            level = GameManager.Instance.Inventory.Level;
            UpdateConstructionUI();
            GameManager.Instance.Inventory.MaterialCollection.OnItemChanged += UpdateConstructionUI;
        }
    }

    // NOTE: TO SUPPORT DEV FLOW WHERE BASESCENEMANAGER IS USED TO LOAD MASTER AFTER SCENE IN EDITOR
    void InitUpdateConstructionUI(object input = null)
    {
        level = GameManager.Instance.Inventory.Level;
        UpdateConstructionUI();
        GameManager.Instance.Inventory.MaterialCollection.OnItemChanged += UpdateConstructionUI;
        EventManager.StopListening(SystemEvents.SystemsReady, InitUpdateConstructionUI);
    }

    void OnDisable() => GameManager.Instance.Inventory.MaterialCollection.OnItemChanged -= UpdateConstructionUI;

    public void UpdateLevel(int le)
    {
        level = le;
        UpdateConstructionUI();
    }

    public void UpdateConstructionUI()
    {

        foreach (Transform child in scrollViewContent.transform)
            Destroy(child.gameObject);

        playerInventory = GameManager.Instance.Inventory.MaterialCollection;
        foreach (var itemStack in playerInventory.Items)
        {
            if (itemStack.Data.itemType != ItemType.Constructions) continue;
            if (itemStack.Count <= 0) continue;
            GameObject slot = Instantiate(SlotPrefab, scrollViewContent.transform);

            LargeSlotController slotController = slot.GetComponent<LargeSlotController>();
            if (slotController != null) slotController.InitializeSlot(itemStack.Data, level);
            else Debug.LogError("LargeSlotController 组件未找到!");
        }
    }
}
