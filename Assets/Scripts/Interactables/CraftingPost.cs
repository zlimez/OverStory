using Abyss.EventSystem;
using Abyss.Player;
using UnityEngine;
using Tuples;
using System.Collections.Generic;

namespace Abyss.Interactables
{
    // TODO: Add persistence or refresh items logic
    public class CraftingPost : Interactable
    {
        void Start()
        {
        }

        public override void Interact() => EventManager.InvokeEvent(PlayEvents.CraftingPostEntered);
    }
}
