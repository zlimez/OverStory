using Abyss.EventSystem;
using Abyss.Interactables;
using UnityEngine;

public class Watermill : Interactable
{
    [SerializeField] GameObject wheel;
    [SerializeField] Item wheelReplacement;
    [SerializeField] GameObject broken, repaired;
    [SerializeField] AnimationCurve spinCurve;
    [SerializeField][Tooltip("In deg")] float maxSpinAngularVelocity = 80f;
    [SerializeField] float timeToReachMaxSpin = 5f;
    [SerializeField] DynamicEvent millFixedEvent;

    bool _isFixed = false;
    float _time;

    void OnEnable()
    {
        if (EventLedger.Instance == null)
            EventManager.StartListening(SystemEvents.LedgerReady, Load);
        else Load();
    }

    void Load(object input = null)
    {
        if (EventLedger.Instance.HasOccurred(new GameEvent(millFixedEvent.EventName)))
        {
            wheel.SetActive(true);
            broken.SetActive(false);
            repaired.SetActive(true);
            _isFixed = true;
            _time = timeToReachMaxSpin;
        }
        EventManager.StopListening(SystemEvents.LedgerReady, Load);
    }

    void OnDisable() => EventManager.StopListening(SystemEvents.LedgerReady, Load);

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if (!_isFixed && GameManager.Instance.Inventory.MaterialCollection.Contains(wheelReplacement))
            base.OnTriggerEnter2D(collider);
    }

    public override void Interact()
    {
        base.Interact();
        GameManager.Instance.Inventory.MaterialCollection.Remove(wheelReplacement);
        wheel.SetActive(true);
        broken.SetActive(false);
        repaired.SetActive(true);
        EventLedger.Instance.Record(new GameEvent(millFixedEvent.EventName));
        _isFixed = true;
        _time = 0;
    }

    void Update()
    {
        if (_isFixed)
        {
            if (_time < timeToReachMaxSpin)
            {
                var spinVel = spinCurve.Evaluate(_time / timeToReachMaxSpin) * maxSpinAngularVelocity;
                wheel.transform.Rotate(Vector3.forward, spinVel * Time.deltaTime);
                _time += Time.deltaTime;
            }
            else
                wheel.transform.Rotate(Vector3.forward, maxSpinAngularVelocity * Time.deltaTime);
        }
    }
}
