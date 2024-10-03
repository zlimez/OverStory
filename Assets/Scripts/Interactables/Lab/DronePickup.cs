using Abyss.EventSystem;
using UnityEngine;

public class DronePickup : Interactable
{
    [SerializeField] DroneBT droneBT;
    [SerializeField] SpriteRenderer droneSprite;
    public override void Interact()
    {
        droneBT.enabled = true;
        gameObject.SetActive(false);
        droneSprite.color = Color.white;
        EventManager.InvokeEvent(new GameEvent("DronePicked"));
    }
}
