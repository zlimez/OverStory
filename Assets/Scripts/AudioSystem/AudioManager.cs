using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abyss.EventSystem;
using Abyss.Utils;

public class AudioManager : Singleton<AudioManager>
{
    private float timeDuration = 0.5f;
    private List<AudioSource> audioSources = new List<AudioSource>();
    [SerializeField] AudioSource uiAudioSource, soundEffectAudioSource, controlledAudioSource;
    [SerializeField] AudioClip interactalbeHintAudio;

    void OnEnable()
    {
        EventManager.StartListening(PlayEventCollection.InteractableEntered, PlayInteractableHintAudio);
    }

    void OnDisable()
    {
        EventManager.StopListening(PlayEventCollection.InteractableEntered, PlayInteractableHintAudio);
    }

    public void PlayInteractableHintAudio(object o = null)
    {
        AudioManager.Instance.StartPlayingUiAudio(interactalbeHintAudio);
    }

    public void StartPlayingUiAudio(AudioClip audioClip)
    {
        uiAudioSource.clip = audioClip;
        uiAudioSource.Play();
    }

    public void StartPlayingSoundEffectAudio(AudioClip audioClip)
    {
        soundEffectAudioSource.clip = audioClip;
        soundEffectAudioSource.Play();
    }

    public void StartPlayingControlledAudioSource(AudioClip audioClip, bool isLooping)
    {
        if (controlledAudioSource.clip != null)
        {
            Debug.LogWarning("Existing xontrolled Audio should be stopped before playing new one.");
        }

        controlledAudioSource.clip = audioClip;
        controlledAudioSource.loop = isLooping;
        controlledAudioSource.Play();
    }

    public void StopPlayingUiAudio()
    {
        uiAudioSource.Stop();
        uiAudioSource.clip = null;
    }

    public void StopPlayingSoundEffectAudio()
    {
        soundEffectAudioSource.Stop();
        soundEffectAudioSource.clip = null;
    }

    public void StopPlayingControlledAudioSource()
    {
        controlledAudioSource.Stop();
        controlledAudioSource.clip = null;
        controlledAudioSource.loop = false;
    }

    public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }
}
