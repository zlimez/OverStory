using Abyss.EventSystem;
using UnityEngine;

namespace NPC
{
    [CreateAssetMenu(menuName = "NPCInteract/Dialog")]
    public class DialogNode : ActionNode
    {
        public Conversation convo;

        public override void Execute(GameEvent interactEvent)
        {
            if (Next == null)
                DialogueManager.Instance.SoftStartConvo(convo);
            else DialogueManager.Instance.SoftStartConvo(convo, () => Next.Execute(interactEvent));
        }
    }
}
