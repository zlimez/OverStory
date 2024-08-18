using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableSeats : Interactable
{
    public GameObject normPlayer;
    public GameObject seatPosition;
    private float playerMoveSpeed = 0.4f;

    public bool isSeated;

    public override void Interact()
    {
        Debug.Log("Interacting with cable seat");
        if (isSeated)
        {
            isSeated = false;
            normPlayer.GetComponent<Rigidbody>().useGravity = true;
            normPlayer.GetComponent<JumpAddedController>().jumpEnabled = true;
        }
        else
        {
            isSeated = true;
            normPlayer.GetComponent<Rigidbody>().useGravity = false;
            normPlayer.GetComponent<JumpAddedController>().jumpEnabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (isSeated)
        {
            normPlayer.transform.position = Vector3.MoveTowards(normPlayer.transform.position, seatPosition.transform.position, playerMoveSpeed);
        }
    }
}
