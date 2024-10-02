using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public GameObject inventoryPanel;
    public Button InventoryButton;
    public Button SpellButton;
    public Button ConstructionButton;
    public Button JournalButton;
    public Button CloseButton;
    

    public int level = 3; 
    public Sprite[] backgroundImages;  
    public Image inventoryBackground;  

    private bool isInventoryOpen = false;

    void Start()
    {
        inventoryPanel.SetActive(false);

        UpdateInventoryImage(level-1);

        SpellButton.onClick.AddListener(OpenSpell);
        CloseButton.onClick.AddListener(ToggleInventory);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);
    }

    void UpdateInventoryImage(int order)
    {
        Debug.Log("level: "+order);
        if (order >= 0 && order < backgroundImages.Length)
        {
            inventoryBackground.sprite = backgroundImages[order];
        }
    }

    public void LevelUp()
    {
        level++;
        UpdateInventoryImage(level-1);
    }

    void OpenSpell()
    {
        Debug.Log("Spell已打开！");
    }
}
