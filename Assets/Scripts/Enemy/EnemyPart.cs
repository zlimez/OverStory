using UnityEngine;
using UnityEngine.Assertions;

namespace Abyss.Environment.Enemy
{
    public class EnemyPart : MonoBehaviour
    {
        [SerializeField] private EnemyManager enemyManager;

        void Awake()
        {
            enemyManager = GetComponentInParent<EnemyManager>();
#if DEBUG
            Assert.IsNotNull(enemyManager, "EnemyManager not found in parent");
            Assert.IsNotNull(GetComponent<Collider2D>(), "Collider2D not found");
#endif
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                enemyManager.Strike();
        }

        public int EnemyIntanceId => enemyManager.GetInstanceID();

        public void TakeHit(float baseDamage)
        {
            enemyManager.TakeHit(baseDamage);
        }
    }
}
