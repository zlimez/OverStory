using UnityEngine;
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
        if (!GameManager.Instance.UI.Open(UI.Type.Choice, Clear)) return;
        InChoice = true;
        _choices = choices;
        PopulateChoices();
    }

    public void SetChoice(int index) => _selectedInd = index;

    void PopulateChoices()
    {
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
        Clear();
        GameManager.Instance.UI.Close();
    }

    void Clear()
    {
        _selectedInd = -1;
        choicePanel.SetActive(false);
        for (int i = 0; i < choiceButtons.Length; i++)
            choiceButtons[i].SetActive(false);
        InChoice = false;
    }
}