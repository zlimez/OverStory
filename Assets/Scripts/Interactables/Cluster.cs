using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// General script for all clusters. Inherit this & attach the appropriate prefab.
/// </summary>
public class Cluster : MonoBehaviour
{
	
	private int numItemsRemaining;
	[SerializeField] private GameObject itemPrefab;
	
	// Start is called before the first frame update
	void Start()
	{
		numItemsRemaining = UnityEngine.Random.Range(3, 8); // [3, 8)
	}

	public void TakeHit(int count = 1)
	{
		int newNum = Math.Max(0, numItemsRemaining - count);
		// Spawn items
		Vector3 newItemPos;
		for (int i = 0; i < numItemsRemaining - newNum; i++) 
		{
			newItemPos.x = transform.position.x + UnityEngine.Random.Range(-1.5f, 1.5f);
			newItemPos.y = transform.position.y + UnityEngine.Random.Range(0.1f, 0.4f);
			newItemPos.z = 0;
			Instantiate(itemPrefab, newItemPos, Quaternion.identity);
		}
		numItemsRemaining = newNum;
		if (newNum == 0) 
		{
			// Destroy self
			Destroy(gameObject);
		}
	}
}
