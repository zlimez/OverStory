using Abyss.EventSystem;
using UnityEngine;

namespace Abyss.Interactables
{
    public class BackpackPickup : Interactable
    {
        [SerializeField] protected Conversation foundBagConvo;
        [SerializeField] protected DynamicEvent dronePickedEvent, backpackPickedEvent;

        protected override void PlayerEnterAction(Collider2D collider)
        {
            base.PlayerEnterAction(collider);
            if (EventLedger.Instance.HasOccurred(new GameEvent(dronePickedEvent.EventName))) DialogueManager.Instance.HardStartConvo(foundBagConvo);
        }

        public override void Interact()
        {
            GameManager.Instance.Inventory.Enabled = true;
            EventLedger.Instance.Record(new GameEvent(backpackPickedEvent.EventName));
            Destroy(gameObject);
        }
    }
}
