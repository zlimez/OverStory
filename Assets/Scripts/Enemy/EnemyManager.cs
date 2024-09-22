using AnyPortrait;
using Environment.Enemy.Anim;
using UnityEngine;

namespace Environment.Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        public SpecyAttr specyAttr;
        public EnemyAttr attributes;
        public int health;
        public System.Action OnDeath;

        // TODO: Base damage from player, mods by enemy attributes/specy attr done here
        public bool TakeHit(int baseDamage)
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
    }
}
