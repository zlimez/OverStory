using Abyss.EventSystem;
using UnityEngine;

namespace Abyss.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] PlayerController playerController;
        [SerializeField] Weapon _weapon;
        [SerializeField] PlayerAttr _playerAttr;

        void OnEnable()
        {
            playerController.OnAttackEnded += _weapon.Reset;
            if (GameManager.Instance == null)
                EventManager.StartListening(SystemEvents.SystemsReady, Load);
            else Load();
            EventManager.StartListening(SystemEvents.SceneTransitStart, Save);
        }

        void OnDisable()
        {
            playerController.OnAttackEnded -= _weapon.Reset;
            EventManager.StopListening(SystemEvents.SceneTransitStart, Save);
            EventManager.StopListening(SystemEvents.SystemsReady, Load);
        }

        void FixedUpdate()
        {
            if (playerController.IsAttacking)
                _weapon.Strike(_playerAttr.Strength);
        }

        void Load(object input = null)
        {
            _playerAttr = GameManager.Instance.PlayerPersistence.PlayerAttr;
            // FOR TESTING WHEN WANT TO ASSIGN WEAPON FROM SCENE ITSELF NOT MASTER
            if (_weapon.weaponItem == null) _weapon.weaponItem = GameManager.Instance.PlayerPersistence.WeaponItem;
            EventManager.InvokeEvent(PlayEvents.PlayerHealthChange, _playerAttr.Health);
            EventManager.InvokeEvent(PlayEvents.PlayerPurityChange, _playerAttr.Purity);
            EventManager.StopListening(SystemEvents.SystemsReady, Load);
        }

        void Save(object input = null)
        {
            GameManager.Instance.PlayerPersistence.PlayerAttr = _playerAttr;
            GameManager.Instance.PlayerPersistence.WeaponItem = _weapon.weaponItem;
        }

        // TODO: Base damage from player, mods by enemy attributes/specy attr done here
        public void TakeHit(float baseDamage, bool hasKnockback = false, Vector3 from = default)
        {
            if (_playerAttr.Health == 0) return;
            if (playerController.TakeHit(hasKnockback, from)) return; // Is still taking last damage or isDead
            _playerAttr.Health -= Mathf.Min(_playerAttr.Health, baseDamage);
            EventManager.InvokeEvent(PlayEvents.PlayerHealthChange, _playerAttr.Health);
            if (_playerAttr.Health == 0) playerController.Die();
        }
    }
}
