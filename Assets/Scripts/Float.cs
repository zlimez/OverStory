using UnityEngine;

public class Float : MonoBehaviour
{

    public float amplitude = 0.5f;
    public float frequency = 1f;

    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        Vector3 newPosition = originalPosition;
        newPosition.y += Mathf.Sin(Time.time * frequency) * amplitude;
        transform.localPosition = newPosition;
    }
}
