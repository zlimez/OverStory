using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronellium.TimeManagers;
using Chronellium.EventSystem;

public class BreathSliderManager : TimerSliderManager
{
    protected override void OnEnable()
    {
        base.OnEnable();
        EventManager.StartListening(StaticEvent.Core_SwitchToRealWorld, HideSlider);
    }

    protected override void OnDisable() {
        EventManager.StopListening(StaticEvent.Core_SwitchToRealWorld, HideSlider);
        base.OnDisable();
    }

    void HideSlider(object input = null) {
        timerSliderObject.SetActive(false);
    }
}
