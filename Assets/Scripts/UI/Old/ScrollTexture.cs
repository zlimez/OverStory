using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ScrollTexture : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer _image;
    [SerializeField] private Vector2 speed;
    void Start()
    {
        _image = GetComponent<SpriteRenderer>();
        _image.material = new Material(_image.material); //Clone the original material
    }
    void Update()
    {
        _image.material.mainTextureOffset += speed * Time.deltaTime;
    }
}