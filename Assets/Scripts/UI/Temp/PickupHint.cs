using Abyss.EventSystem;
using UnityEngine;

public class PickupHint : MonoBehaviour
{
    public GameObject infoPanel;

    void OnEnable()
    {
        EventManager.StartListening(PlayEventCollection.InteractableEntered, ShowInfoPanel);
        EventManager.StartListening(PlayEventCollection.InteractableExited, HideInfoPanel);
    }

    void OnDisable()
    {
        EventManager.StopListening(PlayEventCollection.InteractableEntered, ShowInfoPanel);
        EventManager.StopListening(PlayEventCollection.InteractableExited, HideInfoPanel);
    }

    void ShowInfoPanel(object obj = null)
    {
        infoPanel.SetActive(true);
    }

    void HideInfoPanel(object obj = null)
    {
        infoPanel.SetActive(false);
    }
}
