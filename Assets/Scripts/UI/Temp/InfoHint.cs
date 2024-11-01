using Abyss.EventSystem;
using TMPro;
using UnityEngine;

public class InfoHint : MonoBehaviour
{
    public GameObject infoPanel;
    public TextMeshProUGUI infoText;

    void OnEnable()
    {
        EventManager.StartListening(PlayEvents.InteractableEntered, ShowInfoPanel);
        EventManager.StartListening(PlayEvents.InteractableExited, HideInfoPanel);
    }

    void OnDisable()
    {
        EventManager.StopListening(PlayEvents.InteractableEntered, ShowInfoPanel);
        EventManager.StopListening(PlayEvents.InteractableExited, HideInfoPanel);
    }

    void ShowInfoPanel(object obj)
    {
        infoPanel.SetActive(true);
        infoText.text = (string)obj;
    }

    void HideInfoPanel(object obj)
    {
        infoPanel.SetActive(false);
        infoText.text = "";
    }
}
