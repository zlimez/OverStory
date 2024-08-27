using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abyss.EventSystem;
using TMPro;


public class GameControlUi : MonoBehaviour
{
    [SerializeField] GameObject gameControl, button;
    [SerializeField] TMP_Text text;
    [SerializeField, TextArea(3, 5)] string startingInstruction, skipInstruction;
    public static GameControlUi Instance;
    // This indicates whether the button is active
    // The button is deactivated in Cutscenes by CutSceneGameControl
    public bool isButtonActive = true;

    void Awake() 
    {
        if (Instance == null)
            Instance = this;
    
    }

    public void Toggle() 
    {
        if (gameControl.activeInHierarchy)
        {
            Close();
        }
        else
        {
            Open();
        }
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
        if (UiStatus.IsOpen)
            return;
        text.text = startingInstruction;

        UiStatus.OpenUI();
        gameControl.SetActive(true);
    }

    public void Close() 
    {
        UiStatus.CloseUI();
        gameControl.SetActive(false);
    }

    public void CloseIndependently(object o = null) 
    {
        // Called when dialog is activated
        // UiStatus.CloseUI() not called as the game is still in dialog
        gameControl.SetActive(false);
    }
}
