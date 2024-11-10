using System.Collections;
using System.Collections.Generic;
using Abyss.EventSystem;
using Abyss.Interactables;
using UnityEngine;

public class BackpackT3Pickup : BackpackPickup
{
	public override void Interact()
	{
		GameManager.Instance.Inventory.Enabled = true;
		GameManager.Instance.Inventory.Level = 3;
		EventLedger.Instance.Record(new GameEvent(backpackPickedEvent.EventName));
		base.Interact();
		Destroy(gameObject);
	}
}
