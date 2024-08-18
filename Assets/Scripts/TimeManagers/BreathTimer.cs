using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronellium.TimeManagers;
using Chronellium.EventSystem;
using DeepBreath.Environment;

public class BreathTimer : CountdownTimer
{
    public static BreathTimer Instance;
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private float lowBreathLevel = 20f;
    private void OnEnable()
    {
        OnTimerExpire.AddListener(SignalBreathout);

        EventManager.StartListening(StaticEvent.Core_SwitchToOtherWorld, StartBreathTimer);
        EventManager.StartListening(StaticEvent.Core_SwitchToRealWorld, ResetBreathTimer);
    }

    private void OnDisable()
    {
        OnTimerExpire.RemoveListener(SignalBreathout);

        EventManager.StopListening(StaticEvent.Core_SwitchToOtherWorld, StartBreathTimer);
        EventManager.StopListening(StaticEvent.Core_SwitchToRealWorld, ResetBreathTimer);
    }

    private void SignalBreathout()
    {
        EventManager.InvokeEvent(StaticEvent.Core_OutOfBreath);
        StateManager.SwitchRealm(true);
    }

    private void SignalLowBreath()
    {
        Debug.Log("Low breath signalled with " + TimeLeft + " left");
        EventManager.InvokeEvent(StaticEvent.Core_LowBreath);
    }

    private void StartBreathTimer(object input = null)
    {
        StartTimer();
        ScheduleAction(lowBreathLevel, SignalLowBreath);
    }

    private void ResetBreathTimer(object input = null)
    {
        ResetTimer();
    }
}
