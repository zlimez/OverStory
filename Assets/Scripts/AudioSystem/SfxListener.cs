using Abyss.EventSystem;
using UnityEngine;

public class SfxListener : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private DynamicEvent triggerEvent, stopEvent;
    [SerializeField] private bool loop;

    void OnEnable()
    {
        EventManager.Subscribe(new GameEvent(triggerEvent.EventName), Play);
        EventManager.Subscribe(new GameEvent(stopEvent.EventName), Stop);
    }

    void Play(object input) => AudioManager.Instance.PlaySFXClip(audioClip, loop);
    void Stop(object input) => AudioManager.Instance.StopSFXClip();

    void OnDisable()
    {
        EventManager.Unsubscribe(new GameEvent(triggerEvent.EventName), Play);
        EventManager.Unsubscribe(new GameEvent(stopEvent.EventName), Stop);
    }
}
