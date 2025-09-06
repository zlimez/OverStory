using System;
using Abyss.EventSystem;
using Abyss.Player;
using Abyss.SceneSystem;
using Abyss.TimeManagers;
using UnityEngine;

namespace Abyss.Interactables
{
    public class RestSite : Interactable
    {
        [SerializeField][Tooltip("Should be unique")] string restSiteName;
        [SerializeField] float purityRestoration;
        [SerializeField][Tooltip("Cooldown period before purity restoration effect can be applied again")] float purityRestoreCooldown;
        [SerializeField] GameObject bonfire;
        [SerializeField] float timeFastForward;
        [SerializeField] float lastPurityRestoreTime = -1; // cooldown for purity to be restored
                                                           // TODO: Include crafting, some functions to transferred to eventual rest site menu
        [SerializeField] BlueprintItem bandageBP;
        [SerializeField] DynamicEvent firstDeathEvent;
        [SerializeField] BlueprintItem[] lureBlueprints;
        [SerializeField] Transform lureSpawnPoint;

        PlayerManager _playerManager;

        void OnEnable()
        {
            if (GameManager.Instance == null)
                EventManager.Subscribe(SystemEvents.SystemsReady, Load);
            else Load();
            EventManager.Subscribe(SystemEvents.SceneTransitStart, Save);
        }

        void OnDisable()
        {
            EventManager.Unsubscribe(SystemEvents.SceneTransitStart, Save);
            EventManager.Unsubscribe(SystemEvents.SystemsReady, Load);
            EventManager.Unsubscribe(PlayEvents.RestEnd, EndRest);
        }

        // Lure spawning can be done here as player death always trigger scene reload
        void Load(object input = null)
        {
            if (GameManager.Instance.RestsitesPersistence.ContainsKey(restSiteName))
                lastPurityRestoreTime = GameManager.Instance.RestsitesPersistence[restSiteName];
            if (GameManager.Instance.PlayerPersistence.KilledBy == "") return;
            foreach (var lureBP in lureBlueprints)
                if ((lureBP.objectItem as Lure).specy.specyName == GameManager.Instance.PlayerPersistence.KilledBy && !GameManager.Instance.Inventory.MaterialCollection.Contains(lureBP))
                {
                    Instantiate(lureBP.itemPrefab, lureSpawnPoint.position, Quaternion.identity);
                    break;
                }
            if (!GameManager.Instance.Inventory.MaterialCollection.Contains(bandageBP))
                if (EventLedger.Instance != null)
                    FirstGiveBandage();
                else EventManager.Subscribe(SystemEvents.LedgerReady, FirstGiveBandage);
        }

        void FirstGiveBandage(object input = null)
        {
            if (!EventLedger.Instance.HasOccurred(new GameEvent(firstDeathEvent.EventName)))
                Instantiate(bandageBP.itemPrefab, lureSpawnPoint.position + Vector3.right, Quaternion.identity);
            EventManager.Unsubscribe(SystemEvents.LedgerReady, FirstGiveBandage);
        }

        void Save(object input = null) => GameManager.Instance.RestsitesPersistence[restSiteName] = lastPurityRestoreTime;

        public override void Interact()
        {
            if (player.TryGetComponent<PlayerManager>(out _playerManager))
            {
                EventManager.InvokeEvent(PlayEvents.RestStart);
                if (lastPurityRestoreTime == -1 || TimeCycle.Instance.TotalTime - lastPurityRestoreTime >= purityRestoreCooldown)
                {
                    EventManager.InvokeEvent(PlayEvents.ActionPurityChange, purityRestoration);
                    lastPurityRestoreTime = TimeCycle.Instance.TotalTime;
                }

                _playerManager.UpdateHealth(PlayerAttr.MaxHealth);
                _playerManager.LastRest.Head = SceneLoader.Instance.ActiveScene;
                _playerManager.LastRest.Tail = transform.position;
                player.GetComponent<PlayerController>().Rest(); // Aesthetics only
                bonfire.SetActive(true);

                TimeCycle.Instance.Forward(timeFastForward / 2);
                if (GameManager.Instance.Inventory.MaterialCollection.HasItemType(ItemType.Blueprints))
                    EventManager.InvokeEvent(PlayEvents.CraftingPostEntered, (Action)EndCraft);
                else EventManager.InvokeEvent(PlayEvents.InRest);
                EventManager.Subscribe(PlayEvents.RestEnd, EndRest);

                base.Interact();
            }
        }

        void EndCraft() => EventManager.InvokeEvent(PlayEvents.InRest);

        void EndRest(object input = null)
        {
            bonfire.SetActive(false);
            TimeCycle.Instance.Forward(timeFastForward / 2);
            player.GetComponent<PlayerController>().Unrest(_playerManager.WeaponItem);
            _playerManager = null;
            EventManager.Unsubscribe(PlayEvents.RestEnd, EndRest);
        }
    }
}