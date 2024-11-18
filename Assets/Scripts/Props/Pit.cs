using Abyss.Player;
using UnityEngine;

public class Pit : Construct
{
    public float Damage = 10, JumpDelay = 0.5f;

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.CompareTag("Player"))
        {
            collider2D.GetComponent<PlayerManager>().TakeHit(Damage);
            TakeDmg();
        }
    }
}
