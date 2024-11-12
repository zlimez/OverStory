using System;
using Tuples;
using UnityEngine;

public class MaterialDeposit : MonoBehaviour
{

	[SerializeField] int stock;
	[SerializeField] GameObject materialDropPrefab;
	[SerializeField] Pair<GameObject, GameObject> beforeAfter;
	[SerializeField] SpriteFlash spriteFlash;
	public int DepoId { get; private set; }

	public static int DepoIdCnter = 0;

	void Awake() => DepoId = DepoIdCnter++;

	public void TakeHit(int count = 1)
	{
		if (stock == 0) return;

		spriteFlash.StartFlash();
		int pop = Math.Min(stock, count);
		// Spawn items
		Vector3 newItemPos;
		for (int i = 0; i < pop; i++)
		{
			newItemPos.x = transform.position.x + UnityEngine.Random.Range(-1.5f, 1.5f);
			newItemPos.y = transform.position.y + UnityEngine.Random.Range(0.1f, 0.4f);
			newItemPos.z = 0;
			Instantiate(materialDropPrefab, newItemPos, Quaternion.identity);
		}
		stock -= pop;
		if (stock == 0)
		{
			beforeAfter.Head.SetActive(false);
			beforeAfter.Tail.SetActive(true);
		}
	}
}
