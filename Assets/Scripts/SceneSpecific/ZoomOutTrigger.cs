using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomOutTrigger : MonoBehaviour
{
    [SerializeField] private MainCamera mainCamera;
    [SerializeField] private Transform focusPoint;
    [SerializeField] private float zoomDistance;
    private Transform playerFollowPoint;

    void OnTriggerEnter(Collider otherBody) {
        if (otherBody.CompareTag("Player")) {
            playerFollowPoint = mainCamera.FollowTransform;
            mainCamera.SetFollowTransform(focusPoint, zoomDistance);
        }
    }

    void OnTriggerExit(Collider otherBody) {
        if (otherBody.CompareTag("Player")) {
            mainCamera.SetFollowTransform(playerFollowPoint, mainCamera.DefaultDistance);
        }
    }
}
