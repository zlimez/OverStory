using Abyss.EventSystem;
using UnityEngine;

public class ConvoTrigger : MonoBehaviour
{
    [SerializeField] Conversation conversation;
    [SerializeField][Tooltip("Events that must have occurred to trigger the conversation")] DynamicEvent[] eventConditions;
    [SerializeField] bool noRepeat;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && CheckEventConditionsMet())
        {
            DialogueManager.Instance.StartConvo(conversation);
            if (noRepeat) Destroy(gameObject);
        }
    }

    bool CheckEventConditionsMet()
    {
        foreach (DynamicEvent e in eventConditions)
            if (!EventLedger.Instance.HasOccurred(new GameEvent(e.EventName))) return false;
        return true;
    }
}
