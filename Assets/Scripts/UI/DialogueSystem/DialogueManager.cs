using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Abyss.EventSystem;
using Abyss.Utils;

public class DialogueManager : Singleton<DialogueManager>
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
    [SerializeField] private GameObject dialogBox;
    [SerializeField] private Text speakerName;
    [SerializeField] private Text dialogue;
    [SerializeField] private Image leftSprite;
    [SerializeField] private Image rightSprite;
    [SerializeField] private Image blackBackground;

    private int currentIndex;
    private Conversation currentConversation;
    private bool isCurrentLinePrinting;
    [SerializeField] private float typingSpeed;
    private const float DefaultTypingSpeed = 0.005f;
    private bool isCentered;
    private Coroutine dialogueLineCoroutine;
    public float speedModifier;

    public delegate void DialogueFinishedCallback();
    private event DialogueFinishedCallback EndDialogue;

    protected override void Awake()
    {
        base.Awake();
        PreserveSpriteAspect();
    }

    private void PreserveSpriteAspect()
    {
        leftSprite.preserveAspect = true;
        rightSprite.preserveAspect = true;
    }


    public void StartConversation(Conversation conversation, DialogueFinishedCallback callback = null)
    {
        if (!CanStartConversation(conversation)) return;

        PrepareConversationUI(conversation);
        SetInitialSpeakerSprites(conversation);
        BeginDialogue(conversation);

        // Add the callback to the EndDialogue event if it is not null
        if (callback != null)
        {
            EndDialogue += callback;
        }
    }

    public void StartAutomaticConversation(Conversation conversation, DialogueFinishedCallback callback = null)
    {
        StartConversation(conversation, callback);
        StartCoroutine(AutomaticallyRead());
    }

    private bool CanStartConversation(Conversation conversation)
    {
        if (UiStatus.IsDisabled())
        {
            Debug.Log($"{conversation.name} not started because scene in transition");
            return false;
        }

        return conversation != null;
    }

    private void PrepareConversationUI(Conversation conversation)
    {
        EventManager.InvokeEvent(UIEventCollection.DialogStarted);

        dialogBox.SetActive(true);
        UiStatus.OpenUI();

        currentIndex = 0;
        currentConversation = conversation;
        speakerName.text = "";
        dialogue.text = "";
        blackBackground.color = conversation.IsBlackBackground
            ? new Color32(0, 0, 0, 255)
            : new Color32(0, 0, 0, 50);
    }

    private void SetInitialSpeakerSprites(Conversation conversation)
    {
        leftSprite.sprite = conversation.StartingLeftSpeaker?.SpeakerSprite ?? defaultSpeaker.SpeakerSprite;
        rightSprite.sprite = conversation.StartingRightSpeaker?.SpeakerSprite ?? defaultSpeaker.SpeakerSprite;
    }

    private void BeginDialogue(Conversation conversation)
    {
        InDialogue = true;
        ReadNext();
    }

    private void HandleDialogButtonClick()
    {
        if (!isCurrentLinePrinting)
            ReadNext();
        else SkipCurrentLine();
    }

    private void SkipCurrentLine()
    {
        if (dialogueLineCoroutine != null)
        {
            StopCoroutine(dialogueLineCoroutine);
        }

        DialogueLine currentLine = currentConversation.AllLines[currentIndex - 1];
        dialogue.text = currentLine.Dialogue;

        isCurrentLinePrinting = false;
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

        DialogueLine currentLine = currentConversation.AllLines[currentIndex];
        SetTypingSpeed(currentLine);
        isCentered = currentLine.IsCentered;

        dialogueLineCoroutine = StartCoroutine(DisplayLine(currentLine.Dialogue));

        UpdateSpeakerUI(currentLine);
        PlayLineAudio(currentLine);

        currentIndex++;
    }

    private void SetTypingSpeed(DialogueLine currentLine)
    {
        typingSpeed = DefaultTypingSpeed * speedModifier
            * (1 / (currentLine.TextSpeed + 0.0001f))
            * (1 / (currentConversation.TextSpeed + 0.0001f));
    }

    private void UpdateSpeakerUI(DialogueLine currentLine)
    {
        if (currentLine.IsLeft)
        {
            UpdateLeftSpeakerUI(currentLine);
        }
        else
        {
            UpdateRightSpeakerUI(currentLine);
        }

        if (!string.IsNullOrEmpty(currentLine.Name))
        {
            speakerName.text = currentLine.Name;
        }
    }

    private void UpdateLeftSpeakerUI(DialogueLine currentLine)
    {
        Speaker currentSpeaker = currentLine.Speaker ?? currentConversation.StartingLeftSpeaker;
        leftSprite.color = new Color32(255, 255, 255, 255);
        leftSprite.sprite = currentSpeaker.SpeakerSprite;
        rightSprite.color = new Color32(110, 110, 110, 255);
        speakerName.text = currentSpeaker.SpeakerName;
    }

    private void UpdateRightSpeakerUI(DialogueLine currentLine)
    {
        Speaker currentSpeaker = currentLine.Speaker ?? currentConversation.StartingRightSpeaker;
        rightSprite.color = new Color32(255, 255, 255, 255);
        rightSprite.sprite = currentSpeaker.SpeakerSprite;
        leftSprite.color = new Color32(110, 110, 110, 255);
        speakerName.text = currentSpeaker.SpeakerName;
    }

    public void PlayLineAudio(DialogueLine currentLine)
    {
        if (currentLine.Audio != null)
        {
            AudioManager.Instance.PlaySFX(currentLine.Audio);
        }
    }


    private IEnumerator DisplayLine(string line)
    {
        dialogue.text = "";
        dialogue.alignment = isCentered ? TextAnchor.MiddleCenter : TextAnchor.UpperLeft;

        foreach (char letter in line.ToCharArray())
        {
            yield return new WaitForSeconds(typingSpeed);
            dialogue.text += letter;
        }
        isCurrentLinePrinting = false;
    }
    private IEnumerator AutomaticallyRead(float waitDuration = 0.5f)
    {
        while (currentIndex != currentConversation.AllLines.Length)
        {
            yield return new WaitWhile(() => isCurrentLinePrinting);
            yield return new WaitForSeconds(waitDuration);
            ReadNext();
        }
        yield return new WaitWhile(() => isCurrentLinePrinting);
        yield return new WaitForSeconds(waitDuration);
        OnEndDialogue();
    }

    private IEnumerator Skip()
    {
        ReadNext();
        yield return new WaitForSeconds(10 * Time.deltaTime);
    }

    private void OnEndDialogue()
    {
        InDialogue = false;
        dialogue.text = "";
        if (dialogueLineCoroutine != null)
        {
            StopCoroutine(dialogueLineCoroutine);
        }
        if (!currentConversation.EndWithChoice)
        {
            CloseDialogueUI();
        }

        EndDialogue?.Invoke();
        EndDialogue = null;
    }

    private void CloseDialogueUI()
    {
        // Don't change UiStatus to !isOpen if the Dialogue is followed by Inventory opening or Choice
        // if (!ChoiceManager.Instance.InChoice && !InventoryUI.Instance.isItemSelectMode)
        UiStatus.CloseUI();
        dialogBox.SetActive(false);
        Input.ResetInputAxes();

        GameEvent dialogCompleteEvent = new GameEvent($"Hector Finished Convo: {currentConversation.name}");
    }
}