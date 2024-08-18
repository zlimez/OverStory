using System.Collections;
using DigitalRuby.Tween;
using Chronellium.EventSystem;
using UnityEngine;

public class Letter : Interactable
{

    public AudioClip sadPianoBGM;
    public AudioSource audioSource2;
    public SpriteRenderer buttonPressHint;
    [SerializeField] Conversation inquiry1, inquiry2;
    private GameEvent startSadPianoEvent = new GameEvent("StartSadPiano");
    private bool hasTalked = false;
    void Start()
    {
    }
    void Update()
    {
        TryInteract();
    }
    public override void Interact()
    {
        EventManager.StartListening(startSadPianoEvent, StartSadPiano);
        if (!hasTalked)
        {
            DialogueManager.Instance.StartConversation(inquiry1);
            hasTalked = true;
        }
        else
        {
            DialogueManager.Instance.StartConversation(inquiry2);
        }
    }
    public void StartSadPiano(object input = null)
    {

        audioSource2.clip = sadPianoBGM;
        audioSource2.Play();
    }
}
