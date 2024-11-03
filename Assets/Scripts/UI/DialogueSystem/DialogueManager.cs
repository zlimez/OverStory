using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Abyss.EventSystem;
using Abyss.Utils;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class DialogueManager : Singleton<DialogueManager>
{

#if UNITY_EDITOR
    // Set this to true (in Master scene Dialogue) to allow skipping even when it is the first time going through that convo.
    public bool AllowDebugSkips = false;
#endif

    public bool InDialogue { get; private set; }

    [SerializeField] float speedMod, defaultCharInterval = 0.005f;
    float charInterval;

    [Header("UI References")]
    [SerializeField] GameObject dialogBox;
    [SerializeField] TextMeshProUGUI speakerName, dialog;
    [SerializeField] Image leftSpeakerImg, rightSpeakerImg;

    int currInd = -1;
    Queue<Conversation> queuedConvos = new();
    Conversation currConvo;
    bool isCurrLinePrinting, isCentered;
    Coroutine dialogLineCoroutine;

    public delegate void OnDialogFinished();
    private event OnDialogFinished OnEndDialogue;

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

    public void SoftStartConvo(Conversation convo)
    {
        if (!GameManager.Instance.UI.Open(UiController.Type.Dialogue, KillDialog)) return;
        queuedConvos.Enqueue(convo);
        if (queuedConvos.Count == 1)
            StartConvo(convo);
    }

    public void HardStartConvo(Conversation convo, OnDialogFinished callback = null)
    {
        if (!GameManager.Instance.UI.Open(UiController.Type.Dialogue, KillDialog)) return;
        queuedConvos.Clear();
        queuedConvos.Enqueue(convo);
        StartConvo(convo, callback);
    }

    void StartConvo(Conversation convo, OnDialogFinished callback = null)
    {
        PrepConvoUI(convo);
        BeginDialog();

        if (callback != null) OnEndDialogue += callback;
    }

    public void StartAutoConvo(Conversation convo, OnDialogFinished callback = null)
    {
        HardStartConvo(convo, callback);
        StartCoroutine(AutoRead());
    }

    void PrepConvoUI(Conversation convo)
    {
        EventManager.InvokeEvent(UIEvents.DialogStarted);
        dialogBox.SetActive(true);

        currInd = 0;
        currConvo = convo;
        speakerName.text = "";
        dialog.text = "";
        if (convo.LeftSpeaker != null && convo.LeftSpeaker.Sprite != null)
            leftSpeakerImg.sprite = convo.LeftSpeaker.Sprite;
        else
        {
            leftSpeakerImg.sprite = null;
            leftSpeakerImg.color = ColorUtils.Transparent;
        }
        if (convo.RightSpeaker != null && convo.RightSpeaker.Sprite != null)
            rightSpeakerImg.sprite = convo.RightSpeaker.Sprite;
        else
        {
            rightSpeakerImg.sprite = null;
            rightSpeakerImg.color = ColorUtils.Transparent;
        }
    }

    void BeginDialog()
    {
        InDialogue = true;
        ReadNext();
    }

    public void HandleNext(InputAction.CallbackContext context)
    {
        if (context.performed && InDialogue)
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
        {
            queuedConvos.Dequeue();
            EndDialog();
        }
        else ProcessCurrLine();
    }

    void ProcessCurrLine()
    {
        isCurrLinePrinting = true;

        if (dialogLineCoroutine != null)
            StopCoroutine(dialogLineCoroutine);

        DialogueLine currLine = currConvo.AllLines[currInd];
        charInterval = defaultCharInterval / speedMod;
        isCentered = currLine.IsCentered;

        dialogLineCoroutine = StartCoroutine(DisplayLine(currLine.Dialogue));

        UpdateSpeakerUI(currLine);
        PlayLineAudio(currLine);

        currInd++;
    }

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
        if (leftSpeakerImg.sprite != null) leftSpeakerImg.color = new Color32(255, 255, 255, 255);
        if (currSpeaker != null)
        {
            leftSpeakerImg.sprite = currSpeaker.Sprite;
            speakerName.text = currSpeaker.SpeakerName;
        }
        if (rightSpeakerImg.sprite != null) rightSpeakerImg.color = new Color32(110, 110, 110, 255);
    }

    void UpdateRightSpeakerUI(DialogueLine currLine)
    {
        Speaker currSpeaker = currLine.Speaker != null ? currLine.Speaker : currConvo.RightSpeaker;
        if (rightSpeakerImg.sprite != null) rightSpeakerImg.color = new Color32(255, 255, 255, 255);
        if (currSpeaker != null)
        {
            rightSpeakerImg.sprite = currSpeaker.Sprite;
            speakerName.text = currSpeaker.SpeakerName;
        }
        if (leftSpeakerImg.sprite != null) leftSpeakerImg.color = new Color32(110, 110, 110, 255);
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
            yield return new WaitForSecondsRealtime(charInterval);
            dialog.text += letter;
        }
        isCurrLinePrinting = false;
    }
    private IEnumerator AutoRead(float waitDuration = 0.5f)
    {
        while (currInd != currConvo.AllLines.Length)
        {
            yield return new WaitWhile(() => isCurrLinePrinting);
            yield return new WaitForSecondsRealtime(waitDuration);
            ReadNext();
        }
        yield return new WaitWhile(() => isCurrLinePrinting);
        yield return new WaitForSecondsRealtime(waitDuration);
        EndDialog();
    }

    private IEnumerator Skip()
    {
        ReadNext();
        yield return new WaitForSecondsRealtime(10 * Time.deltaTime);
    }

    void EndDialog()
    {
        InDialogue = false;
        dialog.text = "";
        currInd = -1;
        if (dialogLineCoroutine != null)
            StopCoroutine(dialogLineCoroutine);

        OnEndDialogue?.Invoke();
        OnEndDialogue = null;
        if (queuedConvos.Count > 0)
            StartConvo(queuedConvos.Peek());
        else CloseDialogUI();
    }

    void CloseDialogUI()
    {
        GameManager.Instance.UI.Close();
        dialogBox.SetActive(false);
    }

    void KillDialog()
    {
        InDialogue = false;
        dialog.text = "";
        currInd = -1;
        if (dialogLineCoroutine != null)
            StopCoroutine(dialogLineCoroutine);

        dialogBox.SetActive(false);
        queuedConvos.Clear();
    }
}