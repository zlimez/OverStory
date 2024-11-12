using UnityEngine;
using Abyss.Utils;
using Abyss.EventSystem;
using Abyss.DataPersistence;
using System.Collections.Generic;

/// <summary>
/// Manages game-related data and states that persist throughout the session.
/// </summary>
public class GameManager : Singleton<GameManager>
{
    public UiController UI;
    // public GameObject InteractableHint;

    public Inventory Inventory = new();
    public PlayerPersistence PlayerPersistence = new();
    public TimePersistence TimePersistence = new();
    public Dictionary<string, float> RestsitesPersistence = new();
    public Dictionary<string, object> EnvStatePersistence = new();
#if UNITY_EDITOR
    public bool DebugEnableInventory = false;
#endif

    protected override void Awake()
    {
        base.Awake();
        Inventory.MaterialCollection.isPlayer = true;
        EventManager.InvokeEvent(SystemEvents.SystemsReady);
#if UNITY_EDITOR
        Inventory.Enabled = DebugEnableInventory;
#endif
    }

    public void PauseGame() => Time.timeScale = 0;
    public void ResumeGame() => Time.timeScale = 1;
}