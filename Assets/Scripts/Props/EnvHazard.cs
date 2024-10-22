using Abyss.Player;
using UnityEngine;

public class EnvHazard : MonoBehaviour
{
    [SerializeField] float damage = 10;
    [SerializeField] float horizontalKnockbackImpulse = 0, fromTopKnockbackImpulse = 0, fromBottomKnockbackImpulse = 0;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var knockbackImpulse = (collision.contacts[0].normal.y > 0 ? fromTopKnockbackImpulse : fromBottomKnockbackImpulse) * Mathf.Abs(collision.contacts[0].normal.y) + horizontalKnockbackImpulse * Mathf.Abs(collision.contacts[0].normal.x);
            collision.gameObject.GetComponent<PlayerManager>().TakeHit(damage, true, collision.contacts[0].point, knockbackImpulse);
        }
    }
}
