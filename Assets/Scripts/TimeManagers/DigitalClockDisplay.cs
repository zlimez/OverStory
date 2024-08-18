using UnityEngine;
using TMPro;
using Chronellium.TimeManagers;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DigitalClockDisplay : MonoBehaviour
{
    [SerializeField] private Clock clock;

    private TextMeshProUGUI _textMeshPro;

    private void Awake()
    {
        _textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        if (clock != null)
        {
            clock.OnClockTick.AddListener(UpdateDisplay);
        }
    }

    private void OnDisable()
    {
        if (clock != null)
        {
            clock.OnClockTick.RemoveListener(UpdateDisplay);
        }
    }

    private void UpdateDisplay(int hours, int minutes, int seconds)
    {
        _textMeshPro.text = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
    }
}
