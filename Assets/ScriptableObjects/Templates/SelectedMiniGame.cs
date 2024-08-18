using UnityEngine;
using System.Collections.Generic;
using Chronellium.MiniGames;

[CreateAssetMenu(fileName = "SelectedMiniGame", menuName = "MiniGames/SelectedMiniGame", order = 0)]
public class SelectedMiniGame : ScriptableObject
{
    public MiniGameUnitBase selectedMiniGamePrefab;
    public List<MiniGameUnitBase> miniGames;

    private int currentIndex = 0;

    public delegate void SelectedMiniGameChangedDelegate(MiniGameUnitBase newSelectedMiniGame);
    public event SelectedMiniGameChangedDelegate OnSelectedMiniGameChanged;

    public void UpdateSelectedMiniGame(MiniGameUnitBase newSelectedMiniGame)
    {
        selectedMiniGamePrefab = newSelectedMiniGame;
        currentIndex = miniGames.IndexOf(newSelectedMiniGame);
        OnSelectedMiniGameChanged?.Invoke(newSelectedMiniGame);
    }

    public void SelectNextMiniGame()
    {
        currentIndex = (currentIndex + 1) % miniGames.Count;
        UpdateSelectedMiniGame(miniGames[currentIndex]);
    }

    public void StartSelectedMiniGame()
    {
        // Debug.Log(selectedMiniGamePrefab);
        selectedMiniGamePrefab?.StartMiniGame();
    }

    public void ResumeSelectedMiniGame()
    {
        // Debug.Log(selectedMiniGamePrefab);
        selectedMiniGamePrefab?.ResumeMiniGame();
    }

    public void PauseSelectedMiniGame()
    {
        // Debug.Log(selectedMiniGamePrefab);
        selectedMiniGamePrefab?.PauseMiniGame();
    }
}
