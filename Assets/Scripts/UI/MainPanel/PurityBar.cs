using Abyss.EventSystem;
using UnityEngine;
using UnityEngine.UI;

public class PurityBar : MonoBehaviour
{
    public Image PurityBarImage;

    void OnEnable()
    {
        EventManager.StartListening(PlayEvents.PurityChange, UpdatePurityBar);
        if (GameManager.Instance != null)
            UpdatePurityBar(GameManager.Instance.PlayerPersistence.PlayerAttr.Purity);
    }

    void OnDisable() => EventManager.StopListening(PlayEvents.PurityChange, UpdatePurityBar);

    void UpdatePurityBar(object input)
    {
        float purity = (float)input;
        float PurityPercentage = purity / 100f;
        PurityBarImage.fillAmount = PurityPercentage;
    }
}
