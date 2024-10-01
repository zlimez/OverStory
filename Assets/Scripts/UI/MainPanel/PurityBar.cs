using System.Collections;
using System.Collections.Generic;
using Abyss.Player;
using UnityEngine;
using UnityEngine.UI;

public class PurityBar : MonoBehaviour
{
    // public PlayerAttr playerAttributes; 
    public Image PurityBarImage;
    public float purity = (float)60;

    void Start()
    {
        UpdatePurityBar();
    }

    void UpdatePurityBar()
    {
        // float PurityPercentage = playerAttributes.purity / 100f;
        float PurityPercentage = purity / 100f;
        PurityBarImage.fillAmount = PurityPercentage;
    }

}


