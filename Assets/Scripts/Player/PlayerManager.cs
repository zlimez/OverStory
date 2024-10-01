using UnityEngine;

namespace Abyss.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] PlayerController playerController;
        [SerializeField] Weapon weapon;
        [SerializeField] PlayerAttr playerAttributes;
        public float health;
        public System.Action OnDeath;

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
            {
                weapon.Strike(playerAttributes.strength);
            }
        }

        // TODO: Base damage from player, mods by enemy attributes/specy attr done here
        public bool TakeHit(float baseDamage)
        {
            if (playerController.TakeHit()) return false; // Is still taking last damage
            health -= baseDamage;
            bool isDead = health <= 0;
            if (isDead)
                OnDeath?.Invoke();
            return isDead;
        }
    }
}
