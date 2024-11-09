using Abyss;
using Abyss.Environment.Enemy;
using Abyss.EventSystem;
using Abyss.Player;
using UnityEngine;

public class Pit : Construct
{
    [SerializeField] float damage = 10, jumpDelay = 0.5f;
    [SerializeField] DynamicEvent damageBugEvent;

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        TakeDmg();
        if (collider2D.CompareTag("Player"))
            collider2D.GetComponent<PlayerManager>().TakeHit(damage);
        else if (collider2D.gameObject.layer == (int)AbyssSettings.Layers.Enemy)
        {
            if (collider2D.CompareTag("Bug"))
                EventManager.InvokeEvent(new GameEvent(damageBugEvent.EventName), jumpDelay);
            collider2D.GetComponent<EnemyManager>().TakeHit(damage);
        }
        // TODO: Add code to emit event that makes bug wait longer before jumping up and lick wound anim
    }
}
