using Abyss.EventSystem;
using Abyss.Player;
using UnityEngine;
using Tuples;
using System.Collections.Generic;

// TODO: Add persistence or refresh items logic
public class TradePost : MonoBehaviour
{
    [SerializeField] Tribe tribe;
    [SerializeField][Tooltip("Initial items and their count available")] List<RefPair<Item, int>> initialItems;

    private Collection itemCollection = new(null, 999);

    void Start()
    {
        foreach (var itemStack in initialItems)
            itemCollection.Add(itemStack.Head, itemStack.Tail);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision with: " + other.gameObject.name);
        if (other.CompareTag("Player"))
            EventManager.InvokeEvent(PlayEvents.TradePostEntered, (tribe, other.GetComponent<PlayerManager>().PlayerAttr, itemCollection));
    }
}
