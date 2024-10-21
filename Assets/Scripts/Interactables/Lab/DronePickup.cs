using Abyss.EventSystem;
using UnityEngine;

public class DronePickup : Interactable
{
    [SerializeField] DroneBT droneBT;
    [SerializeField] SpriteRenderer droneSprite;
    [SerializeField] Conversation awake;
    [SerializeField] DynamicEvent dronePickedEvent;

    public override void Interact()
    {
        droneBT.enabled = true;
        gameObject.SetActive(false);
        droneSprite.color = Color.white;
        GameEvent gameEvent = new(dronePickedEvent.EventName);
        EventManager.InvokeEvent(gameEvent);
        EventLedger.Instance.RecordEvent(gameEvent);
        DialogueManager.Instance.StartConvo(awake);
    }
}
