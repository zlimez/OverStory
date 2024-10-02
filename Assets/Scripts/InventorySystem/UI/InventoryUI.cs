using UnityEngine;
using System.Collections.Generic;
using System;
using Abyss.EventSystem;
using UnityEngine.EventSystems;
using Abyss.Utils;

// Must be placed under a singleton parent.
public class InventoryUI : Singleton<InventoryUI>
{
    [SerializeField]
    ZoomInBox zoomBox; // This is the center biggest UI that shows the details of the items
    [SerializeField]
    ItemObtainedHint itemHint;
    [SerializeField] Animator confirmWindow;
    [SerializeField] GameObject confirmWindowYesButton;
    [SerializeField] GameObject panel;
    [SerializeField] InventorySlotController normalSlotController;
    [SerializeField] InventorySlotController scannedSlotController;
    public bool isOpen;
    [SerializeField] CanvasGroup TipsHider; // Parent object to the tips Displays. We use it to hide the tips after the first time the inventory opens
    [SerializeField] GameObject shortcutKeys; // These are the shorcut hint on the bottom right
    [SerializeField] GameObject shortCutKeysItemDelete;
    [SerializeField] GameObject shortcutKeysItemSelect;
    [SerializeField] GameObject shortCutKeysSwap;
    [SerializeField] AudioClip hover, select;

    private InventorySlotController currentSlotController;
    private Stack<Action> pageRecyclers; // This is a stack of function calls used to control the opening and closing of pages ontop of each other

    public Func<Item, bool> ItemSelectEventCallback;
    public Item curEquipedItem;
    public bool isItemSelectMode;
    private bool isItemSwapMode;
    private bool isFirstTimeOpened;
    private bool isRemoveWindowUp;
    private bool hasStartedItemSelect;
    Animator mWindowAnimator;
    protected override void Awake()
    {
        isFirstTimeOpened = true;

        base.Awake();

        mWindowAnimator = gameObject.GetComponent<Animator>();
        pageRecyclers = new Stack<Action>();

        // Set the first inventory the player controls to be the normal inventory
        currentSlotController = normalSlotController;
        isItemSelectMode = false;
        isRemoveWindowUp = false;

        EnableItemHints();
        HidePanel();
    }

    public void StartItemSelect(Func<Item, bool> callback)
    {
        isItemSelectMode = true;
        hasStartedItemSelect = false;
        ItemSelectEventCallback = callback;
    }
    public void StopItemSelect()
    {
        isItemSelectMode = false;
        hasStartedItemSelect = false;
        ItemSelectEventCallback = null;
    }

    public void EnableItemHints()
    {
        if (normalSlotController.referencedCollection != null)
        {
            normalSlotController.referencedCollection.onNewItemAdded += ShowItemHint;
        }
        if (scannedSlotController.referencedCollection != null)
        {
            scannedSlotController.referencedCollection.onNewItemAdded += ShowItemHint;
        }
    }

    void DisableItemHints()
    {
        if (normalSlotController.referencedCollection != null)
        {
            normalSlotController.referencedCollection.onNewItemAdded -= ShowItemHint;
        }
        if (scannedSlotController.referencedCollection != null)
        {
            scannedSlotController.referencedCollection.onNewItemAdded -= ShowItemHint;
        }
    }


    public void HideRemoveUI()
    {
        confirmWindow.Play("Window Out");
        isRemoveWindowUp = false;
    }

