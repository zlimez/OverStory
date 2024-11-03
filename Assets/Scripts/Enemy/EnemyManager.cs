using System;
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
        public Action OnDefeated, OnDeath;
        public Action<float> OnStrikePlayer; // Subscribed by moveset in BT for the enemy

        [SerializeField] SpriteFlash damageFlash;

        [Header("Drops")]
        [SerializeField][Tooltip("Number of strikes aft health depleted that will kill this enemy")] int hitsToKill = 2;
        [SerializeField] Pair<GameObject, int>[] drops;
        [SerializeField][Tooltip("Left and right endpoint where drops are spawned")] Pair<Transform, Transform> dropRange;

        bool _isDefeated = false, _haveFightWithPlayer = false, _beAttacked = false;
        int _postDefeatStrikes = 0;


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

            if (_isDefeated && ++_postDefeatStrikes == hitsToKill)
            {
                attributes.isAlive = false;
                OnDeath?.Invoke();
                EventManager.InvokeEvent(PlayEvents.PlayerActionPurityChange, -10f);
                Drop();
                Destroy(gameObject);
            }

            Debug.Log($"{name} took {baseDamage} damage");
            health -= Mathf.Min(health, baseDamage);
            if (damageFlash != null) damageFlash.StartFlash();

            _isDefeated = health == 0;
            if (_isDefeated)
            {
                OnDefeated?.Invoke();
                OnStrikePlayer = null;
                EventManager.StartListening(PlayEvents.Rested, Spare);
            }
        }

        // TODO: move to when next rest occurs rationale making a choice end of combat all the time is disruptive
        void Spare(object input = null) => attributes.friendliness += 2.0f;
        void OnDisable() => EventManager.StopListening(PlayEvents.Rested, Spare);

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
                    var dropPos = new Vector3(UnityEngine.Random.Range(dropRange.Head.position.x, dropRange.Tail.position.x), dropRange.Head.position.y, 0);
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
