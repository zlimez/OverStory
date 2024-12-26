using Abyss.EventSystem;
using UnityEngine;
using System.Collections.Generic;
using Tuples;

namespace Abyss.Interactables
{
	public class TutorialTrigger : MonoBehaviour
	{
		
		[SerializeField] public string tip;
		[SerializeField] public bool DestroyAfterUse;
	
		
		void OnTriggerEnter2D(Collider2D collider)
        {
            EventManager.InvokeEvent(UIEvents.TutorialDisplay, tip);
        }

		void OnTriggerExit2D() 
		{
			EventManager.InvokeEvent(UIEvents.TutorialClose, tip);
			if(DestroyAfterUse) gameObject.SetActive(false);
		}
	}
}


