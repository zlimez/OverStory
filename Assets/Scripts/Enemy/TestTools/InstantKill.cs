using Environment.Enemy;
using UnityEngine;


public class InstantKill : MonoBehaviour
{
    public void Kill()
    {
        var enemyManager = GetComponent<EnemyManager>();
        enemyManager.TakeHit(enemyManager.health);
    }
}
