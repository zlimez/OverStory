using System.Collections;
using System.Collections.Generic;
using Abyss.EventSystem;
using Tuples;
using UnityEngine;

public class AttackTutorial : MonoBehaviour
{
	
	// Statements
	[SerializeField] private RefPair<string, float> inventorySpeech = new("Press 'i' to open Inventory and equip the Axe", 8f);
	[SerializeField] private RefPair<string, float> attackSpeech = new("Press 'left mouse button' to Attack", 4f);
	
	[SerializeField] Item axeItem;
	private bool hasSaidInstructions = false;
	
	private void SayInstructions() 
	{
		EventManager.InvokeEvent(PlayEvents.PlayerSpeak, inventorySpeech);
		EventManager.InvokeEvent(PlayEvents.PlayerSpeak, attackSpeech);
		hasSaidInstructions = true;
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		if (hasSaidInstructions) return;
		if (!GameManager.Instance.Inventory.MaterialCollection.Contains(axeItem)) return;
		if (other.gameObject.CompareTag("Player")) 
		{
			SayInstructions();
		}
	}
}
