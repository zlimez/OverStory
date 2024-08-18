using Chronellium.EventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSoundManager : MonoBehaviour
{
    public GameObject realWorldSounds;
    public GameObject otherWorldSounds;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening(StaticEvent.Core_SwitchToRealWorld, SwitchToReal);
        EventManager.StartListening(StaticEvent.Core_SwitchToOtherWorld, SwitchToOther);
    }

    private void SwitchToReal(object input = null)
    {
        realWorldSounds.SetActive(true);
        otherWorldSounds.SetActive(false);
    }

    private void SwitchToOther(object input = null)
    {
        realWorldSounds.SetActive(false);
        otherWorldSounds.SetActive(true);
    }
}
