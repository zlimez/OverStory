using Abyss.EventSystem;
using Abyss.Player;
using UnityEngine;

public class FirstRestRecLost : MonoBehaviour
{
    [SerializeField] Conversation firstDeath, firstRecover, firstLost;
    [SerializeField] DynamicEvent HadFirstRecover, HadFirstLost, HadFirstDeathConvo;

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (EventLedger.Instance == null)
            {
                Debug.LogWarning("Event ledger missing instance");
                return;
            }

            if (!EventLedger.Instance.HasOccurred(new GameEvent(HadFirstDeathConvo.EventName)) && EventLedger.Instance.GetEventCount(PlayEvents.Respawn) == 1)
            {
                EventLedger.Instance.Record(new GameEvent(HadFirstDeathConvo.EventName));
                DialogueManager.Instance.HardStartConvo(firstDeath);
            }
            var playerManager = other.GetComponent<PlayerManager>();

            if (EventLedger.Instance.HasOccurred(PlayEvents.Respawn))
            {
                if (!playerManager.BelowPurityThreshold && !EventLedger.Instance.HasOccurred(new GameEvent(HadFirstRecover.EventName)))
                {
                    EventLedger.Instance.Record(new GameEvent(HadFirstRecover.EventName));
                    DialogueManager.Instance.SoftStartConvo(firstRecover);
                }

                if (playerManager.BelowPurityThreshold && !EventLedger.Instance.HasOccurred(new GameEvent(HadFirstLost.EventName)))
                {
                    EventLedger.Instance.Record(new GameEvent(HadFirstLost.EventName));
                    DialogueManager.Instance.SoftStartConvo(firstLost);
                }
            }
        }
    }
}
