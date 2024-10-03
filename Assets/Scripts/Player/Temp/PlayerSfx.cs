using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSfx : MonoBehaviour
{
    [SerializeField] AudioClip jumpSfx;
    [SerializeField] AudioClip dashSfx;
    [SerializeField] AudioClip hurtSfx;
    AudioSource _audioSource;

    void Awake() { _audioSource = GetComponent<AudioSource>(); }

    public void PlayJump()
    {
        _audioSource.clip = jumpSfx;
        _audioSource.Play();
    }

    public void PlayDash()
    {
        _audioSource.clip = dashSfx;
        _audioSource.Play();
    }

    public void PlayHurt()
    {
        _audioSource.clip = hurtSfx;
        _audioSource.Play();
    }
}
