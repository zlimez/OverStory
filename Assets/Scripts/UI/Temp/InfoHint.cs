using Abyss.EventSystem;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class InfoHint : MonoBehaviour
{
    public RectTransform infoPanel;
    public TextMeshProUGUI infoText;
    public float slideDuration;
    [SerializeField] Vector2 hiddenPos, visiblePos;

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
        string text = (string)obj;
        if (string.IsNullOrEmpty(text)) return;
        infoPanel.DOAnchorPos(visiblePos, slideDuration);
        infoText.text = text;
    }

    void HideInfoPanel(object obj)
    {
        infoPanel.DOAnchorPos(hiddenPos, slideDuration);
        infoText.text = "";
    }
}
