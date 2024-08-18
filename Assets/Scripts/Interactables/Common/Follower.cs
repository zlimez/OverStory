using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public GameObject target;
    private Vector3 offsetToTarget;
    [SerializeField] private float followingSharpness = 1000f;
    // NOTE: Push would not work if player is colliding with the object collider
    [SerializeField] private float bufferDistance = 0.25f;
    [SerializeField] private float creatBufferTime = 0.25f;
    private IEnumerator bufferRoutine;
    private bool bufferDistCreated = false;

    void OnEnable() {
        offsetToTarget = target.transform.position - transform.position;
        offsetToTarget = offsetToTarget * (offsetToTarget.magnitude + bufferDistance) / offsetToTarget.magnitude;
        bufferRoutine = CreateBuffer();
        StartCoroutine(bufferRoutine);
    }

    void OnDisable() {
        if (bufferRoutine != null) StopCoroutine(bufferRoutine);
        bufferDistCreated = false;
    }

    void Update() {
        transform.position = Vector3.Lerp(transform.position, target.transform.position - offsetToTarget, 1f - Mathf.Exp(-followingSharpness * Time.deltaTime));
    }

    IEnumerator CreateBuffer() {
        float timeElapsed = 0;
        Vector3 initialPosition = transform.position;
        Vector3 targetPosition = target.transform.position - offsetToTarget;
        while (timeElapsed < creatBufferTime) {
            timeElapsed += Time.deltaTime;
            transform.position = VectorUtils.CubicLerpVector(initialPosition, targetPosition, timeElapsed / creatBufferTime);
            yield return null;
        }
        bufferRoutine = null;
    }
}
