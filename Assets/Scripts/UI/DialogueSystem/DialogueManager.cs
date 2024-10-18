using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Abyss.EventSystem;
using Abyss.Utils;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueManager : Singleton<DialogueManager>
{

#if UNITY_EDITOR
    // Set this to true (in Master scene Dialogue) to allow skipping even when it is the first time going through that convo.
    public bool AllowDebugSkips = false;
#endif

    public bool InDialogue { get; private set; }

    [SerializeField] Speaker defaultSpeaker;
    [SerializeField] float speedMod;
    [SerializeField] float defaultCharInterval = 0.005f;
    float charInterval;

    [Header("UI References")]
    [SerializeField] GameObject dialogBox;
    [SerializeField] TextMeshProUGUI speakerName;
    [SerializeField] TextMeshProUGUI dialog;
    [SerializeField] Image leftSpeakerImg;
    [SerializeField] Image rightSpeakerImg;

    private int currInd;
    private Conversation currConvo;
    private bool isCurrLinePrinting;
    private bool isCentered;
    private Coroutine dialogLineCoroutine;

    public delegate void OnDialogFinished();
    private event OnDialogFinished EndDialogue;

    protected override void Awake()
    {
        base.Awake();
        PreserveSpriteAspect();
    }

    void PreserveSpriteAspect()
    {
        leftSpeakerImg.preserveAspect = true;
        rightSpeakerImg.preserveAspect = true;
    }


    public void StartConvo(Conversation convo, OnDialogFinished callback = null)
    {
        if (!CanStartConvo(convo)) return;

        PrepConvoUI(convo);
        BeginDialog(convo);

        // Add the callback to the EndDialogue event if it is not null
        if (callback != null)
            EndDialogue += callback;
    }

    public void StartAutoConvo(Conversation convo, OnDialogFinished callback = null)
    {
        StartConvo(convo, callback);
        StartCoroutine(AutoRead());
    }

    bool CanStartConvo(Conversation convo)
    {
        if (UiStatus.IsDisabled())
        {
            Debug.Log($"{convo.name} not started because scene in transition");
            return false;
        }

        return convo != null;
    }

    void PrepConvoUI(Conversation convo)
    {
        EventManager.InvokeEvent(UIEvents.DialogStarted);

        dialogBox.SetActive(true);
        GameManager.Instance.UiStatus.OpenUI();

        currInd = 0;
        currConvo = convo;
        speakerName.text = "";
        dialog.text = "";
        if (convo.LeftSpeaker != null && convo.LeftSpeaker.Sprite != null)
            leftSpeakerImg.sprite = convo.LeftSpeaker.Sprite;
        else leftSpeakerImg.sprite = defaultSpeaker.Sprite;
        if (convo.RightSpeaker != null && convo.RightSpeaker.Sprite != null)
            rightSpeakerImg.sprite = convo.RightSpeaker.Sprite;
        else rightSpeakerImg.sprite = defaultSpeaker.Sprite;
    }

    void BeginDialog(Conversation convo)
    {
        InDialogue = true;
        ReadNext();
    }

    public void HandleNext(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!isCurrLinePrinting)
                ReadNext();
            else FlashCurrLine();
        }
    }

    void FlashCurrLine()
    {
        if (dialogLineCoroutine != null)
            StopCoroutine(dialogLineCoroutine);

        DialogueLine currLine = currConvo.AllLines[currInd - 1];
        dialog.text = currLine.Dialogue;

        isCurrLinePrinting = false;
    }

    bool IsEndOfDialogue => currInd == currConvo.AllLines.Length;

    public void ReadNext()
    {
        if (IsEndOfDialogue)
            OnEndDialogue();
        else ProcessCurrLine();
    }

    void ProcessCurrLine()
    {
        isCurrLinePrinting = true;

        if (dialogLineCoroutine != null)
            StopCoroutine(dialogLineCoroutine);

        DialogueLine currLine = currConvo.AllLines[currInd];
        SetTypeSpeed(currLine);
        isCentered = currLine.IsCentered;

        dialogLineCoroutine = StartCoroutine(DisplayLine(currLine.Dialogue));

        UpdateSpeakerUI(currLine);
        PlayLineAudio(currLine);

        currInd++;
    }

    void SetTypeSpeed(DialogueLine currLine) => charInterval = defaultCharInterval / speedMod;

    void UpdateSpeakerUI(DialogueLine currentLine)
    {
        if (currentLine.IsLeft)
            UpdateLeftSpeakerUI(currentLine);
        else UpdateRightSpeakerUI(currentLine);

        if (!string.IsNullOrEmpty(currentLine.Name))
            speakerName.text = currentLine.Name;
    }

    void UpdateLeftSpeakerUI(DialogueLine currLine)
    {
        Speaker currSpeaker = currLine.Speaker != null ? currLine.Speaker : currConvo.LeftSpeaker;
        leftSpeakerImg.color = new Color32(255, 255, 255, 255);
        leftSpeakerImg.sprite = currSpeaker.Sprite;
        rightSpeakerImg.color = new Color32(110, 110, 110, 255);
        speakerName.text = currSpeaker.SpeakerName;
    }

    void UpdateRightSpeakerUI(DialogueLine currLine)
    {
        Speaker currSpeaker = currLine.Speaker != null ? currLine.Speaker : currConvo.RightSpeaker;
        rightSpeakerImg.color = new Color32(255, 255, 255, 255);
        rightSpeakerImg.sprite = currSpeaker.Sprite;
        leftSpeakerImg.color = new Color32(110, 110, 110, 255);
        speakerName.text = currSpeaker.SpeakerName;
    }

    public void PlayLineAudio(DialogueLine currentLine)
    {
        if (currentLine.Audio != null)
            AudioManager.Instance.PlaySFX(currentLine.Audio);
    }


    private IEnumerator DisplayLine(string line)
    {
        dialog.text = "";
        dialog.alignment = isCentered ? TextAlignmentOptions.Center : TextAlignmentOptions.TopLeft;

        foreach (char letter in line.ToCharArray())
        {
            yield return new WaitForSeconds(charInterval);
            dialog.text += letter;
        }
        isCurrLinePrinting = false;
    }
    private IEnumerator AutoRead(float waitDuration = 0.5f)
    {
        while (currInd != currConvo.AllLines.Length)
        {
            yield return new WaitWhile(() => isCurrLinePrinting);
            yield return new WaitForSeconds(waitDuration);
            ReadNext();
        }
        yield return new WaitWhile(() => isCurrLinePrinting);
        yield return new WaitForSeconds(waitDuration);
        OnEndDialogue();
    }

    private IEnumerator Skip()
    {
        ReadNext();
        yield return new WaitForSeconds(10 * Time.deltaTime);
    }

    void OnEndDialogue()
    {
        InDialogue = false;
        dialog.text = "";
        if (dialogLineCoroutine != null)
            StopCoroutine(dialogLineCoroutine);

        // if (!currConvo.EndWithChoice)
        // {
        CloseDialogueUI();
        // }

        EndDialogue?.Invoke();
        EndDialogue = null;
    }

    void CloseDialogueUI()
    {
        // Don't change UiStatus to !isOpen if the Dialogue is followed by Inventory opening or Choice
        // if (!ChoiceManager.Instance.InChoice && !InventoryUI.Instance.isItemSelectMode)
        GameManager.Instance.UiStatus.CloseUI();
        dialogBox.SetActive(false);
        Input.ResetInputAxes();
    }
}