    // Tips are only shown the first time inventory is opened
    private void OpenPanel(object input = null)
    {
        if (!panel.activeInHierarchy)
        {
            if (isFirstTimeOpened)
            {
                TipsHider.alpha = 1;
                isFirstTimeOpened = false;
            }
            else
            {
                TipsHider.alpha = 0;
            }

            isOpen = true;
            UiStatus.OpenUI();

            // Animate the inventory window in
            mWindowAnimator.CrossFade("Window In", 0.1f);

            // Make everything visible
            normalSlotController.Display();
            normalSlotController.UnhighlightAll();
            scannedSlotController.UnhighlightAll();
            normalSlotController.UpdateUI();
            scannedSlotController.UpdateUI();
            panel.SetActive(true);

            currentSlotController = normalSlotController;
            UpdateSelectedSlotUI();

            // Ensure they eventually become invisible
            pageRecyclers.Push(HidePanel);

            // Stop time 
            // Time.timeScale = 0;

            shortcutKeys.SetActive(true);
            shortcutKeysItemSelect.SetActive(false);
            shortCutKeysSwap.SetActive(false);
            shortCutKeysItemDelete.SetActive(false);
        }
    }

    void OnEnable()
    {
        EventManager.StartListening(UIEventCollection.OpenInventory, OpenPanel);
        // when a dialog is forced starting, inventory should be disabled
        // otherwise the dialog will be interrpted when the player selects
        // an item, which cause bugs (slecting item in PimpRoom when pimp comes back)
        EventManager.StartListening(UIEventCollection.DialogStarted, ClosePanel);
    }

    void OnDisable()
    {
        EventManager.StopListening(UIEventCollection.OpenInventory, OpenPanel);
        EventManager.StopListening(UIEventCollection.DialogStarted, ClosePanel);
    }

    void SelectItem()
    {
        Input.ResetInputAxes(); // This prevents the submit command from trigger a interaction right after  a selection

        // Player has selected something
        curEquipedItem = currentSlotController.GetCurrentItem();

        if (ItemSelectEventCallback == null)
        {
            Debug.LogWarning("ItemSelectEventCallback cannot be null");
            return;
        }

        bool isCorrectItem = false;

        if (curEquipedItem != null)
        {
            // Callback not invoked when an empty slot is selected
            isCorrectItem = ItemSelectEventCallback.Invoke(curEquipedItem);
        }

        if (isCorrectItem)
        {
            // Only use the item if it is the correct item
            currentSlotController.UseCurrentItem();
        }

        curEquipedItem = null;

        // Get outa the UI
        mWindowAnimator.CrossFade("Window Out", 0.1f);
        // Close pages in the correct order
        while (pageRecyclers.Count > 0)
        {
            pageRecyclers.Pop().Invoke();
        }

        isItemSelectMode = false;
        HidePanel();
    }

    // Update is called once per frame
    void Update()
    {
        if (UiStatus.IsDisabled()) return;

        // Start item select mode
        if (isItemSelectMode && !hasStartedItemSelect)
        {
            AudioManager.Instance.StartPlayingUiAudio(hover);
            hasStartedItemSelect = true;
            Input.ResetInputAxes(); // This prevents the input that triggered the item select from exiting it prematurely as well
            OpenPanel();

            SetShortcutKeys(shortcutKeysItemSelect);
        }

        // EXIT THE UI
        if ((Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Escape)) && pageRecyclers.Count > 0)
        {
            AudioManager.Instance.StartPlayingUiAudio(hover);
            ClosePanel();
        }

        // OPEN THE UI
        else if (Input.GetKeyDown(KeyCode.I) && !DialogueManager.Instance.InDialogue)
        {
            AudioManager.Instance.StartPlayingUiAudio(hover);
            OpenPanel();
        }

        // If the Inventory Panel is being displayed
        if (!panel.activeInHierarchy)
        {
            return;
        }

        // MOVEMENT LOGIC 
        if (!isRemoveWindowUp)
        {
            MovementLogic();
        }

        UpdateSelectedSlotUI();

        // If currently select between items in the normal inventory (top row)
        if (currentSlotController == normalSlotController)
        {
            NormalSlotLogic();
        }

        // If currently select between items in the scanned inventory (bottom row)
        else if (currentSlotController == scannedSlotController)
        {
            ScannedSlotLogic();
        }

