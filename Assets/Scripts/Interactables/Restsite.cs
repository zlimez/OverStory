using Abyss.EventSystem;
using Abyss.Player;
using Abyss.SceneSystem;
using Abyss.TimeManagers;
using UnityEngine;

public class RestSite : Interactable
{
    [SerializeField][Tooltip("Should be unique")] string restSiteName;
    [SerializeField] float purityRestoration;
    [SerializeField][Tooltip("Cooldown period before purity restoration effect can be applied again")] float purityRestoreCooldown;
    [SerializeField] float timeFastForward;
    float lastPurityRestoreTime = -1; // cooldown for purity to be restored
    // TODO: Include crafting, some functions to transferred to eventual rest site menu

    void OnEnable()
    {
        if (GameManager.Instance == null)
            EventManager.StartListening(SystemEvents.SystemsReady, Load);
        else Load();
        EventManager.StartListening(SystemEvents.SceneTransitStart, Save);
    }

    void OnDisable()
    {
        EventManager.StopListening(SystemEvents.SceneTransitStart, Save);
        EventManager.StopListening(SystemEvents.SystemsReady, Load);
    }

    void Load(object input = null)
    {
        if (GameManager.Instance.RestsitesPersistence.TryGetValue(restSiteName, out var purityRestoreTime)) lastPurityRestoreTime = purityRestoreTime;
        EventManager.StopListening(SystemEvents.SystemsReady, Load);
    }

    void Save(object input = null) => GameManager.Instance.RestsitesPersistence[restSiteName] = lastPurityRestoreTime;

    public override void Interact()
    {
        if (player.TryGetComponent<PlayerManager>(out var playerManager))
        {
            if (lastPurityRestoreTime == -1 || TimeCycle.Instance.TotalTime - lastPurityRestoreTime >= purityRestoreCooldown)
            {
                playerManager.UpdatePurity(purityRestoration);
                lastPurityRestoreTime = TimeCycle.Instance.TotalTime;
            }
            playerManager.LastRest.Head = SceneLoader.Instance.ActiveScene;
            playerManager.LastRest.Tail = transform.position;
            EventManager.InvokeEvent(PlayEvents.Rested, timeFastForward);
        }
    }
}
