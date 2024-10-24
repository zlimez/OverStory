using Abyss.EventSystem;
using UnityEngine;
using UnityEngine.UI;

public class PurityBar : MonoBehaviour
{
    public Image PurityBarImage;

    void OnEnable() => EventManager.StartListening(PlayEvents.PlayerPurityChange, UpdatePurityBar);
    void OnDisable() => EventManager.StopListening(PlayEvents.PlayerPurityChange, UpdatePurityBar);

    void UpdatePurityBar(object input)
    {
        float purity = (float)input;
        float PurityPercentage = purity / 100f;
        PurityBarImage.fillAmount = PurityPercentage;
    }

}


