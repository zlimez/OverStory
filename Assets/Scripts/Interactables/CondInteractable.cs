using UnityEngine;

namespace Abyss.Interactables
{
    public abstract class CondInteractable : Interactable
    {
        [SerializeField] EventCondChecker condChecker;
        protected bool _isMet = false;

        protected override void OnTriggerEnter2D(Collider2D collider)
        {
            if (condChecker.IsMet())
            {
                _isMet = true;
                base.OnTriggerEnter2D(collider);
            }
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
