using Abyss.EventSystem;
using UnityEngine;
using Abyss.Settings;

public class DynEventEmittor : MonoBehaviour
{
    [SerializeField] DynamicEvent eventToEmit;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tag.Player))
            EventManager.InvokeEvent(new GameEvent(eventToEmit.EventName));
    }
}
