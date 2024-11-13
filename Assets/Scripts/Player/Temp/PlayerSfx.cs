using System.Collections;
using System.Collections.Generic;
using Abyss.SceneSystem;
using UnityEngine;

public class PlayerSfx : MonoBehaviour
{
    [SerializeField] AudioClip jumpSfx;
    [SerializeField] AudioClip dashSfx;
    [SerializeField] AudioClip hurtSfx;

    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioClip labBgm, room1Bgm, room2Bgm, room3Bgm;

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

    public void PlayBgm(AbyssScene scene)
    {
        switch (scene)
        {
            case AbyssScene.Lab:
                bgmSource.clip = labBgm;
                break;
            case AbyssScene.Room1:
                bgmSource.clip = room1Bgm;
                break;
            case AbyssScene.Room2:
                bgmSource.clip = room2Bgm;
                break;
            case AbyssScene.Room3:
                bgmSource.clip = room3Bgm;
                break;
        }
        bgmSource.Play();
    }
}
