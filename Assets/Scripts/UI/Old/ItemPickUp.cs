using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Item item;
    bool picked = false;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Collision with: " + collider.name);
        if (collider.CompareTag("Player") && !picked)
        {
            picked = true;
            Pickup();
        }
    }
    void Pickup()
    {
        Debug.Log("Picked up " + item.itemName);
        GameManager.Instance.Inventory.AddTo(item);
        Destroy(gameObject);
    }
}
