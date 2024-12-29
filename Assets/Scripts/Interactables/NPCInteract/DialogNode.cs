using UnityEngine;

namespace NPC
{
    [CreateAssetMenu(menuName = "NPCInteract/Dialog")]
    public class DialogNode : ActionNode
    {
        public Conversation convo;

        public override void Execute()
        {
            if (Next == null)
                DialogueManager.Instance.HardStartConvo(convo, null, true);
            else DialogueManager.Instance.HardStartConvo(convo, Next.Execute, true);
        }
    }
}
