using Abyss.EventSystem;
using NPC;
using UnityEngine;

public class TentLoot : NPCInteractable
{
    [SerializeField] GameObject before, after;

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!EventLedger.Instance.HasOccurred(new GameEvent(InteractRec + "/" + name)))
            base.OnTriggerEnter2D(other);
    }

    public override void Interact()
    {
        before.SetActive(false);
        after.SetActive(true);
        base.Interact();
    }
}
