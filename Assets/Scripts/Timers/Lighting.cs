using System;
using System.Collections;
using System.Collections.Generic;
using Abyss.EventSystem;
using Abyss.TimeManagers;
using Algorithms;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Lighting : MonoBehaviour
{
    [SerializeField][Tooltip("Dev responsibility to ensure the broadcasting TimeCycle's interval has sufficient granularity")] List<LightSettings> lightingAts;
    Light2D _light2D;
    IEnumerator _activeTransition;
    int _activePter = -1;

    void OnEnable() => EventManager.StartListening(SystemEvents.TimeBcastEvent, StartTransition);
    void OnDisable() => EventManager.StopListening(SystemEvents.TimeBcastEvent, StartTransition);

    void Awake()
    {
        _light2D = GetComponent<Light2D>();
        lightingAts.Sort((a, b) => a.TimeOfDay.CompareTo(b.TimeOfDay));
    }

    // Only latest transition bwing ran
    void StartTransition(object input = null)
    {
        (float timeOfDay, _) = (ValueTuple<float, float>)input;
        var transitTarget = Search.BinFindLEQ(lightingAts, timeOfDay, (a, b) => a.CompareTo(b.TimeOfDay));
        if (transitTarget == _activePter) return;
        _activePter = transitTarget;
        if (_activeTransition != null)
        {
            StopCoroutine(_activeTransition);
            _activeTransition = null;
        }
        if (transitTarget == -1) transitTarget = lightingAts.Count - 1;
        Debug.Log("Lighting " + transitTarget);
        _activeTransition = TransitionRoutine(lightingAts[transitTarget]);
        StartCoroutine(_activeTransition);
    }

    IEnumerator TransitionRoutine(LightSettings target)
    {
        float elapsedTime = 0;
        float stIntensity = _light2D.intensity;
        Color stColor = _light2D.color;

        while (elapsedTime < target.Duration)
        {
            elapsedTime += Time.deltaTime * TimeCycle.Instance.SpeedMod;
            _light2D.intensity = Mathf.Lerp(stIntensity, target.Intensity, elapsedTime / target.Duration);
            _light2D.color = Color.Lerp(stColor, target.Color, elapsedTime / target.Duration);
            yield return null;
        }
        _light2D.intensity = target.Intensity;
        _light2D.color = target.Color;

    }
}

[Serializable]
public struct LightSettings
{
    public float Intensity;
    public float TimeOfDay;
    public float Duration;
    public Color Color;
}
