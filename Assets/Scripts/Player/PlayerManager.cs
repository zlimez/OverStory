using Abyss.EventSystem;
using UnityEngine;

namespace Abyss.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] PlayerController playerController;
        [SerializeField] Weapon weapon;
        [SerializeField] PlayerAttr playerAttr;
        public PlayerAttr PlayerAttr => playerAttr;

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
                weapon.Strike(playerAttr.Strength);
        }

        void Load(object input = null)
        {
            playerAttr = GameManager.Instance.PlayerPersistence.PlayerAttr;
            // FOR TESTING WHEN WANT TO ASSIGN WEAPON FROM SCENE ITSELF NOT MASTER
            if (weapon.weaponItem == null) weapon.weaponItem = GameManager.Instance.PlayerPersistence.WeaponItem;
            if (weapon.weaponItem != null) playerController.EquipWeapon(weapon.weaponItem);
            EventManager.InvokeEvent(PlayEvents.PlayerHealthChange, playerAttr.Health);
            EventManager.InvokeEvent(PlayEvents.PlayerPurityChange, playerAttr.Purity);
            EventManager.StopListening(SystemEvents.SystemsReady, Load);
        }

        void Save(object input = null)
        {
            GameManager.Instance.PlayerPersistence.PlayerAttr = playerAttr;
            GameManager.Instance.PlayerPersistence.WeaponItem = weapon.weaponItem;
        }

        // TODO: Base damage from player, mods by enemy attributes/specy attr done here
        public void TakeHit(float baseDamage, bool hasKnockback = false, Vector3 from = default)
        {
            if (playerAttr.Health == 0) return;
            if (playerController.TakeHit(hasKnockback, from)) return; // Is still taking last damage or isDead
            playerAttr.Health -= Mathf.Min(playerAttr.Health, baseDamage);
            EventManager.InvokeEvent(PlayEvents.PlayerHealthChange, playerAttr.Health);
            if (playerAttr.Health == 0) playerController.Die();
        }
    }
}
