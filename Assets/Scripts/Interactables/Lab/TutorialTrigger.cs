using Abyss.EventSystem;
using UnityEngine;

namespace Abyss.Interactables
{
	public class TutorialTrigger : MonoBehaviour
	{
		[SerializeField][Tooltip("Conditions to trigger this convo")] EventCondChecker condChecker;
		[SerializeField] string tip;
		[SerializeField] bool DestroyAfterUse;
		bool _active = false, _playerIn = false;


		void OnTriggerEnter2D(Collider2D collider)
		{
			if (collider.CompareTag("Player"))
			{
				_playerIn = true;
				if (condChecker.IsMet())
				{
					_active = true;
					EventManager.InvokeEvent(UIEvents.TutorialDisplay, tip);
				}
			}
		}

		// FIXME: Bad implementation?
		void Update()
		{
			if (!_active && _playerIn && condChecker.IsMet())
			{
				_active = true;
				EventManager.InvokeEvent(UIEvents.TutorialDisplay, tip);
			}
		}

		void OnTriggerExit2D(Collider2D collider)
		{
			if (collider.CompareTag("Player") && _active)
			{
				EventManager.InvokeEvent(UIEvents.TutorialClose, tip);
				if (DestroyAfterUse) gameObject.SetActive(false);
			}
		}
	}
}


