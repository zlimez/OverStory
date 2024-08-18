using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronellium.EventSystem;

// NOTE: Transparency does not work @abner leave it to you for the visuals
public class GhostFader : MonoBehaviour
{

    // [SerializeField] private SpriteRenderer spriteRenderer;
    // [SerializeField] private float fadeDuration;
    // private IEnumerator fadeOutRoutine;
    // private IEnumerator fadeInRoutine;

    void OnEnable() {

        EventManager.StartListening(DynamicEvent.GhostReplayCompleted, Dismiss);
    }

    void OnDisable() {
        EventManager.StopListening(DynamicEvent.GhostReplayCompleted, Dismiss);
    }

    public void Dismiss(object input = null) {
        gameObject.SetActive(false);
    }

    // public void StartFadeOut(object input = null) {
    //     if (fadeOutRoutine != null) return;
    //     if (fadeInRoutine != null) StopCoroutine(fadeInRoutine);
    //     fadeOutRoutine = Fade(false);
    //     StartCoroutine(fadeOutRoutine);
    // }

    // public void StartFadeIn(object input = null) {
    //     if (fadeInRoutine != null) return;
    //     if (fadeOutRoutine != null) StopCoroutine(fadeOutRoutine);
    //     fadeInRoutine = Fade(true);
    //     StartCoroutine(fadeInRoutine);
    // }

    // IEnumerator Fade(bool isIn)
    // {
    //     float timeElapsed = 0;
    //     while (timeElapsed < fadeDuration)
    //     {
    //         timeElapsed += Time.deltaTime;
    //         spriteRenderer.color = isIn 
    //             ? Color.LerpUnclamped(spriteRenderer.color, Color.white, timeElapsed / fadeDuration) 
    //             : Color.LerpUnclamped(spriteRenderer.color, ColorUtils.transparent, timeElapsed / fadeDuration);
    //         yield return null;
    //     }

    //     if (isIn) {
    //         fadeInRoutine = null;
    //     } else {
    //         fadeOutRoutine = null;
    //     }
    // }
}
