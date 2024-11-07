using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Abyss.Utils;

public class SpeechManager : Singleton<SpeechManager>
{
    [SerializeField] private float speedMod = 1f, defaultCharInterval = 0.005f;

    [Header("Speech Bubble")]
    [SerializeField] private GameObject playerSpeechBox; // Assign this in the inspector to the UI element above the player's head
    [SerializeField] private TextMeshProUGUI playerSpeechTMP; // Assign this in the inspector to the TextMeshProUGUI component in the playerDialogBox

    private Queue<string> dialogueQueue = new Queue<string>();
    private bool isDialoguePlaying = false;
    private Coroutine dialogueCoroutine;

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
        playerSpeechBox.SetActive(true);
        playerSpeechTMP.text = "";

        foreach (char letter in dialogue.ToCharArray())
        {
            playerSpeechTMP.text += letter;
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
}