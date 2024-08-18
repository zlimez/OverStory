using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableSeatsOW : MonoBehaviour
{
    public WindmillRotator windmillOW;
    public bool isMoving;
    public bool isMovingRight;
    public Waypoints waypoints;

    private IEnumerator moveRight;
    private IEnumerator moveLeft;
    public float moveSpeed = 4f;

    void OnEnable()
    {
        moveRight = MoveRight();
        moveLeft = MoveLeft();

        windmillOW.OnRotatingAnticlockwise += StartMoveLeft;
        windmillOW.OnRotatingClockwise += StartMoveRight;
        windmillOW.OnStopped += Stop;
    }

    void OnDisable()
    {
        moveRight = null;
        moveLeft = null;

        windmillOW.OnRotatingAnticlockwise += StartMoveLeft;
        windmillOW.OnRotatingClockwise += StartMoveRight;
        windmillOW.OnStopped += Stop;
    }

    private void StartMoveLeft()
    {
        Debug.Log("Moving cable car to the left");
        StopCoroutine(moveRight);
        StartCoroutine(moveLeft);
    }

    private void StartMoveRight()
    {
        Debug.Log("Moving cable car to the right");
        StopCoroutine(moveLeft);
        StartCoroutine(moveRight);
    }

    private void Stop()
    {
        StopCoroutine(moveRight);
        StopCoroutine(moveLeft);
    }

    IEnumerator MoveRight()
    {
        while (!waypoints.reachedEnd)
        {
            Vector3 toWaypoint = waypoints.currentWaypoint.position - transform.position;
            Vector3 direction = toWaypoint.normalized;
            Vector3 deltaPosition = direction * moveSpeed * Mathf.Abs(windmillOW.CurrRotationVel) / windmillOW.PeakRotationSpeed * Time.deltaTime;
            transform.position += deltaPosition;
            if (deltaPosition.sqrMagnitude >= toWaypoint.sqrMagnitude) waypoints.GetNextWaypoint();
            yield return null;
        }
    }

    IEnumerator MoveLeft()
    {
        while (!waypoints.reachedStart)
        {
            Vector3 toWaypoint = waypoints.currentWaypoint.position - transform.position;
            Vector3 direction = toWaypoint.normalized;
            Vector3 deltaPosition = direction * moveSpeed * Mathf.Abs(windmillOW.CurrRotationVel) / windmillOW.PeakRotationSpeed * Time.deltaTime;
            transform.position += deltaPosition;
            if (deltaPosition.sqrMagnitude >= toWaypoint.sqrMagnitude) waypoints.GetPrevWaypoint();
            yield return null;
        }
    }
}
