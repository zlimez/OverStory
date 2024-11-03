using Abyss.EventSystem;
using UnityEngine;
using System.Collections.Generic;

namespace Abyss.Interactables
{
    // TODO: Add persistence or refresh items logic
    public class LearningPost : Interactable
    {
        [SerializeField] Tribe tribe;
        [SerializeField][Tooltip("Initial special items available")] List<BlueprintItem> initialItems;

        private readonly Collection itemCollection = new(null, 999);

        void Start()
        {
            foreach (var item in initialItems)
                itemCollection.Add(item);
        }

        public override void Interact() => EventManager.InvokeEvent(PlayEvents.LearningPostEntered, (tribe, itemCollection));
    }
}
