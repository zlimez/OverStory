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
        [SerializeField] WeaponItem _weaponItem;
        public HashSet<int> hits = new();

        void OnEnable()
        {
            EventManager.StartListening(new GameEvent(WeaponItem.WeaponEquippedPrefix), Equip);
        }

        void OnDisable()
        {
            EventManager.StopListening(new GameEvent(WeaponItem.WeaponEquippedPrefix), Equip);
        }

        void Equip(object obj)
        {
            _weaponItem = (WeaponItem)obj;
        }

        public void Strike(float str)
        {
            // TODO: Change position based on weapon movement
            var hitEnemies = Physics2D.OverlapCircleAll(transform.position, _weaponItem.radius, _layerMask);
            foreach (var hitEnemy in hitEnemies)
            {
                if (hitEnemy.gameObject.TryGetComponent<EnemyPart>(out var enemyPart))
                {
                    if (hits.Contains(enemyPart.EnemyIntanceId)) continue;
                    hits.Add(enemyPart.EnemyIntanceId);
                    enemyPart.TakeHit(_weaponItem.damage + str);
                }
            }
        }

        public void Reset()
        {
            hits.Clear();
        }

#if DEBUG
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _weaponItem.radius);
        }
#endif
    }
}
