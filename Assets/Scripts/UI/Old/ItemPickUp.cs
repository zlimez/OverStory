using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [SerializeField] Item item;
    [SerializeField] Conversation onPickupConvo;
    bool picked = false;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Collision with: " + collider.name);
        if (collider.CompareTag("Player") && !picked) Pickup();
    }

    bool Pickup()
    {
        if (!GameManager.Instance.Inventory.Enabled) return false;
        Debug.Log("Picked up " + item.itemName);
        if (onPickupConvo != null) DialogueManager.Instance.HardStartConvo(onPickupConvo);
        picked = true;
        GameManager.Instance.Inventory.Add(item);
        Destroy(gameObject);
        return true;
    }
}
