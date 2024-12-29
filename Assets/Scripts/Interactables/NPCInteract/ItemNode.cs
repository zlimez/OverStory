using Tuples;
using UnityEngine;

namespace NPC
{
    [CreateAssetMenu(menuName = "NPCInteract/Item")]
    public class ItemNode : ActionNode
    {
        public Pair<Item, int>[] itemsAdded, itemsRemoved;

        public override void Execute()
        {
            foreach (var item in itemsAdded)
                GameManager.Instance.Inventory.MaterialCollection.Add(item.Head, item.Tail);

            foreach (var item in itemsRemoved)
                GameManager.Instance.Inventory.MaterialCollection.RemoveStock(item.Head, item.Tail);
        }
    }
}
