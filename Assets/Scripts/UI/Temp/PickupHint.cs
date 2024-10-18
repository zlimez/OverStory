using Abyss.EventSystem;
using UnityEngine;

public class PickupHint : MonoBehaviour
{
    public GameObject infoPanel;

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

    void ShowInfoPanel(object obj = null) => infoPanel.SetActive(true);
    void HideInfoPanel(object obj = null) => infoPanel.SetActive(false);
}
