using UnityEngine.EventSystems;
using UnityEngine;
using Abyss.EventSystem;

// This class is responsible for controlleing setting the inventory slots which are it's children.
public class InventorySlotController : MonoBehaviour
{
    public bool isNormal;// Indicate whether the controller is controlling the normal inventory or the scanned inventory.
    public Collection referencedCollection; // A reference to which inventory
    public InventorySlot[] slots; // Represents a collection of the individual slots in the UI
    public GameObject firstSlot;

    public Transform inventorySlotParent;

    private int selectedSlot;

    // Returns the currently selected item
    public Item GetCurrentItem()
    {
        if (selectedSlot < 0 || selectedSlot >= referencedCollection.Size())
        {
            return null;
        }

        if (slots[selectedSlot].itemStack == null)
            return null;

        return slots[selectedSlot].itemStack.Data;
    }

    public void UseCurrentItem()
    {
        if (selectedSlot < 0 || selectedSlot >= referencedCollection.Size())
        {
            return;
        }

        if (slots[selectedSlot].itemStack == null)
            return;

        slots[selectedSlot].UseItem();
        return;
    }

    // Called when object becomes active, in this case when inventory ui is opened
    void OnEnable()
    {
        EventManager.StartListening(Inventory.inventoryAssignedEvent, Init);
    }

    void OnDisable()
    {
        EventManager.StopListening(Inventory.inventoryAssignedEvent, Init);
    }


    // This is called when  inventory has been assigned
    public void Init(object input = null)
    {
        // We have two separate inventories. This controller could be attached to control either of them
        referencedCollection = isNormal ? Inventory.Instance.NormalCollection : Inventory.Instance.ScannedCollection;
        if (isNormal)
        {
            InventoryUI.Instance.EnableItemHints();
        }



        slots = inventorySlotParent.GetComponentsInChildren<InventorySlot>();

        // Clear all the slots so they have no icons
        // These inventory slots need a reference to the other slots
        foreach (InventorySlot slot in slots)
        {
            slot.ClearSlot();
            slot.referencedCollection = referencedCollection;
        }

        // Sets the actual items to be in the inventory
        for (int i = 0; i < referencedCollection.Size(); i++)
        {
            slots[i].SetItem(referencedCollection.items[i]);
        }

        selectedSlot = 0;
    }

    // Highlights one specific inventory slot at the selectedSlot index
    public void HiglightSlotAtIndex()
    {
        _higlightSlotAtIndex(selectedSlot);
    }

    private void _higlightSlotAtIndex(int highlightSlot)
    {
        if (highlightSlot < 0 || highlightSlot > slots.Length)
        {
            return;
        }

        foreach (InventorySlot i in slots)
        {
            i.Deselect();

        }
        slots[highlightSlot].Select();
    }

    // Deselects all inventory slots
    public void UnhighlightAll()
    {
        foreach (InventorySlot i in slots)
        {
            i.Deselect();

        }
    }

    // Function only need to be registered when referencedCollection panel is active.
    // Updates the UI when the refrencedCollection (Normal/Scanned) changes
    public void UpdateUI()
    {
        // Display all the items as icons in their respective slots
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < referencedCollection.Size())
            {
                slots[i].SetItem(referencedCollection.items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }

    // Changes the selected inventory slot
    public void SelectNext()
    {
        selectedSlot++;
        ClampSelectedSlot();
    }

    // Changes the selected inventory slot
    public void SelectPrev()
    {
        selectedSlot--;
        ClampSelectedSlot();
    }

    // Only certain valid slots depending on the type of inventory we are controlling
    private void ClampSelectedSlot()
    {
        selectedSlot = Mathf.Clamp(selectedSlot, 0, slots.Length - 1);

        // For normal items, restrict selections to things already displayed
        if (isNormal)
        {
            selectedSlot = Mathf.Clamp(selectedSlot, 0, referencedCollection.Size() - 1);
        }
    }

    // This is called once every time the inventory panel opens
    public void Display()
    {
        // NOTE: Defensive coding in case when inventory is initialized the callback Init has not
        // registered with EventManager and thus referenced collection is not assigned
        if (referencedCollection == null) Init();

        referencedCollection.onItemChanged += UpdateUI;

        // The referencedCollection is closed every time a special item is used.
        // UpdateUI();
        foreach (Transform child in transform)
        {
            // Disable the renderer of the child object
            child.gameObject.SetActive(true);
        }

        //Set the first selected object to be the first slot
        if (referencedCollection.Size() != 0)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSlot);
        }

    }
}
