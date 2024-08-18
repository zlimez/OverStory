using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronellium.EventSystem;
using System;

public class SplitCamera : MonoBehaviour
{
    [SerializeField] private Camera camera1, camera2;
    // NOTE: 1 is left, 2 is right. Subsequent values refer to camera 1
    [SerializeField] private float defaultWidth, splitWidth;
    [SerializeField] private float splitInterval = 0.25f, removeSecondaryCamThreshold = 0.1f;
    [SerializeField] private GameEvent[] splittingEvents;
    [SerializeField] private GameEvent[] unsplittingEvents;
    private IEnumerator changeRoutine;

    void OnEnable() {
        Array.ForEach(splittingEvents, splittingEvent => EventManager.StartListening(splittingEvent, Split));
        Array.ForEach(unsplittingEvents, unsplittingEvent => EventManager.StartListening(unsplittingEvent, Unsplit));
    }

    void OnDisable() {
        Array.ForEach(splittingEvents, splittingEvent => EventManager.StopListening(splittingEvent, Split));
        Array.ForEach(unsplittingEvents, unsplittingEvent => EventManager.StopListening(unsplittingEvent, Unsplit));
    }

    public void Split(object input = null) {
        if (changeRoutine != null) StopCoroutine(changeRoutine);
        changeRoutine = Change(true);
        StartCoroutine(changeRoutine);
    }

    public void Unsplit(object input = null) {
        if (changeRoutine != null) StopCoroutine(changeRoutine);
        changeRoutine = Change(false);
        StartCoroutine(changeRoutine);
    }

    IEnumerator Change(bool isSplit) {
        float timeElapsed = 0;
        float startWidth = camera1.rect.width;
        float targetWidth = isSplit ? splitWidth : defaultWidth;
        if (isSplit) camera2.gameObject.SetActive(true);
        while (timeElapsed < splitInterval) {
            timeElapsed += Time.deltaTime;
            float currWidth = VectorUtils.EaseOutSquare(startWidth, targetWidth, timeElapsed / splitInterval);
            camera1.rect = new Rect(0, 0, currWidth, 1);
            camera2.rect = new Rect(Mathf.Min(1, currWidth), 0, 1 - currWidth, 1);
            if (!isSplit && (1 - currWidth < removeSecondaryCamThreshold)) camera2.gameObject.SetActive(false);
            yield return null;
        }
    }
}
