using UnityEngine;
using Chronellium.EventSystem;

public abstract class Interactable : MonoBehaviour
{
    [Header("Hint")]
    [SerializeField] private bool hasHint = true;
    [SerializeField] private Vector3 offSetPosition;
    [SerializeField] private float hintScale = 1;

    [Header("Use Item")]
    [SerializeField] private bool isItemUsable = false;
    // StartDialog is played before selection; 
    // If left empty, no dialog will be triggered before choice.
    [SerializeField] private Conversation startDialog;
    // WrongItem is the default dialog when a wrong item is chosen.
    [SerializeField] protected Conversation wrongItem;
    [SerializeField] string useItemText = "Use Item", interactText = "Interact", leaveText = "Leave";

    public GameObject Player { get; private set; }
    protected Choice useItem, interact, leave;
    private bool playerInRange;
    private GameObject hint;
    public static string EventPrefix => "Interacted With: ";

    void Update()
    {
        TryInteract();
    }

    public virtual void TryInteract()
    {
        if (UiStatus.IsOpen)
        {
            // when the ui is open, eg inventory is open or in dialogue,
            // the player cannot interact with interactable objects 
            return;
        }

        if (InputManager.InteractButtonActivated && playerInRange)
        {
            InputManager.InteractButtonActivated = false;
            if (isItemUsable)
            {
                InitialiseItemChoice();
                DialogueManager.Instance.StartConversation(startDialog);
                ChoiceManager.Instance.StartChoice(interact, useItem, leave);
                return;
            }

            Interact();
        }
    }

    protected void InitialiseItemChoice()
    {
        useItem = new Choice(useItemText, UseItemChoice);
        interact = new Choice(interactText, InteractChoice);
        leave = new Choice(leaveText, LeaveChoice);
    }

    public virtual void UseItemChoice(object o = null)
    {
        EventManager.InvokeEvent(CommonEventCollection.OpenInventory);
        InventoryUI.Instance.StartItemSelect(OnSelectItem);
    }

    public virtual bool OnSelectItem(Item item)
    {
        // Wrong item selected
        DialogueManager.Instance.StartConversation(wrongItem);
        // Whether the item is removed
        return false;
    }

    public virtual void InteractChoice(object o = null)
    {
        Interact();
    }

    public virtual void LeaveChoice(object o = null) { }

    void SpawnHint()
    {
        if (hint != null)
            return;
        EventManager.InvokeEvent(CoreEventCollection.InteractableEntered);

        if (!hasHint)
            return;

        hint = Instantiate(GameManager.Instance.GetInteractableHint());
        hint.transform.SetParent(this.transform);
        hint.transform.SetParent(null);
        BoxCollider collider = this.GetComponent<BoxCollider>();
        hint.transform.position = collider.transform.position + offSetPosition;
        hint.transform.localScale = hintScale * Vector3.one;
    }

    protected void DestroyHint()
    {
        if (hint != null)
            Destroy(hint);
    }

    protected void DisableHint()
    {
        hasHint = false;
        DestroyHint();
    }

    public abstract void Interact();

    protected virtual void OnTriggerEnter(Collider collision)
    {
        if (IsPlayer(collision.gameObject))
        {
            playerInRange = true;
            Player = collision.gameObject;
            SpawnHint();
        }
    }

    protected virtual void OnTriggerExit(Collider collision)
    {
        if (IsPlayer(collision.gameObject))
        {
            playerInRange = false;
            DestroyHint();
        }
    }

    private bool IsPlayer(GameObject otherObject)
    {
        return otherObject.CompareTag("Player");
    }
}
