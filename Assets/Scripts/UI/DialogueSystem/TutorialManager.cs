using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Abyss.EventSystem;
using Utils.Tuples;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    // [SerializeField] private float speedMod = 1f, defaultCharInterval = 0.005f;
    [SerializeField] private TextMeshProUGUI tutorialTMP;

    void OnEnable()
    {
        EventManager.StartListening(UIEvents.TutorialDisplay, Display);
        EventManager.StartListening(UIEvents.TutorialClose, Close);
    }

    void OnDisable()
    {
        EventManager.StopListening(UIEvents.TutorialDisplay, Display);
        EventManager.StopListening(UIEvents.TutorialClose, Close);
    }

    void Display(object args)
    {
        string text = (string)args;
        tutorialTMP.text = text;
    }

    void Close(object args)
    {
        string text = (string)args;
        if (tutorialTMP.text == text) tutorialTMP.text = "";
    }

}