using Abyss.Environment.Enemy;
using UnityEngine;

namespace Abyss.Player.Spells
{
	public class EmberSpell : Spell
	{

		#region Fields

		public const string EMBER_SPELL_NAME = "Spell-Fire-Ember";

		private const string TAG_ENEMY = "Enemy";
		private const string TAG_BURNABLE = "Burnable";

		// Movement
		private Vector2 currLocation;
		[SerializeField] private float moveSpeedValue = 0.02f;
		private Vector2 moveVector;

		// Behaviour
		[SerializeField] private float damageAmount = 30f;
		[SerializeField] private float existForTime = 1f;

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
			if (other.CompareTag(TAG_ENEMY) && other.TryGetComponent<EnemyPart>(out var enemyPart))
				enemyPart.TakeHit(damageAmount);
			else if (other.gameObject.CompareTag(TAG_BURNABLE))
				Destroy(other.gameObject); // TODO: Add burn effect
		}

		public override void Cast(bool toLeft)
		{
			transform.position += toLeft ? Vector3.left : Vector3.right;
			Initialize(toLeft);
		}
	}
}
