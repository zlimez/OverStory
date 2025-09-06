using System.Collections;
using Abyss.EventSystem;
using Abyss.Player;
using Abyss.TimeManagers;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerItemEffects : MonoBehaviour
{
	[Header("Bandages")]
	[SerializeField] float duration;
	[SerializeField] DynamicEvent basicBandageUsed;
	[SerializeField] float basicHealPortion;

	// Good bandages
	[SerializeField] DynamicEvent goodBandageUsed;
	[SerializeField] float goodHealPortion;

	// Master bandages
	[SerializeField] DynamicEvent masterBandageUsed;
	[SerializeField] float masterHealPortion;

	PlayerManager _playerManager;

	private void Awake() => _playerManager = GetComponent<PlayerManager>();

	private void OnValidate() => Assert.IsTrue(basicHealPortion >= 0 && basicHealPortion <= 1, "Heal portion must be between 0 and 1");

	private void OnEnable()
	{
		EventManager.Subscribe(new GameEvent(basicBandageUsed.EventName), UseBasicBandage);
		EventManager.Subscribe(new GameEvent(goodBandageUsed.EventName), UseGoodBandage);
		EventManager.Subscribe(new GameEvent(masterBandageUsed.EventName), UseMasterBandage);
		EventManager.Subscribe(PlayEvents.LureUsed, PlaceLure);
	}

	private void OnDisable()
	{
		EventManager.Unsubscribe(new GameEvent(basicBandageUsed.EventName), UseBasicBandage);
		EventManager.Subscribe(new GameEvent(goodBandageUsed.EventName), UseGoodBandage);
		EventManager.Subscribe(new GameEvent(masterBandageUsed.EventName), UseMasterBandage);
		EventManager.Unsubscribe(PlayEvents.LureUsed, PlaceLure);
	}

	public void UseBasicBandage(object input = null) => StartCoroutine(Heal(basicHealPortion * PlayerAttr.MaxHealth));
	public void UseGoodBandage(object input = null) => StartCoroutine(Heal(goodHealPortion * PlayerAttr.MaxHealth));
	public void UseMasterBandage(object input = null) => StartCoroutine(Heal(masterHealPortion * PlayerAttr.MaxHealth));

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

	void PlaceLure(object input = null)
	{
		(float radius, string specyName) = ((float, string))input;
		EventManager.InvokeEvent(new GameEvent(LurePlacedEvtNameFor(specyName)), (radius, transform.position));
	}

	public static string LurePlacedEvtNameFor(string specyName) => new(PlayEvents.LurePlaced.ToString() + " " + specyName);
}
