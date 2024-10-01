using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Abyss.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] PlayerController playerController;
        [SerializeField] Weapon weapon;
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
            if (playerController.IsAttacking && !weapon.HitMade)
            {

            }
        }

        // TODO: Base damage from player, mods by enemy attributes/specy attr done here
        public bool TakeHit(float baseDamage)
        {
            health -= baseDamage;
            bool isDead = health <= 0;
            if (isDead)
            {
                attributes.isAlive = false;
                OnDeath?.Invoke();
            }
            return isDead;
        }

        void Hit(PlayerManager player)
        {

        }
    }
}
}
