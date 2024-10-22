using Tuples;
using UnityEngine;

namespace Abyss.Environment.Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        public SpecyAttr specyAttr;
        public EnemyAttr attributes;
        public float health;
        public System.Action OnDefeated, OnDeath;
        public System.Action<float> OnStrikePlayer; // Subscribed by moveset in BT for the enemy
        [SerializeField] Pair<GameObject, int>[] drops;
        [SerializeField][Tooltip("Left and right endpoint where drops are spawned")] Pair<Transform, Transform> dropRange;
        bool _isDefeated = false;

        // TODO: Base damage from player, mods by enemy attributes/specy attr done here
        public void TakeHit(float baseDamage)
        {
            if (!attributes.isAlive) return;
            if (_isDefeated)
            {
                attributes.isAlive = false;
                OnDeath?.Invoke();
                Drop();
                return;
            }
            Debug.Log($"{name} took {baseDamage} damage");
            health -= Mathf.Min(health, baseDamage);
            _isDefeated = health == 0;
            if (_isDefeated) // Can spare enemy
            {
                OnDefeated?.Invoke();
                OnStrikePlayer = null;
            }
        }

        public void Strike()
        {
            OnStrikePlayer?.Invoke(attributes.strength);
        }

        void Drop()
        {
            foreach (var drop in drops)
                for (int i = 0; i < drop.Tail; i++)
                {
                    var dropPos = new Vector3(Random.Range(dropRange.Head.position.x, dropRange.Tail.position.x), dropRange.Head.position.y, 0);
                    Instantiate(drop.Head, dropPos, Quaternion.identity);
                }
        }

        // NOTE: If enemy always moving (enter/exit trigger), this is not required
        // void OnTriggerStay2D(Collider2D other)
        // {
        //     if (other.CompareTag("Player"))
        //         OnStrikePlayer?.Invoke(attributes.strength);
        // }
    }
}
