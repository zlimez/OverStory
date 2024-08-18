using UnityEngine;

public class PlayAudioOnTriggerEnter : MonoBehaviour
{
    public AudioSource audioSource;
    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasPlayed && audioSource != null && other.CompareTag("Player"))
        {
            audioSource.Play();
            hasPlayed = true;
        }
    }
}