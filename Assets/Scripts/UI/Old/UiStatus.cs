using System;
using Abyss.SceneSystem;

public class UiStatus
{
    public bool IsOpen { get; private set; } = false;
    public event Action OnOpenUI;
    public event Action OnCloseUI;

    public void OpenUI()
    {
        IsOpen = true;
        OnOpenUI?.Invoke();
    }

    public void CloseUI()
    {
        IsOpen = false;
        OnCloseUI?.Invoke();
    }

    public static bool IsDisabled => SceneLoader.Instance.InTransit;
}