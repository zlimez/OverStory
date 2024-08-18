using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableSeatsOld : Interactable
{
    public Windmill windmillBlades;
    public bool isMoving;
    public bool isMovingRight;
    public bool isTurning;
    public Waypoints waypoints;
    public GameObject normPlayer;
    public GameObject seatPosition;

    private IEnumerator moveRight;
    private IEnumerator moveLeft;
    private float moveSpeed = 0.1f;
    private float playerMoveSpeed = 0.2f;

    public bool isSeated;
    private float rotationY;

    private void Start()
    {
        moveRight = MoveRight();
        moveLeft = MoveLeft();
        rotationY = transform.rotation.y;
    }

    public override void Interact()
    {
        Debug.Log("Interacting");
        if (isSeated)
        {
            isSeated = false;
            normPlayer.GetComponent<Rigidbody>().useGravity = true;
        }
        else
        {
            isSeated = true;
            normPlayer.GetComponent<Rigidbody>().useGravity = false;
        }
    }

    private void FixedUpdate()
    {
        if (isSeated)
        {
            //Debug.Log("Set player transform");
            //player.transform.position = seatPosition.transform.position;
            normPlayer.transform.position = Vector3.MoveTowards(normPlayer.transform.position, seatPosition.transform.position, playerMoveSpeed);
        }

        if (windmillBlades.isRotating)
        {
            Debug.Log("Windmill rotating");
            isMoving = true;
            if (windmillBlades.isRotatingClockwise)
            {
                isMovingRight = true;
            }
            else
            {
                isMovingRight = false;
            }
        }
        else
        {
            isMoving = false;
        }

        if (isTurning)
        {
            if (isMovingRight)
            {
                if (rotationY == 180)
                {
                    isTurning = false;
                }
                else
                {
                    rotationY += 20;
                    transform.rotation = Quaternion.Euler(transform.rotation.x, rotationY, transform.rotation.z);
                }
            }
            else
            {
                if (rotationY == 0)
                {
                    isTurning = false;
                }
                else
                {
                    rotationY -= 20;
                    transform.rotation = Quaternion.Euler(transform.rotation.x, rotationY, transform.rotation.z);
                }
            }
        }

        if (!isMoving)
        {
            // Comment out these stopCoroutines if we want it to continue moving after windmill stops rotating.
            //StopCoroutine(moveRight);
            //StopCoroutine(moveLeft);
            return;
        }
        else
        {
            if (isMovingRight)
            {
                StopCoroutine(moveRight);
                StopCoroutine(moveLeft);
                StartCoroutine(moveRight);
            }
            else
            {
                StopCoroutine(moveRight);
                StopCoroutine(moveLeft);
                StartCoroutine(moveLeft);
            }
        }
    }

    public IEnumerator MoveRight()
    {
        while (!waypoints.reachedEnd)
        {
            if (transform.position != waypoints.currentWaypoint.position)
            {
                transform.position = Vector3.MoveTowards(transform.position, waypoints.currentWaypoint.position, moveSpeed);
            }
            else
            {
                waypoints.GetNextWaypoint();
                transform.position = Vector3.MoveTowards(transform.position, waypoints.currentWaypoint.position, moveSpeed);
            }
            yield return null;
        }
    }

    public IEnumerator MoveLeft()
    {
        while (!waypoints.reachedStart)
        {
            if (transform.position != waypoints.currentWaypoint.position)
            {
                transform.position = Vector3.MoveTowards(transform.position, waypoints.currentWaypoint.position, moveSpeed);
            }
            else
            {
                waypoints.GetPrevWaypoint();
                transform.position = Vector3.MoveTowards(transform.position, waypoints.currentWaypoint.position, moveSpeed);
            }
            yield return null;
        }
    }
}
