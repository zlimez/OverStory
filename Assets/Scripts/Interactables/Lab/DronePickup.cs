using Abyss.EventSystem;
using UnityEngine;

namespace Abyss.Interactables
{
    public class DronePickup : MonoBehaviour
    {
        [SerializeField] DroneBT droneBT;
        [SerializeField] SpriteRenderer droneSprite;
        [SerializeField] GameObject droneLight;
        [SerializeField] Conversation awake;
        [SerializeField] DynamicEvent dronePickedEvent;

        void OnTriggerEnter2D(Collider2D other)
        {
            droneBT.enabled = true;
            gameObject.SetActive(false);
            droneLight.SetActive(true);
            droneSprite.color = Color.white;
            GameEvent gameEvent = new(dronePickedEvent.EventName);
            EventManager.InvokeEvent(gameEvent);
            EventLedger.Instance.Record(gameEvent);
            DialogueManager.Instance.HardStartConvo(awake);
        }
    }
}
