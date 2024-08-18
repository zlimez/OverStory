using Chronellium.EventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptySpace : MonoBehaviour
{

    private void OnTriggerEnter(Collider collision)
    {
        if (IsPlayer(collision.gameObject))
        {
            EventManager.InvokeEvent(StaticEvent.Core_ResetPuzzle);
        }
    }

    private bool IsPlayer(GameObject otherObject)
    {
        return otherObject.CompareTag("Player");
    }
}
