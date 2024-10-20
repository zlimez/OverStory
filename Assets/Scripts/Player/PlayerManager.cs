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
        [Header("Attributes")]
        public PlayerAttr PlayerAttr;
        public Pair<AbyssScene, Vector3> LastRest;

        void Start()
        {
            if (GameManager.Instance && GameManager.Instance.PlayerPersistence.JustDied)
            {
                GameManager.Instance.PlayerPersistence.JustDied = false;
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
        }

        void OnDisable()
        {
            playerController.OnAttackEnded -= weapon.Reset;
            EventManager.StopListening(SystemEvents.SceneTransitStart, Save);
            EventManager.StopListening(SystemEvents.SystemsReady, Load);
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
            playerAttr = GameManager.Instance.PlayerPersistence.PlayerAttr;
            lastRest = GameManager.Instance.PlayerPersistence.LastRest;
#endif
            // For testing when want to assign weaponItem or lastRest from scene not from master
            if (weapon.weaponItem == null) weapon.weaponItem = GameManager.Instance.PlayerPersistence.WeaponItem;
            if (weapon.weaponItem != null) playerController.EquipWeapon(weapon.weaponItem);
            EventManager.InvokeEvent(PlayEvents.PlayerHealthChange, PlayerAttr.Health);
            EventManager.InvokeEvent(PlayEvents.PlayerPurityChange, PlayerAttr.Purity);
            EventManager.StopListening(SystemEvents.SystemsReady, Load);
        }

        void Save(object input = null)
        {
            GameManager.Instance.PlayerPersistence.PlayerAttr = PlayerAttr;
            GameManager.Instance.PlayerPersistence.LastRest = LastRest;
            GameManager.Instance.PlayerPersistence.WeaponItem = weapon.weaponItem;
        }

        // TODO: Base damage from player, mods by enemy attributes/specy attr done here
        public void TakeHit(float baseDamage, bool hasKnockback = false, Vector3 from = default)
        {
            if (PlayerAttr.Health == 0) return;
            if (playerController.TakeHit(hasKnockback, from)) return; // Is still taking last damage or isDead
            PlayerAttr.Health -= Mathf.Min(PlayerAttr.Health, baseDamage);
            EventManager.InvokeEvent(PlayEvents.PlayerHealthChange, PlayerAttr.Health);
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
            SceneLoader.Instance.LoadWithMaster(LastRest.Head);
            EventManager.StopListening(PlayEvents.PlayerDeath, ReturnToRestScene);
        }

        public void UpdatePurity(float purityChange)
        {
            PlayerAttr.Purity = Mathf.Clamp(PlayerAttr.Purity + purityChange, 0, PlayerAttr.MaxPurity);
            EventManager.InvokeEvent(PlayEvents.PlayerPurityChange, PlayerAttr.Purity);
        }
    }
}
