using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Abyss.EventSystem;
using Abyss.Utils;

public class SecondaryDialogueManager : Singleton<SecondaryDialogueManager>
{

#if UNITY_EDITOR
    // Set this to true (in Master scene Dialogue) to allow skipping even when 
    // it is the first time going through that convo.
    // Only works in Unity Editor - will not be in build
    public bool AllowConvoSkipsForDebug = false;
#endif

    public bool InDialogue { get; private set; }

    [SerializeField] private Speaker defaultSpeaker;

    [Header("UI References")]
    [SerializeField] private GameObject showTimeline;
    [SerializeField] private GameObject hideTimeline;
    [SerializeField] private Text speakerName;
    [SerializeField] private Text dialogue;
    [SerializeField] private Image sprite;

    private int currentIndex;
    private SecondaryConversation currentConversation;
    private bool isCurrentLinePrinting;
    [SerializeField] private float typingSpeed;
    private const float DefaultTypingSpeed = 1000f;
    private const float DefaultWaitingTimeOffset = 3f;
    private Coroutine dialogueLineCoroutine;

    public delegate void DialogueFinishedCallback();
    private event DialogueFinishedCallback EndDialogue;

    protected override void Awake()
    {
        base.Awake();
        PreserveSpriteAspect();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && InDialogue)
        {
            StartCoroutine(Skip());
        }
    }

    void OnEnable()
    {
        EventManager.StartListening(SystemEventCollection.SceneTransitWithMasterStart, ForceCloseDialogueUI);
    }

    void OnDisable()
    {
        EventManager.StopListening(SystemEventCollection.SceneTransitWithMasterStart, ForceCloseDialogueUI);
    }

    private void PreserveSpriteAspect()
    {
        sprite.preserveAspect = true;
    }

    // Secondary Dialogue should always be automatic
    private void StartConversation(SecondaryConversation conversation, DialogueFinishedCallback callback = null)
    {
        if (!CanStartConversation(conversation))
        {
            Debug.LogWarning("Cannot start secondary conversation since Ui is disabled");
            return;
        }

        PrepareConversationUI(conversation);
        BeginDialogue(conversation);

        // Add the callback to the EndDialogue event if it is not null
        if (callback != null)
        {
            EndDialogue += callback;
        }
    }

    public void StartAutomaticConversation(SecondaryConversation conversation, DialogueFinishedCallback callback = null)
    {
        StartConversation(conversation, callback);
        StartCoroutine(AutomaticallyRead());
    }

    private bool CanStartConversation(SecondaryConversation conversation)
    {
        if (UiStatus.IsDisabled())
        {
            Debug.Log($"{conversation.name} not started because scene in transition");
            return false;
        }

        return conversation != null;
    }

    private void PrepareConversationUI(SecondaryConversation conversation)
    {
        EventManager.InvokeEvent(UIEventCollection.DialogStarted);

        currentIndex = 0;
        currentConversation = conversation;
        speakerName.text = "";
        dialogue.text = "";
    }

    private void BeginDialogue(SecondaryConversation conversation)
    {
        InDialogue = true;
        if (!showTimeline.activeInHierarchy)
        {
            showTimeline.SetActive(true);
        }

        ReadNext();
    }

    private bool IsEndOfDialogue()
    {
        return currentIndex == currentConversation.AllLines.Length;
    }

    public void ReadNext()
    {
        if (IsEndOfDialogue())
        {
            OnEndDialogue();
        }
        else
        {
            ProcessCurrentLine();
        }
    }

    private void ProcessCurrentLine()
    {
        isCurrentLinePrinting = true;

        if (dialogueLineCoroutine != null)
        {
            StopCoroutine(dialogueLineCoroutine);
        }

        SecondaryDialogueLine currentLine = currentConversation.AllLines[currentIndex];
        SetTypingSpeed(currentLine);

        dialogueLineCoroutine = StartCoroutine(DisplayLine(currentLine.Dialogue));

        UpdateSpeakerUI(currentLine);
        PlayLineAudio(currentLine);

        currentIndex++;
    }

    private void SetTypingSpeed(SecondaryDialogueLine currentLine)
    {

        typingSpeed = 1 / (DefaultTypingSpeed * Time.timeScale);
    }

    private void UpdateSpeakerUI(SecondaryDialogueLine currentLine)
    {
        Speaker currentSpeaker = currentLine.Speaker;
        if (currentSpeaker.SpeakerSprite != null)
        {
            sprite.sprite = currentSpeaker.SpeakerSprite;
        }
        speakerName.text = currentSpeaker.SpeakerName;
    }

    public void PlayLineAudio(SecondaryDialogueLine currentLine)
    {
        if (currentLine.Audio != null)
        {
            AudioManager.Instance.PlaySFX(currentLine.Audio);
        }
    }

    private IEnumerator DisplayLine(string line)
    {
        dialogue.text = "";

        foreach (char letter in line.ToCharArray())
        {
            yield return new WaitForSeconds(typingSpeed);
            dialogue.text += letter;
        }
        isCurrentLinePrinting = false;
    }

    private IEnumerator AutomaticallyRead()
    {
        while (currentIndex != currentConversation.AllLines.Length)
        {
            yield return new WaitWhile(() => isCurrentLinePrinting);
            yield return new WaitForSeconds(DefaultWaitingTimeOffset * (1 / Time.timeScale));
            ReadNext();
        }
        yield return new WaitWhile(() => isCurrentLinePrinting);
        yield return new WaitForSeconds(DefaultWaitingTimeOffset * (1 / Time.timeScale));
        OnEndDialogue();
    }

    private void OnEndDialogue()
    {
        InDialogue = false;
        dialogue.text = "";
        if (dialogueLineCoroutine != null)
        {
            StopCoroutine(dialogueLineCoroutine);
        }

        CloseDialogueUI();

        EndDialogue?.Invoke();
        EndDialogue = null;
    }

    private void CloseDialogueUI()
    {
        showTimeline.SetActive(false);
        hideTimeline.SetActive(true);

        GameEvent dialogCompleteEvent = new GameEvent($"Hector Finished Convo: {currentConversation.name}");
    }

    private void ForceCloseDialogueUI(object o = null)
    {
        if (!InDialogue)
        {
            return;
        }
        showTimeline.SetActive(false);
        hideTimeline.SetActive(true);

        // Event is still recorded to prevent unexpected behaviour when the player
        // cuts the dialog off through scene transition
        GameEvent dialogCompleteEvent = new GameEvent($"Hector Finished Convo: {currentConversation.name}");
    }

    private IEnumerator Skip()
    {
        ReadNext();
        yield return new WaitForSeconds(10 * Time.deltaTime);
    }
}