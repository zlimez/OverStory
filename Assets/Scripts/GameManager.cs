using Abyss.EventSystem;
using UnityEngine;
using Abyss.Utils;
using Abyss.SceneSystem;
using Abyss.Environment.Enemy;

/// <summary>
/// Manages game-related data and states that persist throughout the session.
/// </summary>
public class GameManager : Singleton<GameManager>
{
    public Conversation CutsceneConversation;
    public SecondaryConversation CutsceneSecondaryConversation;
    public GameObject InteractableHint;

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
}