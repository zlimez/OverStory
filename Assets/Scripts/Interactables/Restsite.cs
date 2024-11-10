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

        PlayerManager _playerManager;

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
            if (GameManager.Instance.RestsitesPersistence.ContainsKey(restSiteName))
                lastPurityRestoreTime = GameManager.Instance.RestsitesPersistence[restSiteName];
        }

        void Save(object input = null) => GameManager.Instance.RestsitesPersistence[restSiteName] = lastPurityRestoreTime;

        public override void Interact()
        {
            if (player.TryGetComponent<PlayerManager>(out _playerManager))
            {
                if (lastPurityRestoreTime == -1 || TimeCycle.Instance.TotalTime - lastPurityRestoreTime >= purityRestoreCooldown)
                {
                    EventManager.InvokeEvent(PlayEvents.PlayerActionPurityChange, purityRestoration);
                    _playerManager.UpdatePurity();
                    lastPurityRestoreTime = TimeCycle.Instance.TotalTime;
                }

                _playerManager.UpdateHealth(PlayerAttr.MaxHealth);
                _playerManager.LastRest.Head = SceneLoader.Instance.ActiveScene;
                _playerManager.LastRest.Tail = transform.position;
                player.GetComponent<PlayerController>().UnequipWeapon(); // Aesthetics only
                bonfire.SetActive(true);

                TimeCycle.Instance.Forward(timeFastForward / 2);
                if (GameManager.Instance.Inventory.MaterialCollection.HasItemType(ItemType.Blueprints))
                    EventManager.InvokeEvent(PlayEvents.CraftingPostEntered);

                EventManager.StartListening(PlayEvents.RestEnd, EndRest);
            }
        }

        void EndRest(object input = null)
        {
            bonfire.SetActive(false);
            TimeCycle.Instance.Forward(timeFastForward / 2);
            _playerManager = null;
            if (_playerManager.WeaponItem != null) player.GetComponent<PlayerController>().EquipWeapon(_playerManager.WeaponItem);
            EventManager.StopListening(PlayEvents.RestEnd, EndRest);
        }
    }
}