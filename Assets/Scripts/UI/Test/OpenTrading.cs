using UnityEngine;
using UnityEngine.Events;

public class OpenTrading : MonoBehaviour
{
    public UnityEvent onTrading;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            onTrading.Invoke();
        }
    }
}
