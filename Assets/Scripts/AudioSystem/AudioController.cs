using System.Collections;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void AudioFadeOut()
    {
        StartCoroutine(StartFade());
    }

    IEnumerator StartFade(float duration = 1f)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, 0f, currentTime / duration);
            yield return null;
        }
        yield break;
    }
}