using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject Bag;
    public GameObject Spell;
    public GameObject Construction;
    public GameObject Journal;


    public Button BagButton;
    public Button SpellButton;
    public Button ConstructionButton;
    public Button JournalButton;
    public Button CloseButton;
    

    public int level = 3; 
    public Sprite[] backgroundImages;  
    public Image inventoryBackground;  
    public Sprite[] BagImages;  
    public Image BagBG;  
    public Sprite[] SpellImages;  
    public Image SpellBG;  
    public Sprite[] ConstructionImages;  
    public Image ConstructionBG;  
    public Sprite[] JournalImages;  
    public Image JournalBG;  

    private bool isInventoryOpen = false;
    private bool isBagOpen = false;
    private bool isSpellOpen = false;
    private bool isConstructionOpen = false;
    private bool isJournalOpen = false;

    void Start()
    {
        inventoryPanel.SetActive(false);

        UpdateInventoryImage(level-1);

        BagButton.onClick.AddListener(OpenBag);
        SpellButton.onClick.AddListener(OpenSpell);
        ConstructionButton.onClick.AddListener(OpenConstruction);
        JournalButton.onClick.AddListener(OpenJournal);
        CloseButton.onClick.AddListener(ToggleInventory);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
            OpenBag();
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
            BagBG.sprite = BagImages[order];
            SpellBG.sprite = SpellImages[order];
            ConstructionBG.sprite = ConstructionImages[order];
            JournalBG.sprite = JournalImages[order];
        }
    }

    public void LevelUp()
    {
        level++;
        UpdateInventoryImage(level-1);
    }

    void OpenBag()
    {
        isBagOpen = true;
        Bag.SetActive(isBagOpen);

        CloseSpell();
        CloseConstruction();
        CloseJournal();
    }

    void CloseBag()
    {
        isBagOpen = false;
        Bag.SetActive(isBagOpen);
    }

    void OpenSpell()
    {
        isSpellOpen = true;
        Spell.SetActive(isSpellOpen);

        CloseBag();
        CloseConstruction();
        CloseJournal();
    }

    void CloseSpell()
    {
        isSpellOpen = false;
        Spell.SetActive(isSpellOpen);
    }

    void OpenConstruction()
    {
        isConstructionOpen = true;
        Construction.SetActive(isConstructionOpen);

        CloseBag();
        CloseSpell();
        CloseJournal();
    }

    void CloseConstruction()
    {
        isConstructionOpen = false;
        Construction.SetActive(isConstructionOpen);
    }

    void OpenJournal()
    {
        isJournalOpen = true;
        Journal.SetActive(isJournalOpen);

        CloseBag();
        CloseSpell();
        CloseConstruction();
    }

    void CloseJournal()
    {
        isJournalOpen = false;
        Journal.SetActive(isJournalOpen);
    }
    
}
