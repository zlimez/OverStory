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
        bool _isDefeated = false, _haveFightWithPlayer = false, _beAttacked = false;
        Choice _spare = new("Spare"), _kill = new("Kill");


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

            // Temp code, when game pause implemented should not need
            if (_isDefeated) return;

            Debug.Log($"{name} took {baseDamage} damage");
            health -= Mathf.Min(health, baseDamage);
            _isDefeated = health == 0;
            if (_isDefeated) // Can spare enemy
            {
                OnDefeated?.Invoke();
                OnStrikePlayer = null;
                _spare.OnSelected += Spare;
                _kill.OnSelected += Kill;
                ChoiceManager.Instance.StartChoice(_spare, _kill);
            }
        }

        void Kill()
        {
            attributes.isAlive = false;
            OnDeath?.Invoke();
            // ActionPurity -10
            EventManager.InvokeEvent(PlayEvents.PlayerActionPurityChange, -10f);
            Drop();
            Destroy(gameObject);
        }

        void Spare() => attributes.friendliness += 2.0f;

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
