using UnityEngine;
using Abyss.EventSystem;
using Abyss.Player;

public abstract class Interactable : MonoBehaviour
{
    [Header("Hint")]
    [SerializeField] private bool hasHint = true;
    [SerializeField] private Vector3 offSetPosition;
    [SerializeField] private float hintScale = 1;
    [SerializeField] GameObject hintPrefab;

    // [Header("Use Item")]
    // [SerializeField] private bool isItemUsable = false;
    // StartDialog is played before selection, if left empty, no dialog will be triggered before choice.
    // [SerializeField] private Conversation startDialog;
    // WrongItem is the default dialog when a wrong item is chosen.
    // [SerializeField] protected Conversation wrongItem;
    // [SerializeField] string useItemText = "Use Item", interactText = "Interact", leaveText = "Leave";

    // protected Choice useItem, interact, leave;
    private GameObject hint;
    // public static string EventPrefix => "Interacted With: ";

    // protected void InitialiseItemChoice()
    // {
    //     useItem = new Choice(useItemText, UseItemChoice);
    //     interact = new Choice(interactText, InteractChoice);
    //     leave = new Choice(leaveText, LeaveChoice);
    // }

    // public virtual void UseItemChoice(object o = null)
    // {
    //     EventManager.InvokeEvent(CommonEventCollection.OpenInventory);
    //     InventoryUI.Instance.StartItemSelect(OnSelectItem);
    // }

    // public virtual bool OnSelectItem(Item item)
    // {
    //     // Wrong item selected
    //     DialogueManager.Instance.StartConversation(wrongItem);
    //     // Whether the item is removed
    //     return false;
    // }

    // public virtual void InteractChoice(object o = null)
    // {
    //     Interact();
    // }

    // public virtual void LeaveChoice(object o = null) { }

    void SpawnHint()
    {
        if (!hasHint)
            return;

        hint = Instantiate(hintPrefab);
        hint.transform.SetParent(transform);
        hint.transform.SetParent(null);
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
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

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            EventManager.InvokeEvent(PlayEventCollection.InteractableEntered);
            collider.GetComponent<PlayerController>().OnAttemptInteract += Interact;
            SpawnHint();
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            EventManager.InvokeEvent(PlayEventCollection.InteractableExited);
            collider.GetComponent<PlayerController>().OnAttemptInteract -= Interact;
            DestroyHint();
        }
    }
}
