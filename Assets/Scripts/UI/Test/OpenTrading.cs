using Abyss.EventSystem;
using Abyss.Player;
using UnityEngine;

public class OpenTrading : MonoBehaviour
{
    public Tribe tribe;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision with: " + other.gameObject.name);
        if (other.CompareTag("Player"))
            EventManager.InvokeEvent(UIEvents.OpenTrading, (tribe, other.GetComponent<PlayerManager>().PlayerAttr));
    }
}
