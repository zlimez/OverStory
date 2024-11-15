using System.Linq;
using UnityEngine;

namespace Abyss.Interactables
{
    public abstract class CondInteractable : Interactable
    {
        [SerializeField] EventCondChecker[] condCheckers;
        protected bool _isMet = false;

        protected bool CheckIsMet()
        {
            if (condCheckers.Count() == 0) 
            {
                _isMet = true;
                return true;
            }
            foreach (EventCondChecker condChecker in condCheckers)
            {
                if (condChecker.IsMet()) 
                {
                    _isMet = true;
                    return true;
                }
            }
            return false;
        } 

        protected override void OnTriggerEnter2D(Collider2D collider)
        {
            CheckIsMet();
            if (_isMet) base.OnTriggerEnter2D(collider);
        }

        protected override void OnTriggerExit2D(Collider2D collider)
        {
            if (_isMet)
            {
                base.OnTriggerExit2D(collider);
                _isMet = false;
            }
        }
    }
}
