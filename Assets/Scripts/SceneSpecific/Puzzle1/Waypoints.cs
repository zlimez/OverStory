using Chronellium.EventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    public int initialIndex = 34;
    public bool reachedEnd = false;
    public bool reachedStart = false;
    public Transform initialWaypoint;
    public int currentIndex;
    public Transform currentWaypoint;
    [SerializeField] private string ownerObjectName;
    public static readonly string ReachedEndSuffix = " reached end";

    private void Start()
    {
        Debug.Log("Total Waypoints: " + GetTotalWaypoints());
        currentIndex = initialIndex;
        currentWaypoint = initialWaypoint;
        EventManager.StartListening(StaticEvent.Core_ResetPuzzle, ResetPositions);
    }

    public int GetTotalWaypoints()
    {
        return transform.childCount;
    }

    public Transform GetNextWaypoint()
    {
        // Debug.Log("Current waypoint: " + currentIndex);
        currentIndex += 1;
        reachedStart = false;
        if (currentIndex >= transform.childCount)
        {
            currentIndex = transform.childCount - 1;
            reachedEnd = true;
            EventManager.InvokeEvent(new GameEvent(ownerObjectName + ReachedEndSuffix));
        }
        currentWaypoint = transform.GetChild(currentIndex);
        return currentWaypoint;
    }

    public Transform GetPrevWaypoint()
    {
        currentIndex -= 1;
        reachedEnd = false;
        if (currentIndex <= 0)
        {
            currentIndex = 0;
            reachedStart = true;
        }
        currentWaypoint = transform.GetChild(currentIndex);
        return currentWaypoint;
    }

    public void ResetPositions(object input = null)
    {
        currentIndex = initialIndex;
        currentWaypoint = initialWaypoint;
    }
}
