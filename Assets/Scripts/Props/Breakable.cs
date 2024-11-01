using UnityEngine;

public class Breakable : MonoBehaviour
{
    // TODO: Effects and health
    public virtual void TakeHit(float damage)
    {
        Destroy(gameObject);
    }
}
