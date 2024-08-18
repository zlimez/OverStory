using KinematicCharacterController;
using KinematicCharacterController.PlayerCameraCharacterSetup;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE: [Room 14][Level 2 corridor](2 scientists in both of the rooms)
// NOTE: Use case only for cutscene
public class SpawnController : MonoBehaviour
{
    // For setting player character back to original position before cutscene
    void Awake()
    {
        // Debug.Log("GameManager: " + GameManager.Instance);
        if (GameManager.Instance != null && GameManager.Instance.LastPosition != null)
        {
            // Debug.Log("lastPosition: " + GameManager.Instance.lastPosition);
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null && GameManager.Instance.RevertToLastPosition)
            {
                GameManager.Instance.RevertToLastPosition = false;

                // Disable Components
                player.GetComponent<PlayerCharacterController>().enabled = false;
                player.GetComponent<KinematicCharacterMotor>().enabled = false;
                player.GetComponent<PlayerInputHandler>().enabled = false;

                // Set player location and direction
                GameObject playerRoot = player.transform.GetChild(0).gameObject;
                player.GetComponent<PlayerCharacterController>().PlayerSprite.GetComponent<SpriteRenderer>().flipX = GameManager.Instance.LastPositionFlipX;
                player.GetComponent<CapsuleCollider>().transform.position = GameManager.Instance.LastPosition ?? player.transform.position;
                player.GetComponent<Rigidbody>().transform.position = GameManager.Instance.LastPosition ?? player.transform.position;
                player.GetComponent<KinematicCharacterMotor>().SetPosition(GameManager.Instance.LastPosition ?? player.transform.position);


                // Enable Components
                player.GetComponent<PlayerCharacterController>().enabled = true;
                player.GetComponent<KinematicCharacterMotor>().enabled = true;
                player.GetComponent<PlayerInputHandler>().enabled = true;

                // Reset GameManager parameters
                GameManager.Instance.LastPosition = null;
                GameManager.Instance.LastPositionFlipX = false;
            }
        }
    }
}
