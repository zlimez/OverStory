using System.Collections;
using UnityEngine;
using Abyss.EventSystem;
using Abyss.Utils;
using Tuples;
using Abyss.SceneSystem;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] AudioSource uiSource, bgmSource;
    [SerializeField] float transitionDuration = 1f, bgmVolume = 0.5f;
    [SerializeField] Pair<AbyssScene, AudioClip>[] sceneStartBgmClips;

    void OnEnable()
    {
        bgmSource.volume = bgmVolume;
        foreach (var p in sceneStartBgmClips)
            if (p.Head == Parser.GetSceneFromText(SceneManager.GetActiveScene().name))
            {
                bgmSource.clip = p.Tail;
                bgmSource.Play();
                break;
            }

        EventManager.StartListening(SystemEvents.SceneTransitStart, ChangeBgm);
    }

    void OnDisable() => EventManager.StopListening(SystemEvents.SceneTransitStart, ChangeBgm);

    public void PlayUIClip(AudioClip audioClip)
    {
        uiSource.clip = audioClip;
        uiSource.Play();
    }

    public void ChangeBgm(object input = null)
    {
        AbyssScene scene = (AbyssScene)input;
        bool found = false;
        foreach (var p in sceneStartBgmClips)
            if (p.Head == scene)
            {
                found = true;
                if (bgmSource.clip == p.Tail) return;
                StartCoroutine(CrossFade(bgmSource, p.Tail, transitionDuration));
                break;
            }

        if (!found) StartCoroutine(Transition(bgmSource, transitionDuration, 0));
    }

    public void StopUIClip()
    {
        uiSource.Stop();
        uiSource.clip = null;
    }

    public static IEnumerator Transition(AudioSource audioSource, float duration, float targetVolume)
    {
        float etime = 0;
        float stVol = audioSource.volume;
        while (etime < duration)
        {
            etime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(stVol, targetVolume, etime / duration);
            yield return null;
        }
        audioSource.volume = targetVolume;
    }

    public static IEnumerator CrossFade(AudioSource audioSource, AudioClip newClip, float fadeDuration)
    {
        float stVol = audioSource.volume;
        yield return Transition(audioSource, fadeDuration, 0);

        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();

        yield return Transition(audioSource, fadeDuration, stVol);
    }
}
