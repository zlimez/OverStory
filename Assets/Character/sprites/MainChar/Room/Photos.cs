using System.Collections;
using DigitalRuby.Tween;
using Chronellium.EventSystem;
using UnityEngine;

public class Photos : Interactable
{

    public SpriteRenderer buttonPressHint;
    [SerializeField] Conversation inquiry1;
    void Update()
    {
        TryInteract();
    }
    public override void Interact()
    {
        DialogueManager.Instance.StartConversation(inquiry1);
    }
}
