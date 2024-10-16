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
                EventManager.StartListening(SystemEventCollection.SystemsReady, InitLoad);
            else Load();
            EventManager.StartListening(SystemEventCollection.SceneTransitDone, Load);
            EventManager.StartListening(SystemEventCollection.SceneTransitStart, Save);
        }

        void OnDisable()
        {
            playerController.OnAttackEnded -= _weapon.Reset;
            EventManager.StopListening(SystemEventCollection.SceneTransitDone, Load);
            EventManager.StopListening(SystemEventCollection.SceneTransitStart, Save);
        }

        void FixedUpdate()
        {
            if (playerController.IsAttacking)
                _weapon.Strike(_playerAttr.Strength);
        }


        // NOTE: TO SUPPORT DEV FLOW WHERE BASESCENEMANAGER IS USED TO LOAD MASTER AFTER SCENE IN EDITOR
        void InitLoad(object input = null)
        {
            _playerAttr = GameManager.Instance.PlayerPersistence.PlayerAttr;
            // FOR TESTING WHEN WANT TO ASSIGN WEAPON FROM SCENE ITSELF NOT MASTER
            if (_weapon.weaponItem == null) _weapon.weaponItem = GameManager.Instance.PlayerPersistence.WeaponItem;
            EventManager.InvokeEvent(PlayEventCollection.PlayerHealthChange, _playerAttr.Health);
            EventManager.InvokeEvent(PlayEventCollection.PlayerPurityChange, _playerAttr.Purity);
            EventManager.StopListening(SystemEventCollection.SystemsReady, InitLoad);
        }

        void Load(object input = null)
        {
            _playerAttr = GameManager.Instance.PlayerPersistence.PlayerAttr;
            _weapon.weaponItem = GameManager.Instance.PlayerPersistence.WeaponItem;
            EventManager.InvokeEvent(PlayEventCollection.PlayerHealthChange, _playerAttr.Health);
            EventManager.InvokeEvent(PlayEventCollection.PlayerPurityChange, _playerAttr.Purity);
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
            EventManager.InvokeEvent(PlayEventCollection.PlayerHealthChange, _playerAttr.Health);
            if (_playerAttr.Health == 0) playerController.Die();
        }
    }
}
