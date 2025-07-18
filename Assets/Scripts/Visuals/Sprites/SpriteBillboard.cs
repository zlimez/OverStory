using UnityEngine;

/* Attach this as a component to make a sprite always face the player */
public class SpriteBillboard : MonoBehaviour
{
    private void Update()
    {
        transform.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, 0f);
    }
}