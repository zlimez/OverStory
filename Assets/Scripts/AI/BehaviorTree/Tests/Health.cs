using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int health;

    public bool TakeHit(int hitpoints) {
        health -= hitpoints;
        bool isDead = health <= 0;
        if (isDead) Destroy(gameObject);
        return isDead;
    }
}
