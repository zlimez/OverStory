using Abyss.Environment.Enemy;
using Abyss.EventSystem;
using Abyss.SceneSystem;
using Tuples;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Abyss.Player
{
    public class PlayerManager : MonoBehaviour
    {
#if UNITY_EDITOR
        [Header("Dev Only")]
        [SerializeField] bool getPlayerAttrFrmMaster = true;
        [SerializeField] bool getLastRestFrmMaster = true;
#endif
        [Header("References")]
        [SerializeField] PlayerController playerController;
        [SerializeField] Weapon weapon;
        [SerializeField] SpriteFlash spriteFlash;
        public RefPair<AbyssScene, Vector3> LastRest = new();
        public PlayerAttr PlayerAttr; // By reference when assign to gamemanager playerattr changes are on the same instance
        [Header("Settings")]
        [SerializeField] float purityLoseItemsThreshold = 40, portionLost = 0.5f;
        [SerializeField] Pair<AbyssScene, Transform>[] sceneCrossSpawnPoints;

        public bool BelowPurityThreshold => PlayerAttr.Purity < purityLoseItemsThreshold;
        public WeaponItem WeaponItem => weapon.weaponItem;

        void Start()
        {
            if (GameManager.Instance && GameManager.Instance.PlayerPersistence.JustDied)
            {
                GameManager.Instance.PlayerPersistence.JustDied = false;
                EventLedger.Instance.Record(PlayEvents.Respawn);
                transform.position = LastRest.Tail;
            }

            if (SceneLoader.Instance && SceneLoader.Instance.LastScene != AbyssScene.None)
            {
                foreach (var spawnPoint in sceneCrossSpawnPoints)
                    if (spawnPoint.Head == SceneLoader.Instance.LastScene)
                    {
                        transform.position = spawnPoint.Tail.position;
                        break;
                    }
            }
        }

        void OnEnable()
        {
            playerController.OnAttackEnded += weapon.Reset;
            if (GameManager.Instance == null)
                EventManager.StartListening(SystemEvents.SystemsReady, Load);
            else Load();
            EventManager.StartListening(SystemEvents.SceneTransitStart, Save);
            EventManager.StartListening(PlayEvents.PlayerActionPurityChange, UpdateActionPurity);
            EventManager.StartListening(PlayEvents.PlayerFriendlinessPurityChange, UpdateFriendlinessPurity);
        }

        void OnDisable()
        {
            playerController.OnAttackEnded -= weapon.Reset;
            EventManager.StopListening(SystemEvents.SceneTransitStart, Save);
            EventManager.StopListening(SystemEvents.SystemsReady, Load);
            EventManager.StopListening(PlayEvents.PlayerActionPurityChange, UpdateActionPurity);
            EventManager.StopListening(PlayEvents.PlayerFriendlinessPurityChange, UpdateFriendlinessPurity);
        }

        void FixedUpdate()
        {
            if (playerController.IsAttacking)
                weapon.Strike(PlayerAttr.Strength);
        }

        void Load(object input = null)
        {
#if UNITY_EDITOR
            if (getPlayerAttrFrmMaster)
                PlayerAttr = GameManager.Instance.PlayerPersistence.PlayerAttr;
            if (getLastRestFrmMaster)
                LastRest = GameManager.Instance.PlayerPersistence.LastRest;
            // For testing when want to assign weaponItem or lastRest from scene not from master
            if (weapon.weaponItem == null) weapon.weaponItem = GameManager.Instance.PlayerPersistence.WeaponItem;
#else
            weapon.weaponItem = GameManager.Instance.PlayerPersistence.WeaponItem;
            PlayerAttr = GameManager.Instance.PlayerPersistence.PlayerAttr;
            LastRest = GameManager.Instance.PlayerPersistence.LastRest;
#endif
            if (weapon.weaponItem != null) playerController.EquipWeapon(weapon.weaponItem);
            EventManager.InvokeEvent(PlayEvents.PlayerHealthChange, PlayerAttr.Health);
            EventManager.InvokeEvent(PlayEvents.PlayerPurityChange, PlayerAttr.Purity);
        }

        void Save(object input = null)
        {
#if UNITY_EDITOR
            if (!getPlayerAttrFrmMaster)
                GameManager.Instance.PlayerPersistence.PlayerAttr = PlayerAttr;
            if (!getLastRestFrmMaster)
                GameManager.Instance.PlayerPersistence.LastRest = LastRest;
#endif
        }

        // TODO: Base damage from player, mods by enemy attributes/specy attr done here
        public void TakeHit(float baseDamage, string striker = "", bool hasKnockback = false, Vector3 from = default, float kbImpulse = 0)
        {
            if (PlayerAttr.Health == 0) return;
            if (playerController.TakeHit(hasKnockback, from, kbImpulse)) return; // Is still taking last damage or isDead
            UpdateHealth(-baseDamage);
            spriteFlash.StartFlash();
            if (PlayerAttr.Health == 0)
            {
                Save();
                GameManager.Instance.PlayerPersistence.JustDied = true;
                GameManager.Instance.PlayerPersistence.KilledBy = striker;
                EventManager.StartListening(PlayEvents.PlayerDeath, ReturnToRestScene);
                playerController.Die();
            }
        }

        void ReturnToRestScene(object input = null)
        {
            PlayerAttr.Health = PlayerAttr.MaxHealth;
            if (PlayerAttr.Purity < purityLoseItemsThreshold)
                GameManager.Instance.Inventory.Clear();
            else GameManager.Instance.Inventory.RanRemovePortion(portionLost);

            SceneLoader.Instance.PrepLoadWithMaster(LastRest.Head);
            EventManager.StopListening(PlayEvents.PlayerDeath, ReturnToRestScene);
        }

        public void UpdateHealth(float healthChange)
        {
            PlayerAttr.Health = Mathf.Clamp(PlayerAttr.Health + healthChange, 0, PlayerAttr.MaxHealth);
            EventManager.InvokeEvent(PlayEvents.PlayerHealthChange, PlayerAttr.Health);
        }

        public void UpdatePurity()
        {
            PlayerAttr.Purity = PlayerAttr.ActionPurity + PlayerAttr.FriendlinessPurity;
            EventManager.InvokeEvent(PlayEvents.PlayerPurityChange, PlayerAttr.Purity);
        }

        public void UpdateActionPurity(object input)
        {
            float actionPurityChange = (float)input;
            PlayerAttr.ActionPurity = Mathf.Clamp(PlayerAttr.ActionPurity + actionPurityChange, 0, PlayerAttr.MaxActionPurity);
        }

        public void UpdateFriendlinessPurity(object input)
        {
            float averageFriendliness = EnemyPopManager.Instance.FriendlinessAverage;
            Pair<float, float> friendlinessRange = EnemyPopManager.Instance.FriendlinessRange;
            PlayerAttr.FriendlinessPurity = averageFriendliness * 40.0f / friendlinessRange.Tail;
            UpdatePurity();
        }
    }
}
