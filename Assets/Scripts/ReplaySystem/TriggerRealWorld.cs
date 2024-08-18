using Chronellium.EventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerRealWorld : Interactable
{

    private void Update()
    {
        TryInteract();
    }

    public override void Interact()
    {
        EventManager.InvokeEvent(StaticEvent.Core_SwitchToRealWorld);
        Debug.Log("Real World Entered");
    }
}
