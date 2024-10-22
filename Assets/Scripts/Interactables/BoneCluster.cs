using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneCluster : Cluster
{
	private void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.CompareTag("Player")) 
		{
			TakeHit();
		}
	}
}
