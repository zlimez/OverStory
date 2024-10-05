using System.Collections;
using System.Collections.Generic;
using Abyss.EventSystem;
using Abyss.Player;
using UnityEngine;
using UnityEngine.UI;

public class PurityBar : MonoBehaviour
{
    // public PlayerAttr playerAttributes; 
    public Image PurityBarImage;
    // public float purity = (float)65;

    // void Start()
    // {
    //     UpdatePurityBar();
    // }

    void OnEnable()
    {
        EventManager.StartListening(PlayEventCollection.PlayerPurityChange, UpdatePurityBar);
    }

    void OnDisable()
    {
        EventManager.StopListening(PlayEventCollection.PlayerPurityChange, UpdatePurityBar);
    }

    void UpdatePurityBar(object input)
    {
        float purity = (float)input;
        float PurityPercentage = purity / 100f;
        PurityBarImage.fillAmount = PurityPercentage;
    }

}


