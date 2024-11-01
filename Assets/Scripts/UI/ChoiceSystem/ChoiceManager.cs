using UnityEngine;
using UnityEngine.EventSystems;
using Abyss.Utils;
using UnityEngine.InputSystem;
using TMPro;

public class ChoiceManager : Singleton<ChoiceManager>
{
    public bool InChoice { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private GameObject[] choiceButtons;
    [SerializeField] private GameObject choicePanel;

    Choice[] _choices;
    int _selectedInd = -1;

    void Start() => choicePanel.SetActive(false);

    public void StartChoice(params Choice[] choices)
    {
        if (UiStatus.IsDisabled) return;
        InChoice = true;
        _choices = choices;
        PopulateChoices();
    }

    public void SetChoice(int index)
    {
        _selectedInd = index;
        // EventSystem.current.SetSelectedGameObject(choiceButtons[index]);
    }

    void PopulateChoices()
    {
        GameManager.Instance.UiStatus.OpenUI();
        InChoice = true;
        choicePanel.SetActive(true);
        _selectedInd = 0;

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            GameObject choiceButton = choiceButtons[i];
            if (i < _choices.Length)
            {
                choiceButton.SetActive(true);
                choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = _choices[i].ChoiceText;
                // if (i == 0)
                // {
                //     EventSystem.current.SetSelectedGameObject(null);
                //     EventSystem.current.SetSelectedGameObject(choiceButton);
                // }
            }
            else choiceButton.SetActive(false);
        }
    }

    public void ConfirmChoice(InputAction.CallbackContext context)
    {
        if (context.performed && InChoice && _selectedInd != -1)
        {
            _choices[_selectedInd].OnSelected?.Invoke();
            Close();
        }
    }

    public void Close()
    {
        // Don't change UiStatus to !isOpen if the choice is followed by Dialogue or Item Selection
        // if (!DialogueManager.Instance.InDialogue)
        GameManager.Instance.UiStatus.CloseUI();

        _selectedInd = -1;
        choicePanel.SetActive(false);
        for (int i = 0; i < choiceButtons.Length; i++)
            choiceButtons[i].SetActive(false);
        InChoice = false;
    }
}