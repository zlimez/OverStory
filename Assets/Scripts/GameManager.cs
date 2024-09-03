using Abyss.EventSystem;
using UnityEngine;
using Abyss.Utils;
using Abyss.SceneSystem;
using System.Collections.Generic;

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
        if (Instance == null)
        {
            InitInventory();
        }

        base.Awake();

        EventManager.InvokeEvent(CoreEventCollection.GameManagerReady);

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
        Inventory.AssignNewInventory(gameInventory);
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



    [SerializeField]
    private GameObject npcPrefab;
    private List<GameObject> npcs = new List<GameObject>();
    private void Start()
    {
        List<NPCData> loadedNPCs1 = NPCDataManager.LoadNPCs();
        List<NPCData> loadedNPCs = NPCDataManager.NextGeneration(loadedNPCs1);

        foreach (NPCData data in loadedNPCs)
        {
            GameObject npcObject = Instantiate(npcPrefab, NPCDataManager.GenerateRandomPosition((float)-7.8, (float)13.5), Quaternion.identity);
            NPCController npcController = npcObject.GetComponent<NPCController>();

            if (npcController != null)
            {
                npcController.alive = true;
                npcController.speed = data.speed;
                npcController.strength = data.strength;
            }

            npcs.Add(npcObject);
        }
        NPCDataManager.SaveNPCs(npcs);
        // CreateRandomNPCs();
    }

    // private void CreateRandomNPCs()
    // {
    //     CreateRandomNPC(new Vector3(0, 0, 0));
    //     CreateRandomNPC(new Vector3(2, 0, 0));
    //     NPCDataManager.SaveNPCs(npcs);
    // }

    // private void CreateRandomNPC(Vector3 position)
    // {
    //     float randomSpeed = Random.Range(0f, 10f);
    //     float randomStrength = Random.Range(0f, 10f);

    //     GameObject npcObject = Instantiate(npcPrefab, position, Quaternion.identity);

    //     NPCController npcController = npcObject.GetComponent<NPCController>();
    //     if (npcController != null)
    //     {
    //         npcController.alive = true;
    //         npcController.speed = randomSpeed;
    //         npcController.strength = randomStrength;
    //     }
    //     npcs.Add(npcObject);

    //     Debug.Log($"Created npc with Speed: {randomSpeed}, Strength: {randomStrength}");
    // }


    
}