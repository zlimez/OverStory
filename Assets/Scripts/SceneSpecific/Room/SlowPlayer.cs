using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.PlayerCameraCharacterSetup;

public class SlowPlayer : MonoBehaviour
{
    public PlayerCharacterController player;
    public float slowMoveSpeed;
    private float origMoveSpeed;
    private void OnTriggerExit(Collider other)
    {
        player.MaxStableMoveSpeed = origMoveSpeed;
    }
    private void OnTriggerEnter(Collider other)
    {
        origMoveSpeed = player.MaxStableMoveSpeed;
        player.MaxStableMoveSpeed = slowMoveSpeed;
    }
}
