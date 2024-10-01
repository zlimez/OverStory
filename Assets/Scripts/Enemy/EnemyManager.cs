using Abyss.Player;
using AnyPortrait;
using Environment.Enemy.Anim;
using UnityEngine;

namespace Environment.Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        public SpecyAttr specyAttr;
        public EnemyAttr attributes;
        public float health;
        public System.Action OnDeath;

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

        // baseDamage from move, mods by enemy attrs
        public void Hit(PlayerManager player, float baseDamage)
        {
            player.TakeHit(baseDamage);
        }
    }
}
