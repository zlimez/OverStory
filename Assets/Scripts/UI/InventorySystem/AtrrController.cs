using Abyss.EventSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttrController : MonoBehaviour
{
    public TextMeshProUGUI Purity;
    public TextMeshProUGUI Strength;
    public TextMeshProUGUI Agility;
    public TextMeshProUGUI Intelligence;

    // void OnEnable() => UpdateAttr();
    void OnEnable()
    {
        if (GameManager.Instance == null)
            EventManager.StartListening(SystemEvents.SystemsReady, InitUpdateBagUI);
        else
        {
            UpdateAttr();
        }
    }

    void InitUpdateBagUI(object input = null)
    {
        UpdateAttr();
        EventManager.StopListening(SystemEvents.SystemsReady, InitUpdateBagUI);
    }


    void UpdateAttr()
    {
        UpdatePurity(GameManager.Instance.PlayerPersistence.PlayerAttr.Purity);
        UpdateStrength(GameManager.Instance.PlayerPersistence.PlayerAttr.Strength);
        UpdateAgility(GameManager.Instance.PlayerPersistence.PlayerAttr.Agility);
        UpdateIntelligence(GameManager.Instance.PlayerPersistence.PlayerAttr.Intelligence);
    }

    void UpdatePurity(float input)
    {
        int roundedPurity = Mathf.RoundToInt(input);
        Purity.text = roundedPurity + "%";
    }

    void UpdateStrength(float input)
    {
        int roundedStrength = Mathf.RoundToInt(input);
        Strength.text = roundedStrength.ToString();
    }
    void UpdateAgility(float input)
    {
        int roundedAgility = Mathf.RoundToInt(input);
        Agility.text = roundedAgility.ToString();
    }
    void UpdateIntelligence(float input)
    {
        int roundedIntelligence = Mathf.RoundToInt(input);
        Intelligence.text = roundedIntelligence.ToString();
    }


}


