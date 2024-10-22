using UnityEngine;

public class Breakable : MonoBehaviour
{
    // TODO: Effects and health
    public void TakeHit(float damage)
    {
        Destroy(gameObject);
    }
}
