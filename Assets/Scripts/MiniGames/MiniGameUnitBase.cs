using UnityEngine;
using System;


namespace Chronellium.MiniGames
{
    public abstract class MiniGameUnitBase : MonoBehaviour
    {
        // Fields
        public string title;
        [TextArea(3, 7)] public string description;
        [TextArea(3, 7)] public string hint;
        public MiniGameMode gameMode;
        public MiniGameDifficulty difficulty;

        // Events for miniGame solved correctly or incorrectly
        public event Action OnMiniGameSolvedSuccessfully;
        public event Action OnMiniGameSolvedWrongly;

        // Abstract methods
        public abstract void StartMiniGame();
        public abstract void PauseMiniGame();
        public abstract void ResumeMiniGame();

        // Method for triggering the event when the miniGame is solved successfully
        protected void MiniGameSolvedSuccessfully()
        {
            OnMiniGameSolvedSuccessfully?.Invoke();
        }

        // Method for triggering the event when the miniGame is solved wrongly
        protected void MiniGameSolvedWrongly()
        {
            OnMiniGameSolvedWrongly?.Invoke();
        }
    }

    [Serializable]
    public enum MiniGameMode
    {
        HACKING,
    }

    [Serializable]
    public enum MiniGameDifficulty
    {
        TUTORIAL,
        BEGINNER,
        INTERMEDIATE,
        HARD,
    }
}