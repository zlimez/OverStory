using System.Collections;
using DigitalRuby.Tween;
using Chronellium.EventSystem;
using UnityEngine;

public class Newspapers : Interactable
{
    public SpriteRenderer buttonPressHint;
    private GameEvent testFirstTimeEvent = new GameEvent("Jonas Has looked at the newspapers");
    [SerializeField] Conversation inquiry1, inquiry2, inquiry3, inquiry4;
    private int dialogId = 0;
    private bool hasSeenHint;

    void Start()
    {
        if (buttonPressHint != null)
        {
            // Make Hint invsible at the start
            Color tmp = buttonPressHint.color;
            tmp.a = 0f;
            buttonPressHint.color = tmp;
        }
    }

    void Update()
    {
        TryInteract();
    }
    private void OnTriggerEnter(Collider collision)
    {
        base.OnTriggerEnter(collision);
        if (hasSeenHint || buttonPressHint == null) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            System.Action<ITween<float>> BandInCallBack = (t) =>
            {
                Color tmp = buttonPressHint.color;
                tmp.a = t.CurrentValue;
                buttonPressHint.color = tmp;
            };

            // completion defaults to null if not passed in
            gameObject.Tween("FadeIn", buttonPressHint.color.a, 1.0f, 1.0f, TweenScaleFunctions.CubicEaseInOut, BandInCallBack);
        }

    }
    private void OnTriggerExit(Collider collision)
    {
        base.OnTriggerExit(collision);
        if (hasSeenHint || buttonPressHint == null) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            System.Action<ITween<float>> BandInCallBack = (t) =>
            {
                Color tmp = buttonPressHint.color;
                tmp.a = t.CurrentValue;
                buttonPressHint.color = tmp;
            };

            // completion defaults to null if not passed in
            gameObject.Tween("FadeIn", buttonPressHint.color.a, 0.0f, 1.0f, TweenScaleFunctions.CubicEaseInOut, BandInCallBack);
        }
        hasSeenHint = true;
    }

    public override void Interact()
    {

        switch (dialogId)
        {
            case 0:
                DialogueManager.Instance.StartConversation(inquiry1);
                break;
            case 1:
                DialogueManager.Instance.StartConversation(inquiry2);
                break;
            case 2:
                DialogueManager.Instance.StartConversation(inquiry3);
                break;
            case 3:
                DialogueManager.Instance.StartConversation(inquiry4);
                return;
                break;
            default:
                break;
        }
        dialogId++;
    }
}
