using Abyss.EventSystem;
using Utils.Tuples;
using UnityEngine;

namespace NPC
{
    [CreateAssetMenu(menuName = "NPCInteract/Item")]
    public class ItemNode : ActionNode
    {
        public static readonly string ItemExchangedRec = "ItemExchangedRec";
        public Pair<Item, int>[] itemsAdded, itemsRemoved;

        public override void Execute(GameEvent interactEvent)
        {
            GameEvent fullEvent = new(interactEvent + "/" + ItemExchangedRec + "/" + name);
            if (EventLedger.Instance.HasOccurred(fullEvent)) return;
            EventLedger.Instance.Record(fullEvent);
            foreach (var item in itemsAdded)
                GameManager.Instance.Inventory.MaterialCollection.Add(item.Head, item.Tail);

            foreach (var item in itemsRemoved)
                GameManager.Instance.Inventory.MaterialCollection.RemoveStock(item.Head, item.Tail);
        }
    }
}
