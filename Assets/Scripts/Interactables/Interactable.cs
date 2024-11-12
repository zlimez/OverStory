using UnityEngine;
using Abyss.EventSystem;
using Abyss.Player;

namespace Abyss.Interactables
{
    public abstract class Interactable : MonoBehaviour
    {
        [Header("Hint")]
        [SerializeField] string infoText;
        [SerializeField] GameObject namePrefab;
        [SerializeField] string nameText;
        private Info23Controllor InfoControllor;
        protected GameObject player;

        void Start()
        {
            if (namePrefab != null)
            {
                InfoControllor = Instantiate(namePrefab, transform).GetComponent<Info23Controllor>();
                InfoControllor.InitializePanel(nameText, transform);
            }
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
            if (InfoControllor != null) InfoControllor.OpenPanel();
            EventManager.InvokeEvent(PlayEvents.InteractableEntered, infoText);
            player = collider.gameObject;
            collider.GetComponent<PlayerController>().OnAttemptInteract += Interact;
        }

        protected virtual void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
                PlayerExitAction(collider);
        }

        protected virtual void PlayerExitAction(Collider2D collider)
        {
            if (InfoControllor != null) InfoControllor.ClosePanel();
            EventManager.InvokeEvent(PlayEvents.InteractableExited);
            collider.GetComponent<PlayerController>().OnAttemptInteract -= Interact;
        }
    }
}
