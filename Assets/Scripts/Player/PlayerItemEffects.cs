using System.Collections;
using Abyss.EventSystem;
using Abyss.Player;
using Abyss.TimeManagers;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerItemEffects : MonoBehaviour
{
    [Header("Bandages")]
    [SerializeField] DynamicEvent basicBandageUsed;
    [SerializeField] float basicHealPortion;
    [SerializeField] float duration;

    PlayerManager _playerManager;

    void Awake() => _playerManager = GetComponent<PlayerManager>();

    void OnValidate() => Assert.IsTrue(basicHealPortion >= 0 && basicHealPortion <= 1, "Heal portion must be between 0 and 1");

    void OnEnable()
    {
        EventManager.StartListening(new GameEvent(basicBandageUsed.EventName), UseBasicBandage);
    }

    void OnDisable()
    {
        EventManager.StopListening(new GameEvent(basicBandageUsed.EventName), UseBasicBandage);
    }

    public void UseBasicBandage(object input = null) => StartCoroutine(Heal(basicHealPortion * PlayerAttr.MaxHealth));

    IEnumerator Heal(float healAmt)
    {
        float et = 0;
        while (et < duration)
        {
            float adjustedTFlow = Time.deltaTime * TimeCycle.Instance.SpeedMod;
            et += adjustedTFlow;
            _playerManager.UpdateHealth(healAmt * adjustedTFlow / duration);
            yield return null;
        }
    }
}
