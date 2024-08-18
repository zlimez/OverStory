using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronellium.SceneSystem;

public class Portal : Interactable
{
    [SerializeField] string sceneName;
    [SerializeField] AudioClip portalAudio;

    void Update()
    {
        TryInteract();
    }

    public override void Interact()
    {
        AudioManager.Instance.StartPlayingSoundEffectAudio(portalAudio);
        SceneLoader.Instance.PrepLoadWithMaster(sceneName);
    }
}
