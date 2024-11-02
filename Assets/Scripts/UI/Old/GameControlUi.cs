using UnityEngine;
using TMPro;
using Abyss.Utils;


public class GameControlUi : Singleton<GameControlUi>
{
    [SerializeField] GameObject gameControl, button;
    [SerializeField] TMP_Text text;
    [SerializeField, TextArea(3, 5)] string startingInstruction, skipInstruction;

    // This indicates whether the button is active
    // The button is deactivated in Cutscenes by CutSceneGameControl
    public bool isButtonActive = true;

    public void Toggle()
    {
        if (gameControl.activeInHierarchy)
            Close();
        else Open();
    }

    public void ShowButton()
    {
        // Show the button after Cutsecne
        isButtonActive = true;
        button.SetActive(true);
    }

    public void HideButton()
    {
        // Hide the button when in Cutsecne
        Debug.Log("hide button");

        isButtonActive = false;
        button.SetActive(false);
    }

    public void Open()
    {
        if (GameManager.Instance.UI.IsOpen)
            return;
        text.text = startingInstruction;

        gameControl.SetActive(true);
    }

    public void Close()
    {
        GameManager.Instance.UI.Close();
        gameControl.SetActive(false);
    }

    public void CloseIndependently(object o = null)
    {
        // Called when dialog is activated
        // UiStatus.CloseUI() not called as the game is still in dialog
        gameControl.SetActive(false);
    }
}
