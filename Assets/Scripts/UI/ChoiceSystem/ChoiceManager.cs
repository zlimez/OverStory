using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Abyss.EventSystem;
using Abyss.Utils;

/// <summary>
/// Manages the display and functionality of choice buttons during dialogues.
/// </summary>
public class ChoiceManager : Singleton<ChoiceManager>
{
    public bool InChoice { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private GameObject[] choiceButtons;
    [SerializeField] private GameObject choiceHolder;

    private Choice[] choices;
    private int choiceIndex = -1;
    private bool isActive;
    private bool justSelected;

    private void OnEnable()
    {
        EventManager.StartListening(UIEventCollection.DialogStarted, Close);
    }

    private void OnDisable()
    {
        EventManager.StopListening(UIEventCollection.DialogStarted, Close);
    }

    private void Update()
    {
        if (UiStatus.IsDisabled()) return;

        HandleChoiceActivation();
        HandleChoiceSelection();
    }

    /// <summary>
    /// Begins displaying the given choices.
    /// </summary>
    /// <param name="choices">An array of Choice objects to be displayed.</param>
    public void StartChoice(params Choice[] choices)
    {
        if (UiStatus.IsDisabled()) return;
        if (justSelected)
        {
            justSelected = false;
            return;
        }
        isActive = true;
        this.choices = choices;
    }

    /// <summary>
    /// Sets the current choice based on the given index.
    /// </summary>
    /// <param name="index">The index of the choice in the array.</param>
    public void SetChoice(int index)
    {
        UiStatus.CloseUI();
        choiceIndex = index;
    }

    /// <summary>
    /// Activates the choice buttons based on the given choices.
    /// </summary>
    private void ActivateChoices()
    {
        UiStatus.OpenUI();
        InChoice = true;
        choiceHolder.SetActive(true);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            GameObject choiceButton = choiceButtons[i];
            if (i < choices.Length)
            {
                choiceButton.SetActive(true);
                choiceButton.GetComponentInChildren<Text>().text = choices[i].ChoiceText;
                choiceButton.GetComponentInChildren<Button>().interactable = choices[i].IsActivated;
            }
            else choiceButton.SetActive(false);
        }
    }

    /// <summary>
    /// Handles the activation of choices.
    /// </summary>
    private void HandleChoiceActivation()
    {
        if (isActive && !DialogueManager.Instance.InDialogue)
        {
            Debug.Assert(2 <= choices.Length && choices.Length <= 4);
            isActive = false;

            ActivateChoices();

            GameObject firstChoiceButton = choiceButtons[0].GetComponentInChildren<Button>().gameObject;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstChoiceButton);
        }
    }

    /// <summary>
    /// Handles the selection of choices.
    /// </summary>
    private void HandleChoiceSelection()
    {
        if ((Input.GetButtonDown("Submit") || Input.GetMouseButtonDown(0)) && choiceIndex != -1)
        {
            choices[choiceIndex].ChoiceSelectedEvent.Invoke(null);
            EndChoices();
        }
    }

    /// <summary>
    /// Ends the choice interaction and triggers the appropriate events.
    /// </summary>
    public void EndChoices()
    {
        GameEvent onChoiceSelected;

        if (choiceIndex != -1)
            onChoiceSelected = new GameEvent($"{Choice.EventPrefix}{choices[choiceIndex].ChoiceText}");
        else onChoiceSelected = new GameEvent("No Choice Selected");

        Close();
    }

    /// <summary>
    /// Closes the choice interaction and resets the UI elements.
    /// </summary>
    /// <param name="parameter">Optional parameter to match event listener signature, not used.</param>
    public void Close(object parameter = null)
    {
        // Don't change UiStatus to !isOpen if the choice is followed by Dialogue or Item Selection
        if (!DialogueManager.Instance.InDialogue)
            UiStatus.CloseUI();

        choiceIndex = -1;
        choiceHolder.SetActive(false);
        for (int i = 0; i < choiceButtons.Length; i++)
            choiceButtons[i].SetActive(false);
        InChoice = false;
        justSelected = true;
    }
}