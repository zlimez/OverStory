using System.Collections.Generic;
using Abyss.Environment.Enemy;
using UnityEngine;

namespace Abyss.Player.Spells
{
	public class EmberSpell : Spell
	{

		#region Fields
		const string TAG_BURNABLE = "Burnable";

		// Movement
		private Vector2 currLocation;
		[SerializeField] private float moveSpeedValue = 0.02f;
		private Vector2 moveVector;

		// Behaviour
		[SerializeField] private float damageAmount = 30f;
		[SerializeField] private float existForTime = 1f;

		HashSet<int> _enemyHits = new();

		#endregion

		// Start is called before the first frame update
		void Start() => Destroy(gameObject, existForTime);

		public void Initialize(bool shouldFaceLeft) => moveVector = moveSpeedValue * (shouldFaceLeft ? Vector2.left : Vector2.right);

		void Update()
		{
			currLocation = transform.position;
			currLocation += moveVector;
			transform.position = currLocation;
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Enemy") && other.TryGetComponent<EnemyPart>(out var enemyPart) && !_enemyHits.Contains(enemyPart.EnemyIntanceId))
			{
				enemyPart.TakeHit(damageAmount);
				_enemyHits.Add(enemyPart.EnemyIntanceId);
			}
			else if (other.gameObject.CompareTag(TAG_BURNABLE))
				Destroy(other.gameObject); // TODO: Add burn effect
		}

		public override void Cast(bool toLeft)
		{
			base.Cast(toLeft);
			transform.position += toLeft ? Vector3.left : Vector3.right;
			Initialize(toLeft);
		}
	}
}
