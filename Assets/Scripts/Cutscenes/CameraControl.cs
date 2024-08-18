using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// Credits to: https://www.raziel619.com/blog/screenshake-in-unity-canvas-alternative-approach/
public class CameraControl : MonoBehaviour
{

    public PlayableDirector timeline;
    public Transform cameraFollowPoint;
    public GameObject[] movableActors;

    public Vector3[][] vectorsToMoveActor;
    public Vector3[] cameraPositions;
    private Vector3[] originalMovableActorsPositions;
    private Vector3 originalCameraPosition;
    private int currCameraIndex, currActorIndex;

    private void Start()
    {
        currCameraIndex = 0;
    }
    public void SaveInitialPositions() 
    {
        originalCameraPosition = cameraFollowPoint.transform.position;
        for (int i = 0; i < movableActors.Length; i++) {
            originalMovableActorsPositions[i] = movableActors[i].transform.position;
        }
    }

    public void MoveCameraToNextPosition() {
        if (currCameraIndex >= cameraPositions.Length) { 
            cameraFollowPoint.transform.position = originalCameraPosition;
            return;
        }
        cameraFollowPoint.transform.position = cameraPositions[currCameraIndex];
        currCameraIndex++;
    }

    // this one not used or tested yet
    public void MoveActorsToNextPosition() {
        if (currCameraIndex >= vectorsToMoveActor.Length) { return; }

        // doublecheck
        for (int i = 0; i < movableActors.Length; i++) {
            movableActors[i].transform.position += vectorsToMoveActor[currActorIndex][i];
        }

        currActorIndex++;
    }
}