using System.Collections;
using UnityEngine;
using Abyss.EventSystem;
using Abyss.Utils;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] AudioSource uiAudioSource, sfxAudioSource;
    [SerializeField] AudioClip interactableHint;

    void OnEnable()
    {
        EventManager.StartListening(PlayEventCollection.InteractableEntered, PlayInteractableHint);
    }

    void OnDisable()
    {
        EventManager.StopListening(PlayEventCollection.InteractableEntered, PlayInteractableHint);
    }

    public void PlayInteractableHint(object o = null)
    {
        PlayUIClip(interactableHint);
    }

    public void PlayUIClip(AudioClip audioClip)
    {
        uiAudioSource.clip = audioClip;
        uiAudioSource.Play();
    }

    public void PlaySFX(AudioClip audioClip)
    {
        sfxAudioSource.clip = audioClip;
        sfxAudioSource.Play();
    }

    public void StopUIClip()
    {
        uiAudioSource.Stop();
        uiAudioSource.clip = null;
    }

    public void StopSFX()
    {
        sfxAudioSource.Stop();
        sfxAudioSource.clip = null;
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
