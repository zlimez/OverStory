using UnityEngine;

namespace Abyss.Environment.Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        public SpecyAttr specyAttr;
        public EnemyAttr attributes;
        public float health;
        public System.Action OnDeath;
        public System.Action<float> OnStrikePlayer;

        // TODO: Base damage from player, mods by enemy attributes/specy attr done here
        public bool TakeHit(float baseDamage)
        {
            Debug.Log($"{name} took {baseDamage} damage");
            health -= baseDamage;
            bool isDead = health <= 0;
            if (isDead)
            {
                attributes.isAlive = false;
                OnDeath?.Invoke();
            }
            return isDead;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                OnStrikePlayer?.Invoke(attributes.strength);
        }

        // NOTE: If enemy always moving (enter/exit trigger), this is not required
        // void OnTriggerStay2D(Collider2D other)
        // {
        //     if (other.CompareTag("Player"))
        //         OnStrikePlayer?.Invoke(attributes.strength);
        // }
    }
}
