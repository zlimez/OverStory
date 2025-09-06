using System.Collections;
using UnityEngine;
using Abyss.EventSystem;
using Abyss.Utils;
using Utils.Tuples;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] AudioSource sfxSource, bgmSource;
    [SerializeField] float transitionDuration = 1f, bgmVolume = 0.5f;
    [SerializeField] Pair<Abyss.Settings.Scene, AudioClip>[] sceneStartBgmClips;

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

        EventManager.Subscribe(SystemEvents.SceneTransitStart, ChangeBgm);
    }

    void OnDisable() => EventManager.Unsubscribe(SystemEvents.SceneTransitStart, ChangeBgm);

    public void PlaySFXClip(AudioClip audioClip, bool loop = false)
    {
        sfxSource.clip = audioClip;
        sfxSource.loop = loop;
        sfxSource.Play();
    }

    public void ChangeBgm(object input = null)
    {
        Abyss.Settings.Scene scene = (Abyss.Settings.Scene)input;
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

    public void StopSFXClip()
    {
        sfxSource.Stop();
        sfxSource.clip = null;
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
