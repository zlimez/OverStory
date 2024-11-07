using System.Collections;
using System.Collections.Generic;
using Abyss.EventSystem;
using Abyss.Interactables;
using UnityEngine;

public class BackpackT2Pickup : BackpackPickup
{
	public override void Interact() 
	{
		GameManager.Instance.Inventory.Enabled = true;
		GameManager.Instance.Inventory.Level = 2;
		EventLedger.Instance.Record(new GameEvent(backpackPickedEvent.EventName));
		Destroy(gameObject);
	}
}
