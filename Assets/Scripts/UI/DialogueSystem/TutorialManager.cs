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
        EventManager.Subscribe(UIEvents.TutorialDisplay, Display);
        EventManager.Subscribe(UIEvents.TutorialClose, Close);
    }

    void OnDisable()
    {
        EventManager.Unsubscribe(UIEvents.TutorialDisplay, Display);
        EventManager.Unsubscribe(UIEvents.TutorialClose, Close);
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