using Abyss.EventSystem;
using Abyss.Interactables;
using UnityEngine;

public class FaraMask : Interactable
{
    [SerializeField] GameObject withMask;
    [SerializeField] GameObject withoutMask;
    [SerializeField] DynamicEvent maskTakenEvent;
    [SerializeField] Item mask;

    bool _maskTaken = false;

    void OnEnable()
    {
        if (EventLedger.Instance == null)
            EventManager.StartListening(SystemEvents.LedgerReady, Load);
        else Load();
    }

    void Load(object input = null)
    {
        if (EventLedger.Instance.HasOccurred(new GameEvent(maskTakenEvent.EventName)))
        {
            withMask.SetActive(false);
            withoutMask.SetActive(true);
            _maskTaken = true;
        }
        EventManager.StopListening(SystemEvents.LedgerReady, Load);
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if (!_maskTaken)
            base.OnTriggerEnter2D(collider);
    }

    public override void Interact()
    {
        withMask.SetActive(false);
        withoutMask.SetActive(true);
        EventLedger.Instance.Record(new GameEvent(maskTakenEvent.EventName));
        GameManager.Instance.Inventory.MaterialCollection.Add(mask);
        base.Interact();
    }

    void OnDisable() => EventManager.StopListening(SystemEvents.LedgerReady, Load);
}
