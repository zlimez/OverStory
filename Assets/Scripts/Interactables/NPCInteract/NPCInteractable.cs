using Abyss.EventSystem;
using Abyss.Interactables;
using UnityEngine;

namespace NPC
{
    public class NPCInteractable : CondInteractable
    {
        public static readonly string InteractRec = "InteractRec";
        [SerializeField] ActionNode interactRoot;

        public override void Interact()
        {
            GameEvent interactEvent = new(InteractRec + "/" + name);
            interactRoot.Execute(interactEvent);
            base.Interact();
            EventLedger.Instance.Record(interactEvent);
        }
    }
}
