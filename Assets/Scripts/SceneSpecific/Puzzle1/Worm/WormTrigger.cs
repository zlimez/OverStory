using Chronellium.EventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormTrigger : MonoBehaviour
{
    public GameObject wormBodySprite;

    private void Start()
    {
        wormBodySprite.SetActive(false);
        EventManager.StartListening(StaticEvent.Core_ResetPuzzle, HideWormBody);
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Object entered Worm Trigger: " + collision.gameObject);
        if (IsCableSeats(collision.gameObject))
        {
            // if (!hasAttacked)
            // {
            //     wormBodySprite.SetActive(true);
            //     worm.Attack();
            //     hasAttacked = true;
            // }
        }
    }

    private void HideWormBody(object input = null)
    {
        wormBodySprite.SetActive(false);
    }

    private bool IsCableSeats(GameObject otherObject)
    {
        return otherObject.CompareTag("CableSeats");
    }
}
