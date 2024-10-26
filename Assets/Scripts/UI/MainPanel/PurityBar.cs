using Abyss.EventSystem;
using UnityEngine;
using UnityEngine.UI;

public class PurityBar : MonoBehaviour
{
    public Image PurityBarImage;

    void OnEnable()
    {
        if (GameManager.Instance != null)
            Load();
        else EventManager.StartListening(SystemEvents.SystemsReady, Load);
        EventManager.StartListening(PlayEvents.PlayerPurityChange, UpdatePurityBar);
    }

    void OnDisable()
    {
        EventManager.StopListening(SystemEvents.SystemsReady, Load);
        EventManager.StopListening(PlayEvents.PlayerPurityChange, UpdatePurityBar);
    }

    void Load(object input = null)
    {
        UpdatePurityBar(GameManager.Instance.PlayerPersistence.PlayerAttr.Purity);
        EventManager.StopListening(PlayEvents.PlayerPurityChange, Load);
    }

    void UpdatePurityBar(object input)
    {
        float purity = (float)input;
        float PurityPercentage = purity / 100f;
        PurityBarImage.fillAmount = PurityPercentage;
    }

}


