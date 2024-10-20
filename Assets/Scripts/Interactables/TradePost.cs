using Abyss.EventSystem;
using Abyss.Player;
using UnityEngine;
using Tuples;
using System.Collections.Generic;

// TODO: Add persistence or refresh items logic
public class TradePost : Interactable
{
    [SerializeField] Tribe tribe;
    [SerializeField][Tooltip("Initial items and their count available")] List<RefPair<Item, int>> initialItems;

    private readonly Collection itemCollection = new(null, 999);

    void Start()
    {
        foreach (var itemStack in initialItems)
            itemCollection.Add(itemStack.Head, itemStack.Tail);
    }

    public override void Interact() => EventManager.InvokeEvent(PlayEvents.TradePostEntered, (tribe, player.GetComponent<PlayerManager>().PlayerAttr, itemCollection));
}
