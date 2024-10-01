using UnityEngine;

namespace Abyss.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] PlayerController playerController;
        [SerializeField] Weapon weapon;
        [SerializeField] PlayerAttr playerAttributes;
        // public float health;
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
                weapon.Strike(playerAttributes.strength);
        }

        // TODO: Base damage from player, mods by enemy attributes/specy attr done here
        public void TakeHit(float baseDamage)
        {
            if (isDead) playerController.Die();
            if (playerController.TakeHit()) return; // Is still taking last damage or isDead
            if (health == 0) return;
            health -= Mathf.Min(health, baseDamage);
            bool isDead = health == 0;
        }
    }
}
