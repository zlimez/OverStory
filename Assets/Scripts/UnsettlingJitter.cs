using UnityEngine;

public class UnsettlingJitter : MonoBehaviour
{
    public float jitterSpeed = 1f; // speed of jittering
    public float jitterAmount = 10f; // amount of jittering

    private float jitterTimer = 0f;
    private Vector3 baseRotation;

    void Start()
    {
        baseRotation = transform.rotation.eulerAngles;
    }

    void Update()
    {
        jitterTimer += Time.deltaTime * jitterSpeed;
        float jitter = Mathf.Sin(jitterTimer) * jitterAmount;
        transform.rotation = Quaternion.Euler(baseRotation + new Vector3(0f, 0f, jitter));
    }
}