using System.Collections;
using System.Collections.Generic;
using Abyss.Environment.Enemy;
using UnityEngine;

public class EmberSpell : MonoBehaviour
{
	
	#region Fields
	
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
	void Start()
	{
		Destroy(gameObject, existForTime);
	}

	public void Initialize(bool shouldFaceLeft) 
	{
		moveVector = moveSpeedValue * (shouldFaceLeft ? Vector2.left : Vector2.right);
	}

	// Update is called once per frame
	void Update()
	{
		currLocation = transform.position;
		currLocation += moveVector;
		transform.position = currLocation;
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.CompareTag(TAG_ENEMY)) 
		{
			EnemyManager enemyManager = other.gameObject.GetComponent<EnemyManager>();
			if (enemyManager == null) 
			{
				return;
			}
			enemyManager.TakeHit(damageAmount);
			Debug.Log(other.name + ": health = " + enemyManager.health.ToString());
		}
		else if (other.gameObject.CompareTag(TAG_BURNABLE)) 
		{
			Destroy(other.gameObject);
		}
	}
}
