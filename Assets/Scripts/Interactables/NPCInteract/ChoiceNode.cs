using System.Collections.Generic;
using Abyss.EventSystem;
using Tuples;
using UnityEngine;

namespace NPC
{

    [CreateAssetMenu(menuName = "NPCInteract/Choice")]
    public class ChoiceNode : ActionNode
    {
        public Pair<string, ActionNode>[] Branches;
        public List<Choice> Choices = new();

        public override void Execute(GameEvent interactEvent)
        {
            Choices.Clear();
            foreach (var branch in Branches)
            {
                Choice choice = new(branch.Head);
                choice.OnSelected += () =>
                {
                    if (branch.Tail != null) branch.Tail.Execute(interactEvent);
                };
                Choices.Add(choice);
            }

            ChoiceManager.Instance.StartChoice(Choices.ToArray(), true);
        }
    }
}
