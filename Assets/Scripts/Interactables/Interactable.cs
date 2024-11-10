using UnityEngine;
using Abyss.EventSystem;
using Abyss.Player;

namespace Abyss.Interactables
{
    public abstract class Interactable : MonoBehaviour
    {
        [Header("Hint")]
        [SerializeField] bool hasHint = false; // TODO: Collapse the rest of hint related configs if set to false
        [SerializeField] Vector3 offSetPosition;
        [SerializeField] float hintScale = 1;
        [SerializeField] GameObject hintPrefab;
        [SerializeField] string infoText;

        // [Header("Use Item")]
        // [SerializeField] private bool isItemUsable = false;
        // StartDialog is played before selection, if left empty, no dialog will be triggered before choice.
        // [SerializeField] private Conversation startDialog;
        // WrongItem is the default dialog when a wrong item is chosen.
        // [SerializeField] protected Conversation wrongItem;
        // [SerializeField] string useItemText = "Use Item", interactText = "Interact", leaveText = "Leave";

        private GameObject hint;
        protected GameObject player;

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

        public virtual void Interact()
        {
            player.GetComponent<PlayerController>().OnAttemptInteract -= Interact;
            EventManager.InvokeEvent(PlayEvents.InteractableExited);
        }

        protected virtual void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
                PlayerEnterAction(collider);
        }

        protected virtual void PlayerEnterAction(Collider2D collider)
        {
            EventManager.InvokeEvent(PlayEvents.InteractableEntered, infoText);
            collider.GetComponent<PlayerController>().OnAttemptInteract += Interact;
            player = collider.gameObject;
            SpawnHint();
        }

        protected virtual void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
                PlayerExitAction(collider);
        }

        protected virtual void PlayerExitAction(Collider2D collider)
        {
            EventManager.InvokeEvent(PlayEvents.InteractableExited);
            collider.GetComponent<PlayerController>().OnAttemptInteract -= Interact;
            player = null;
            DestroyHint();
        }
    }
}
