using System.Collections.Generic;
using Abyss.EventSystem;
using UnityEngine;

namespace NPC
{

    [CreateAssetMenu(menuName = "NPCInteract/LearnUI")]
    public class LearnUINode : ActionNode
    {
        [SerializeField] Tribe tribe;
        [SerializeField][Tooltip("Initial special items available")] List<BlueprintItem> initialItems;
        GameEvent _interactEvent;

        public override void Execute(GameEvent interactEvent)
        {
            Collection itemCollection = new(null, 999);
            foreach (var item in initialItems)
                itemCollection.Add(item);
            EventManager.InvokeEvent(PlayEvents.LearningPostEntered, (tribe, itemCollection));
            _interactEvent = interactEvent;
            EventManager.StartListening(PlayEvents.LearningPostExited, NextExec);
        }

        void NextExec(object input)
        {
            if (Next != null) Next.Execute(_interactEvent);
            EventManager.StopListening(PlayEvents.TradePostExited, NextExec);
        }
    }
}
