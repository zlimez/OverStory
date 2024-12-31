using System.Collections;
using System.Collections.Generic;
using Abyss.EventSystem;
using UnityEngine;

public class DynEventEmittor : MonoBehaviour
{
    [SerializeField] DynamicEvent eventToEmit;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            EventManager.InvokeEvent(new GameEvent(eventToEmit.EventName));
    }
}
