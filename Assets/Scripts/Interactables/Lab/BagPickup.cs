using Abyss.EventSystem;
using UnityEngine;

namespace Abyss.Interactables
{
    public class BagPickup : Interactable
    {
        [SerializeField] Conversation foundBagConvo;
        [SerializeField] DynamicEvent dronePickedEvent, bagPickedEvent;

        protected override void OnTriggerEnter2D(Collider2D collider)
        {
            base.OnTriggerEnter2D(collider);
            if (EventLedger.Instance.HasOccurred(new GameEvent(dronePickedEvent.EventName)) && player != null) DialogueManager.Instance.HardStartConvo(foundBagConvo);
        }

        public override void Interact()
        {
            GameManager.Instance.Inventory.Enabled = true;
            EventLedger.Instance.Record(new GameEvent(bagPickedEvent.EventName));
            Destroy(gameObject);
        }
    }
}
