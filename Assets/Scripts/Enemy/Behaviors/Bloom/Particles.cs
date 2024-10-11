using Abyss.Environment.Enemy;
using Abyss.Player;
using UnityEngine;

namespace Abyss.Environment
{
    public class Particles : MonoBehaviour
    {
        [SerializeField] float damage;
        [SerializeField] EnemyManager enemyManager;
        ParticleSystem _ps;

        void Awake()
        {
            _ps = GetComponent<ParticleSystem>();
        }

        void OnEnable()
        {
            enemyManager.OnDeath += _ps.Stop;
        }

        void OnParticleCollision(GameObject other)
        {
            // NOTE: A bit of an antipattern against usual impl -> manager calls strike and listeners from combos determine the actual attack
            if (other.CompareTag("Player"))
                other.GetComponent<PlayerManager>().TakeHit(damage + enemyManager.attributes.strength);
        }

        void OnDisable()
        {
            enemyManager.OnDeath -= _ps.Stop;
        }
    }
}
