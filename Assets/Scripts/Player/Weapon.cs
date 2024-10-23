using System.Collections.Generic;
using Abyss.EventSystem;
using Abyss.Environment.Enemy;
using UnityEngine;

namespace Abyss.Player
{
    public class Weapon : MonoBehaviour
    {
        static readonly int _layerMask = (1 << 6) | (1 << 12); // 6 for enemy, 12 for breakable
        public WeaponItem weaponItem;
        readonly HashSet<int> _enemyHits = new(), _depoHits = new();
        ParticleSystem _particleSystem;

        void Awake() => _particleSystem = GetComponent<ParticleSystem>();

        void OnEnable() => EventManager.StartListening(PlayEvents.WeaponEquipped, Equip);
        void OnDisable() => EventManager.StopListening(PlayEvents.WeaponEquipped, Equip);

        void Equip(object obj) => weaponItem = (WeaponItem)obj;

        public void Strike(float str) // Can be called multiple times in one single "attack", hence the need to track which has already been hit
        {
            if (weaponItem == null) return;
            // TODO: Change position based on weapon movement
            var hits = Physics2D.OverlapCircleAll(transform.position, weaponItem.radius, _layerMask);
            bool psPlayed = false;
            foreach (var hit in hits)
            {
                if (hit.gameObject.TryGetComponent<EnemyPart>(out var enemyPart))
                {
                    if (_enemyHits.Contains(enemyPart.EnemyIntanceId)) continue;
                    if (!psPlayed)
                    {
                        _particleSystem.Play();
                        psPlayed = true;
                    }
                    _enemyHits.Add(enemyPart.EnemyIntanceId);
                    enemyPart.TakeHit(weaponItem.damage + str);
                }
                else if (hit.gameObject.TryGetComponent<Breakable>(out var breakable))
                    breakable.TakeHit(weaponItem.damage + str);
                else if (hit.gameObject.GetComponentInParent<MaterialDeposit>() is MaterialDeposit materialDeposit && !_depoHits.Contains(materialDeposit.DepoId))
                {
                    materialDeposit.TakeHit();
                    _depoHits.Add(materialDeposit.DepoId);
                }
            }
        }

        public void Reset()
        {
            _enemyHits.Clear();
            _depoHits.Clear();
        }

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
