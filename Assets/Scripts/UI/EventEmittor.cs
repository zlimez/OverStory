using Abyss.EventSystem;
using UnityEngine;

public class EventEmittor : MonoBehaviour
{
    // public void EmitDynamicEvent(DynamicEvent dynamicEvent) => EventManager.InvokeEvent(new GameEvent(dynamicEvent.EventName));
    public void EmitStaticEvent(string staticEvent) => EventManager.InvokeEvent(new GameEvent(staticEvent));
}
