using System.Linq;
using UnityEngine;

namespace Abyss.Interactables
{
    public abstract class CondInteractable : Interactable
    {
        [SerializeField][Tooltip("OR connectors")] EventCondChecker[] condCheckers;

        protected bool CheckIsMet()
        {
            if (condCheckers.Count() == 0) return true;
            foreach (EventCondChecker condChecker in condCheckers)
            {
                if (condChecker.IsMet())
                    return true;
            }
            return false;
        }

        protected override void OnTriggerEnter2D(Collider2D collider)
        {
            if (CheckIsMet()) base.OnTriggerEnter2D(collider);
        }

        protected override void OnTriggerExit2D(Collider2D collider)
        {
            if (CheckIsMet()) base.OnTriggerExit2D(collider);
        }
    }
}
