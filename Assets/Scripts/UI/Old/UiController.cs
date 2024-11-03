using System;
using Abyss.SceneSystem;
using Tuples;
using UnityEngine;

[Serializable]
public class UiController
{
    public enum Type { Dialogue, Inventory, Choice, Trade, Craft, Learn, None }
    public bool IsOpen { get; private set; } = false;
    [Tooltip("UI types not explicitly assigned has 0 lowest priority and pauses game, last bool for whether the UI type should pause game")] public Triplet<Type, uint, bool>[] PanelPriorities;
    Type _currType = Type.None;
    uint _currPriority = 0;
    Action _currInterruptHandler;

    bool IsDisabled => SceneLoader.Instance.InTransit;

    public bool Open(Type type, Action interruptHandler = null)
    {
        if (IsDisabled || Compare(type) < 0) return false;
        Debug.Log($"Opening {type} panel");
        _currInterruptHandler?.Invoke();

        if (ShouldPause(type))
            GameManager.Instance.PauseGame();
        else GameManager.Instance.ResumeGame();
        _currType = type;
        _currInterruptHandler = interruptHandler;
        _currPriority = GetPriority(type);
        IsOpen = true;
        return true;
    }

    public void Close()
    {
        _currType = Type.None;
        _currPriority = 0;
        _currInterruptHandler = null;
        IsOpen = false;
        GameManager.Instance.ResumeGame();
    }

    int Compare(Type incomingType) => (int)GetPriority(incomingType) - (int)_currPriority; // >= 0 means the incoming type has higher priority

    uint GetPriority(Type type)
    {
        uint priority = 0;
        for (int i = 0; i < PanelPriorities.Length; i++)
            if (PanelPriorities[i].Item1 == type)
                priority = PanelPriorities[i].Item2;
        return priority;
    }

    bool ShouldPause(Type type)
    {
        bool pause = true;
        for (int i = 0; i < PanelPriorities.Length; i++)
            if (PanelPriorities[i].Item1 == type)
                pause = PanelPriorities[i].Item3;
        return pause;
    }
}