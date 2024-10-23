using Abyss.EventSystem;
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
        bool _haveFightWithPlayer = false;
        bool _beAttacked = false;


        // TODO: Base damage from player, mods by enemy attributes/specy attr done here
        public void TakeHit(float baseDamage)
        {
            if (!attributes.isAlive) return;
            if (!_haveFightWithPlayer)
            {
                attributes.friendliness -= 3.0f;
                _haveFightWithPlayer = true;
                _beAttacked = true;
            }
            if (_isDefeated)
            {
                attributes.isAlive = false;
                OnDeath?.Invoke();
                // ActionPurity -10
                EventManager.InvokeEvent(PlayEvents.PlayerActionPurityChange, -10);
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
                // Forgive
                attributes.friendliness += 2.0f;
            }
        }

        public void Strike()
        {
            if (!_haveFightWithPlayer) _haveFightWithPlayer = true;
            OnStrikePlayer?.Invoke(attributes.strength);
            // prority to reproduce
            if (GameManager.Instance.PlayerPersistence.JustDied) attributes.priority = true;
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
