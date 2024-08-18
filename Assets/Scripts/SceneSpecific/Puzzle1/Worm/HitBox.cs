using UnityEngine;
using Chronellium.EventSystem;

public class HitBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            EventManager.InvokeEvent(StaticEvent.Core_ResetPuzzle);
        }
    }
}
