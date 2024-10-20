using System.Collections.Generic;
using Abyss.EventSystem;
using Abyss.Environment.Enemy;
using UnityEngine;

namespace Abyss.Player
{
    public class Weapon : MonoBehaviour
    {
        // TODO: Add removable obstacle to mask, currently only inc. enemy
        static readonly int _layerMask = 1 << 6;
        public WeaponItem weaponItem;
        readonly HashSet<int> _hits = new();
        ParticleSystem _particleSystem;

        void Awake() => _particleSystem = GetComponent<ParticleSystem>();

        void OnEnable() => EventManager.StartListening(PlayEvents.WeaponEquipped, Equip);
        void OnDisable() => EventManager.StopListening(PlayEvents.WeaponEquipped, Equip);

        void Equip(object obj) => weaponItem = (WeaponItem)obj;

        public void Strike(float str)
        {
            if (weaponItem == null) return;
            // TODO: Change position based on weapon movement
            var hitEnemies = Physics2D.OverlapCircleAll(transform.position, weaponItem.radius, _layerMask);
            bool psPlayed = false;
            foreach (var hitEnemy in hitEnemies)
            {
                if (hitEnemy.gameObject.TryGetComponent<EnemyPart>(out var enemyPart))
                {
                    if (_hits.Contains(enemyPart.EnemyIntanceId)) continue;
                    if (!psPlayed)
                    {
                        _particleSystem.Play();
                        psPlayed = true;
                    }
                    _hits.Add(enemyPart.EnemyIntanceId);
                    enemyPart.TakeHit(weaponItem.damage + str);
                }
            }
        }

        public void Reset() => _hits.Clear();

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (weaponItem != null)
                Gizmos.DrawWireSphere(transform.position, weaponItem.radius);
        }
#endif
    }
}
