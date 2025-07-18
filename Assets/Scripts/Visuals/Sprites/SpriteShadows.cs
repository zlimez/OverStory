using UnityEngine;

/* Attach this as a component to make a sprite be affect by shadows and also cast it's own shadows */
public class SpriteShadows : MonoBehaviour
{
    private void Start()
    {
        GetComponent<SpriteRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }
}