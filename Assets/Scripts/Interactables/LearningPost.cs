using Abyss.EventSystem;
using Abyss.Player;
using UnityEngine;
using Tuples;
using System.Collections.Generic;

namespace Abyss.Interactables
{
    // TODO: Add persistence or refresh items logic
    public class LearningPost : Interactable
    {
        [SerializeField] Tribe tribe;
        [SerializeField][Tooltip("Initial special items available")] List<Item> initialItems;

        private readonly Collection itemCollection = new(null, 999);

        void Start()
        {
            foreach (var item in initialItems)
                itemCollection.Add(item);
        }

        public override void Interact() => EventManager.InvokeEvent(PlayEvents.LearningPostEntered, (tribe, itemCollection));
    }
}
