using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.PlayerCameraCharacterSetup;

public class SlowPlayerRigidBody : MonoBehaviour
{
    public JumpAddedController playerController;
    public float slowMoveSpeed;
    private float origMoveSpeed;
    private void OnTriggerExit(Collider other)
    {
        playerController.moveSpeed = origMoveSpeed;
    }
    private void OnTriggerEnter(Collider other)
    {
        origMoveSpeed = playerController.moveSpeed;
        playerController.moveSpeed = slowMoveSpeed;
    }
}
