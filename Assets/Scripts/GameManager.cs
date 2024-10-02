using Abyss.EventSystem;
using UnityEngine;
using Abyss.Utils;
using Abyss.SceneSystem;

/// <summary>
/// Manages game-related data and states that persist throughout the session.
/// </summary>
public class GameManager : Singleton<GameManager>
{
    /// <summary>
    /// The last scene visited.
    /// </summary>
    public AbyssScene LastScene;

    /// <summary>
    /// The last position of the player. Tied to SpawnController.
    /// </summary>
    public Vector3? LastPosition;

    /// <summary>
    /// Indicates whether the player's last position should be flipped along the X-axis. Tied to SpawnController.
    /// </summary>
    public bool LastPositionFlipX;

    /// <summary>
    /// Indicates whether the player should revert to the last position. Tied to SpawnController.
    /// </summary>
    public bool RevertToLastPosition;

    /// <summary>
    /// The conversation for the current cutscene.
    /// </summary>
    public Conversation CutsceneConversation;

    /// <summary>
    /// The conversation for the current cutscene.
    /// </summary>
    public SecondaryConversation CutsceneSecondaryConversation;

    [SerializeField]
    private GameObject interactableHint;

    protected override void Awake()
    {
        if (Instance == null) InitInventory();

        base.Awake();
        EventManager.InvokeEvent(SystemEventCollection.GameManagerReady);
    }

    /// <summary>
    /// Returns the interactable hint object.
    /// </summary>
    /// <returns>GameObject representing the interactable hint.</returns>
    public GameObject GetInteractableHint()
    {
        return interactableHint;
    }

    /// <summary>
    /// Initializes the inventory.
    /// </summary>
    private void InitInventory()
    {
        Inventory gameInventory = new Inventory();
        Inventory.AssignInventory(gameInventory);
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    /// <summary>
    /// Resumes the game.
    /// </summary>
    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
}