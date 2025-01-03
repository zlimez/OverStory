using System;
using Abyss.SceneSystem;
using Tuples;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MaterialDeposit : MonoBehaviour
{

	[SerializeField] int stock;
	[SerializeField] GameObject materialDropPrefab;
	[SerializeField] Pair<GameObject, GameObject> beforeAfter;
	[SerializeField] SpriteFlash spriteFlash;
	[SerializeField] Transform dropBL, dropTR;
	public int TempDepoId { get; private set; }
	public static int DepoIdCnter = 0;

	void Awake()
	{
		if (spriteFlash == null) spriteFlash = GetComponent<SpriteFlash>();
		TempDepoId = DepoIdCnter++;
	}

	void Start()
	{
		if (GameManager.Instance.EnvStatePersistence.ContainsKey(SceneLoader.Instance.ActiveScene + "/Depo/" + name))
		{
			stock = (int)GameManager.Instance.EnvStatePersistence[SceneLoader.Instance.ActiveScene + "/Depo/" + name];
			if (stock == 0)
			{
				beforeAfter.Head.SetActive(false);
				beforeAfter.Tail.SetActive(true);
			}
		}
	}

	public void TakeHit(int count = 1)
	{
		if (stock == 0) return;

		spriteFlash.StartFlash();
		int pop = Math.Min(stock, count);
		// Spawn items
		Vector3 newItemPos;
		for (int i = 0; i < pop; i++)
		{
			newItemPos.x = UnityEngine.Random.Range(dropBL.position.x, dropTR.position.x);
			newItemPos.y = UnityEngine.Random.Range(dropBL.position.y, dropTR.position.y);
			newItemPos.z = 0;
			Instantiate(materialDropPrefab, newItemPos, Quaternion.identity);
		}
		stock -= pop;
		GameManager.Instance.EnvStatePersistence[SceneLoader.Instance.ActiveScene + "/Depo/" + name] = stock;
		if (stock == 0)
		{
			beforeAfter.Head.SetActive(false);
			beforeAfter.Tail.SetActive(true);
		}
	}
}
