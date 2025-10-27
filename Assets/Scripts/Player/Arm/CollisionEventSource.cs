using System;
using UnityEngine;

public class CollisionEventSource : MonoBehaviour
{
    public Action<Collision2D> OnCollision;
    public Action<Collider2D> OnContact;
    void OnCollisionEnter2D(Collision2D col) => OnCollision?.Invoke(col);
    void OnTriggerEnter2D(Collider2D other) => OnContact?.Invoke(other);
}
