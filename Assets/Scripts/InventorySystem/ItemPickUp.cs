using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Item item; 
    bool picked = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision with: " + collision.gameObject.name); 
        if (collision.gameObject.CompareTag("Player")&&!picked)
        {
            picked = true;
            Pickup();
        }
    }
    void Pickup()
    {
        Debug.Log("Picked up " + item.itemName);
        
        Inventory.Instance.AddTo(item);

        Destroy(gameObject);
    }
}
