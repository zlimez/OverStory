using System.Collections.Generic;
using Abyss.EventSystem;
using Tuples;
using UnityEngine;

namespace NPC
{

    [CreateAssetMenu(menuName = "NPCInteract/TradeUI")]
    public class TradeUINode : ActionNode
    {
        [SerializeField] Tribe tribe;
        [SerializeField][Tooltip("Initial items and their count available")] List<RefPair<Item, int>> initialItems;
        GameEvent _interactEvent;

        public override void Execute(GameEvent interactEvent)
        {
            Collection itemCollection = new(null, 999);
            foreach (var itemStack in initialItems)
                itemCollection.Add(itemStack.Head, itemStack.Tail);
            EventManager.InvokeEvent(PlayEvents.TradePostEntered, (tribe, itemCollection));
            _interactEvent = interactEvent;
            EventManager.StartListening(PlayEvents.TradePostExited, NextExec);
        }

        void NextExec(object input)
        {
            if (Next != null) Next.Execute(_interactEvent);
            EventManager.StopListening(PlayEvents.TradePostExited, NextExec);
        }
    }
}
