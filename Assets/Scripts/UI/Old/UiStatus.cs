using System;
using UnityEngine;
using Abyss.SceneSystem;

/// <summary>
/// Represents the state and events related to the UI.
/// </summary>
public class UiStatus : MonoBehaviour
{
    /// <summary>
    /// Indicates if the UI is currently open.
    /// </summary>
    public static bool IsOpen { get; private set; }

    /// <summary>
    /// Event triggered when the UI is opened.
    /// </summary>
    public static event Action OnOpenUI;

    /// <summary>
    /// Event triggered when the UI is closed.
    /// </summary>
    public static event Action OnCloseUI;

    private void Awake()
    {
        // Reset the static variable IsOpen to false
        // otherwise, it might not reset automatically when 
        // the scene restarts
        IsOpen = false;
    }

    /// <summary>
    /// Opens the UI and triggers the OnOpenUI event.
    /// </summary>
    public static void OpenUI()
    {
        IsOpen = true;
        OnOpenUI?.Invoke();
    }

    /// <summary>
    /// Closes the UI and triggers the OnCloseUI event.
    /// </summary>
    public static void CloseUI()
    {
        IsOpen = false;
        OnCloseUI?.Invoke();
    }

    /// <summary>
    /// Checks if the UI is disabled due to a scene transition.
    /// </summary>
    /// <returns>True if the UI is disabled, false otherwise.</returns>
    public static bool IsDisabled()
    {
        return SceneLoader.Instance.InTransit;
    }
}