        normalSlotController.UpdateUI();
        scannedSlotController.UpdateUI();
    }


    void MovementLogic()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            // Go Left
            AudioManager.Instance.StartPlayingUiAudio(hover);
            currentSlotController.SelectPrev();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            // Go Right
            AudioManager.Instance.StartPlayingUiAudio(hover);
            currentSlotController.SelectNext();
        }

        if (!isItemSwapMode)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                // Go Up
                AudioManager.Instance.StartPlayingUiAudio(hover);

                // Switch to the normalSlotController
                currentSlotController.UnhighlightAll();
                currentSlotController = normalSlotController;

                if (!isItemSelectMode)
                {
                    SetShortcutKeys(shortcutKeys);
                }
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                // Go Down
                AudioManager.Instance.StartPlayingUiAudio(hover);

                // Switch to the scannedSlotController
                currentSlotController.UnhighlightAll();
                currentSlotController.UnhighlightAll();
                currentSlotController = scannedSlotController;

                if (!isItemSelectMode)
                {
                    SetShortcutKeys(shortCutKeysItemDelete);
                }
            }
        }
    }
    void ScannedSlotLogic()
    {
        if (isItemSwapMode)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Putting Something into the scanned items
                AudioManager.Instance.StartPlayingUiAudio(hover);
                currentSlotController = normalSlotController;

                SwapIntoScannedInventory();
                scannedSlotController.UnhighlightAll();
                UpdateSelectedSlotUI();

                // Ensure the correct shortcut keys become visible 
                SetShortcutKeys(shortcutKeys);

                isItemSwapMode = false;
            }
        }
        else if (isItemSelectMode)
        {
            if (InputManager.InventoryButtonActivated)
            {
                InputManager.InventoryButtonActivated = false;
                AudioManager.Instance.StartPlayingUiAudio(select);
                SelectItem();
            }
        }
        else
        {
            // Remove item mode. From scanned to normal inventory
            if (Input.GetKeyDown(KeyCode.E))
            {
                AudioManager.Instance.StartPlayingUiAudio(hover);
                var curScannedItem = scannedSlotController.GetCurrentItem();
                if (curScannedItem != null)
                {
                    Debug.Log("Removing item From scanned to normal inventory");
                    DisableItemHints(); // Otherwise will show the hints as if a new item has been added to the inventory
                                        // Remove an item and ask the controller to update it visually
                    normalSlotController.referencedCollection.Add(curScannedItem);
                    scannedSlotController.referencedCollection.RemoveItem(curScannedItem);
                    EnableItemHints();
                }
            }
        }
    }

    void NormalSlotLogic()
    {
        if (!isItemSelectMode)
        {
            // Swap Items into scanned inventory
            if (Input.GetKeyDown(KeyCode.E))
            {
                AudioManager.Instance.StartPlayingUiAudio(hover);
                var curNormalItem = normalSlotController.GetCurrentItem();

                if (curNormalItem == null)
                {
                    return;
                }

                // If inventory is not full just fill it to the first avaialble slot
                if (scannedSlotController.referencedCollection.Size < scannedSlotController.slots.Length)
                {

                    if (normalSlotController.referencedCollection.Contains(curNormalItem))
                    {
                        normalSlotController.referencedCollection.RemoveItem(curNormalItem);
                    }

                    DisableItemHints();
                    scannedSlotController.referencedCollection.Add(curNormalItem);
                    EnableItemHints();
                    UpdateSelectedSlotUI();
                }
                // If the inventory is full, enter swapping mode to replace one of the current scanned items
                else
                {
                    isItemSwapMode = true;
                    // Player initiates putting something into the scanned items
                    currentSlotController = scannedSlotController;
                    UpdateSelectedSlotUI();

                    // Ensure the correct shortcut keys become visible 
                    SetShortcutKeys(shortCutKeysSwap);
                }
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                AudioManager.Instance.StartPlayingUiAudio(hover);
                var curNormalItem = normalSlotController.GetCurrentItem();

                // TODO Should start a remove item prompt over here
                if (curNormalItem != null)
                {
                    isRemoveWindowUp = true;
                    EventSystem.current.SetSelectedGameObject(confirmWindowYesButton); // Sets Yes to be the default option
                    confirmWindow.Play("Window In");
                    pageRecyclers.Push(HideRemoveUI);
                }
            }

        }
        else
        {
            //Item selection mode
            if (InputManager.InventoryButtonActivated)
            {
                InputManager.InventoryButtonActivated = false;
                AudioManager.Instance.StartPlayingUiAudio(select);
                SelectItem();
            }
        }
    }
    private void SetShortcutKeys(GameObject correctShortCutKey)
    {
        shortcutKeys.SetActive(false);
        shortcutKeysItemSelect.SetActive(false);
        shortCutKeysSwap.SetActive(false);
        shortCutKeysItemDelete.SetActive(false);

        correctShortCutKey.SetActive(true);
    }

    public void ClosePanel(object o = null)
    {
        if (!isOpen)
            // ClosePanel is called when a dialog starts if the 
            // inventory is not opened when the dialog starts
            // the animation will not be played
            return;

        mWindowAnimator.CrossFade("Window Out", 0.1f);

        // Close pages in the correct order
        while (pageRecyclers.Count > 0)
        {
            pageRecyclers.Pop().Invoke();
        }

        isItemSelectMode = false;
    }

    public void RemoveCurItem()
    {
        var curNormalItem = normalSlotController.GetCurrentItem();
        if (curNormalItem != null && normalSlotController.referencedCollection.Contains(curNormalItem))
        {
            normalSlotController.referencedCollection.RemoveItem(curNormalItem);
        }

        HideRemoveUI();
    }


    // This function attempts to store one item from normal to scanned
    void SwapIntoScannedInventory()
    {
        var curScannedItem = scannedSlotController.GetCurrentItem();
        var curNormalItem = normalSlotController.GetCurrentItem();

        // There is no selected item to store, abort!
        if (curNormalItem == null)
        {
            return;
        }

        // If the item already exist in scanned don't do anything
        if (curScannedItem != null && scannedSlotController.referencedCollection.Contains(curNormalItem))
        {
            return;
        }


        DisableItemHints(); // Otherwise will show the hints as if a new item has been added to the inventory

        // If scanned items is not full, then add the current normal item to the scanned 
        if (scannedSlotController.slots.Length > scannedSlotController.referencedCollection.Size)
        {
            normalSlotController.referencedCollection.RemoveItem(curNormalItem);
            scannedSlotController.referencedCollection.Add(curNormalItem);
        }

        // Otherwise the scanned inventory is full! Replace the selected scanned item item
        else
        {
            scannedSlotController.referencedCollection.RemoveItem(curScannedItem);
            scannedSlotController.referencedCollection.Add(curNormalItem);
            normalSlotController.referencedCollection.RemoveItem(curNormalItem);
            normalSlotController.referencedCollection.Add(curScannedItem);
        }

        EnableItemHints();

        normalSlotController.UpdateUI();
        scannedSlotController.UpdateUI();
    }

    void UpdateSelectedSlotUI()
    {
        currentSlotController.HiglightSlotAtIndex();
        if (currentSlotController.GetCurrentItem() != null)
        {
            ZoomToShowItem(currentSlotController.GetCurrentItem());
        }
        else
        {
            zoomBox.Hide();
        }
    }

    // This hides the inventory UI, returns to regular gameplay
    public void HidePanel()
    {
        isOpen = false;

        panel.SetActive(false);

        // Don't change UiStatus to !isOpen if the closing inventory is followed by Dialogue or Choice
        if (!DialogueManager.Instance.InDialogue && !ChoiceManager.Instance.InChoice)
            UiStatus.CloseUI();
        // Time.timeScale = 1;
    }

    public void ZoomToShowItem(Item item)
    {
        zoomBox.Show(item);
    }

    // Shows item hint pop up
    public void ShowItemHint(Item item)
    {
        itemHint.Show(item);
    }
}