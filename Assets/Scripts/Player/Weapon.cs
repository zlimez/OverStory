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
		[SerializeField] private int breakableDamage = 1;

		void Awake() => _particleSystem = GetComponent<ParticleSystem>();

		void OnEnable() => EventManager.StartListening(new GameEvent(WeaponItem.WeaponEquippedPrefix), Equip);
		void OnDisable() => EventManager.StopListening(new GameEvent(WeaponItem.WeaponEquippedPrefix), Equip);

		void Equip(object obj)
		{
			if (weaponItem != null) GameManager.Instance.Inventory.AddTo(weaponItem);
			weaponItem = (WeaponItem)obj;
		}

		public void Strike(float str)
		{
			if (weaponItem == null) return;
			// TODO: Change position based on weapon movement
			var hitEnemies = Physics2D.OverlapCircleAll(transform.position, weaponItem.radius, _layerMask);
			bool psPlayed = false;
			foreach (var hitEnemy in hitEnemies)
			{
				Debug.Log(hitEnemy.name);
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
				else if (hitEnemy.gameObject.CompareTag("Breakable")) 
				{
					hitEnemy.gameObject.TryGetComponent<Cluster>(out var clusterComponent);
					clusterComponent.TakeHit(breakableDamage);
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
