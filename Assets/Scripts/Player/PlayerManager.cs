using Abyss.Environment.Enemy;
using Abyss.EventSystem;
using Abyss.SceneSystem;
using Tuples;
using UnityEngine;

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
        public Pair<AbyssScene, Vector3> LastRest;
        [Header("Settings")]
        public PlayerAttr PlayerAttr;
        [SerializeField] float purityLoseItemsThreshold = 40, portionLost = 0.5f;

        public bool BelowPurityThreshold => PlayerAttr.Purity < purityLoseItemsThreshold;

        void Start()
        {
            if (GameManager.Instance && GameManager.Instance.PlayerPersistence.JustDied)
            {
                GameManager.Instance.PlayerPersistence.JustDied = false;
                EventLedger.Instance.Record(PlayEvents.Respawn);
                transform.position = LastRest.Tail;
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
#else
            PlayerAttr = GameManager.Instance.PlayerPersistence.PlayerAttr;
            LastRest = GameManager.Instance.PlayerPersistence.LastRest;
#endif
            // For testing when want to assign weaponItem or lastRest from scene not from master
            if (weapon.weaponItem == null) weapon.weaponItem = GameManager.Instance.PlayerPersistence.WeaponItem;
            if (weapon.weaponItem != null) playerController.EquipWeapon(weapon.weaponItem);
            EventManager.InvokeEvent(PlayEvents.PlayerHealthChange, PlayerAttr.Health);
            EventManager.InvokeEvent(PlayEvents.PlayerPurityChange, PlayerAttr.Purity);
        }

        void Save(object input = null)
        {
            GameManager.Instance.PlayerPersistence.PlayerAttr = PlayerAttr;
            GameManager.Instance.PlayerPersistence.LastRest = LastRest;
            GameManager.Instance.PlayerPersistence.WeaponItem = weapon.weaponItem;
        }

        // TODO: Base damage from player, mods by enemy attributes/specy attr done here
        public void TakeHit(float baseDamage, bool hasKnockback = false, Vector3 from = default, float kbImpulse = 0)
        {
            if (PlayerAttr.Health == 0) return;
            if (playerController.TakeHit(hasKnockback, from, kbImpulse)) return; // Is still taking last damage or isDead
            UpdateHealth(-baseDamage);
            spriteFlash.StartFlash();
            if (PlayerAttr.Health == 0)
            {
                Save();
                GameManager.Instance.PlayerPersistence.JustDied = true;
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
