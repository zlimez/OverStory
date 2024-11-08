using Abyss.EventSystem;
using UnityEngine;

namespace Abyss.Interactables
{
    public class BagPickup : Interactable
    {
        [SerializeField] Conversation foundBagConvo;
        [SerializeField] DynamicEvent dronePickedEvent, bagPickedEvent;

        protected override void PlayerEnterAction(Collider2D collider)
        {
            base.PlayerEnterAction(collider);
            if (EventLedger.Instance.HasOccurred(new GameEvent(dronePickedEvent.EventName))) DialogueManager.Instance.HardStartConvo(foundBagConvo);
        }

        public override void Interact()
        {
            GameManager.Instance.Inventory.Enabled = true;
            EventLedger.Instance.Record(new GameEvent(bagPickedEvent.EventName));
            Destroy(gameObject);
        }
    }
}
