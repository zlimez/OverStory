using Abyss.EventSystem;
using Tuples;
using UnityEngine;
using Abyss.Settings;

namespace NPC
{
    public class NPCTrigger : MonoBehaviour
    {
        public static readonly string TriggerRec = "TriggerRec";
        [SerializeField] ActionNode interactRoot;
        [SerializeField] EventCondChecker condChecker;
        [SerializeField][Tooltip("Tail boolean refers to whether to record the event")] Pair<DynamicEvent, bool>[] eventsToTriggerAndRecord;

        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag(Tag.Player) && condChecker.IsMet())
            {
                GameEvent triggerEvent = new(TriggerRec + "/" + name);
                interactRoot.Execute(triggerEvent);
                foreach (var evt in eventsToTriggerAndRecord)
                {
                    EventManager.InvokeEvent(new GameEvent(evt.Head.EventName));
                    if (evt.Tail) EventLedger.Instance.Record(new GameEvent(evt.Head.EventName));
                }
            }
        }
    }
}
