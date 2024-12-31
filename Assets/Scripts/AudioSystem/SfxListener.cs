using Abyss.EventSystem;
using UnityEngine;

public class SfxListener : MonoBehaviour
{
    [SerializeField] AudioClip audioClip;
    [SerializeField] DynamicEvent triggerEvent, stopEvent;
    [SerializeField] bool loop;

    void OnEnable()
    {
        EventManager.StartListening(new GameEvent(triggerEvent.EventName), Play);
        EventManager.StartListening(new GameEvent(stopEvent.EventName), Stop);
    }

    void Play(object input) => AudioManager.Instance.PlaySFXClip(audioClip, loop);
    void Stop(object input) => AudioManager.Instance.StopSFXClip();

    void OnDisable()
    {
        EventManager.StopListening(new GameEvent(triggerEvent.EventName), Play);
        EventManager.StopListening(new GameEvent(stopEvent.EventName), Stop);
    }
}
