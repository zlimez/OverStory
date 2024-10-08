using Abyss.EventSystem;
using UnityEngine;

namespace Abyss.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] PlayerController playerController;
        [SerializeField] Weapon weapon;
        [SerializeField] PlayerAttr playerAttributes;
        // public float health;

        void Start()
        {
            EventManager.InvokeEvent(PlayEventCollection.PlayerHealthChange, playerAttributes.health);
        }

        void OnEnable()
        {
            playerController.OnAttackEnded += weapon.Reset;
        }

        void OnDisable()
        {
            playerController.OnAttackEnded -= weapon.Reset;
        }

        void FixedUpdate()
        {
            if (playerController.IsAttacking)
                weapon.Strike(playerAttributes.strength);
        }

        // TODO: Base damage from player, mods by enemy attributes/specy attr done here
        public void TakeHit(float baseDamage, bool hasKnockback = false, Vector3 from = default)
        {
            if (playerAttributes.health == 0) return;
            if (playerController.TakeHit(hasKnockback, from)) return; // Is still taking last damage or isDead
            playerAttributes.health -= Mathf.Min(playerAttributes.health, baseDamage);
            EventManager.InvokeEvent(PlayEventCollection.PlayerHealthChange, playerAttributes.health);
            if (playerAttributes.health == 0) playerController.Die();
        }
    }
}
