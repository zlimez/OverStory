using System;
using Abyss.EventSystem;
using Abyss.Interactables;
using UnityEngine;

public class PowerPanel : Interactable
{
    public static readonly Config DEFAULT_CONFIG = Config.One;
    public static readonly string CONFIG_KEY = "PowerPanelConfig";

    [SerializeField] DynamicEvent millFixedEvent;
    [SerializeField] GameObject inactive, illum;
    [SerializeField] GameObject[] configs;

    int _activeConfig = -1;
    bool _hasPower = false;

    [Serializable]
    public enum Config { One = 0, Two = 1, Three = 2 }

    void OnEnable()
    {
        if (GameManager.Instance == null)
            EventManager.StartListening(SystemEvents.SystemsReady, Load);
        else Load();
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if (_hasPower) base.OnTriggerEnter2D(collider);
    }

    public override void Interact()
    {
        EventManager.InvokeEvent(PlayEvents.InteractableExited);
        Activate((_activeConfig + 1) % configs.Length);
    }

    void Load(object input = null)
    {
        if (!GameManager.Instance.EnvStatePersistence.ContainsKey("PowerPanelConfig")) return;

        Config config = (Config)GameManager.Instance.EnvStatePersistence["PowerPanelConfig"];
        illum.SetActive(true);
        _activeConfig = (int)config;
        configs[_activeConfig].SetActive(true);
        inactive.SetActive(false);
        _hasPower = true;
        EventManager.StopListening(SystemEvents.SystemsReady, Load);
    }

    void Activate(int config)
    {
        if (_activeConfig == config) return;

        configs[_activeConfig].SetActive(false);
        configs[config].SetActive(true);
        _activeConfig = config;
        GameManager.Instance.EnvStatePersistence["PowerPanelConfig"] = (Config)_activeConfig;
    }

    void OnDisable() => EventManager.StopListening(SystemEvents.SystemsReady, Load);
}