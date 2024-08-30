using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int _health;

    public bool TakeHit(int hitpoints) {
        _health -= hitpoints;
        bool isDead = _health <= 0;
        if (isDead) Destroy(gameObject);
        return isDead;
    }
}
