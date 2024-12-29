using UnityEngine;

namespace NPC
{
    public class NPCInteractSystem : MonoBehaviour
    {
        public ActionNode interactRoot;

        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
                interactRoot.Execute();
        }
    }
}
