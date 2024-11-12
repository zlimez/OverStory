using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Abyss.Utils;
using Abyss.EventSystem;
using Tuples;

public class SpeechManager : MonoBehaviour
{
    [SerializeField] private float speedMod = 1f, defaultCharInterval = 0.005f;

    [Header("Speech Bubble")]
    [SerializeField] private bool isForPlayer;
    [SerializeField] private GameObject playerSpeechPanel;
    [SerializeField] private GameObject playerSpeechBox; // Assign this in the inspector to the UI element above the player's head
    [SerializeField] private TextMeshProUGUI playerSpeechTMP; // Assign this in the inspector to the TextMeshProUGUI component in the playerDialogBox

    private Queue<string> dialogueQueue = new Queue<string>();
    private bool isDialoguePlaying = false;
    private Coroutine dialogueCoroutine;
    public bool IsFacingLeft { get; private set; } = false;

    void OnEnable()
    {
        if (isForPlayer)
        {
            EventManager.StartListening(PlayEvents.PlayerSpeak, UseEnqueueDialogue);
            EventManager.StartListening(PlayEvents.PlayerSpeakFlip, FlipHorizontally);
        }
    }

    void OnDisable()
    {
        if (isForPlayer)
        {
            EventManager.StopListening(PlayEvents.PlayerSpeak, UseEnqueueDialogue);
            EventManager.StopListening(PlayEvents.PlayerSpeakFlip, FlipHorizontally);
        }
    }

    void UseEnqueueDialogue(object args)
    {
        if (args is RefPair<string, float> arg) EnqueueDialogue(arg.Head, arg.Tail);
    }

    public void EnqueueDialogue(string dialogue, float duration)
    {
        dialogueQueue.Enqueue(dialogue);
        if (!isDialoguePlaying)
        {
            StartNextDialogue(duration);
        }
    }

    private void StartNextDialogue(float duration)
    {
        if (dialogueQueue.Count > 0)
        {
            string nextDialogue = dialogueQueue.Dequeue();
            dialogueCoroutine = StartCoroutine(DisplayDialogue(nextDialogue, duration));
        }
    }

    private IEnumerator DisplayDialogue(string dialogue, float duration)
    {
        isDialoguePlaying = true;
        playerSpeechTMP.text = "";
        playerSpeechBox.SetActive(true);

        foreach (char letter in dialogue.ToCharArray())
        {
            playerSpeechTMP.text += letter;
            if(isForPlayer) AdjustPosition();
            yield return new WaitForSecondsRealtime(defaultCharInterval / speedMod);
        }

        yield return new WaitForSeconds(duration);
        playerSpeechBox.SetActive(false);
        isDialoguePlaying = false;

        if (dialogueQueue.Count > 0)
        {
            StartNextDialogue(duration);
        }
    }

    public void ClearQueue()
    {
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
        }
        dialogueQueue.Clear();
        playerSpeechBox.SetActive(false);
        isDialoguePlaying = false;
    }

    public void AdjustPosition()
    {
        RectTransform rectTransform = playerSpeechTMP.gameObject.GetComponent<RectTransform>();
        RectTransform panelTransform = playerSpeechPanel.gameObject.GetComponent<RectTransform>();
        float targetXPosition = 3.0f;
        if (IsFacingLeft) targetXPosition = rectTransform.rect.width * 1.5f + 3;
        panelTransform.anchoredPosition = new Vector2(targetXPosition, panelTransform.anchoredPosition.y);
        
    }
    public void FlipHorizontally(object inputs)
    {
        Vector3 currScale = playerSpeechTMP.gameObject.transform.localScale;
        currScale.x *= -1;
        playerSpeechTMP.gameObject.transform.localScale = currScale;

        IsFacingLeft = !IsFacingLeft;

        AdjustPosition();
        
    }
}