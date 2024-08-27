using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Tuples;
using UnityEngine.Assertions;

// Camera's right vector is always parallel to the x-axis
public class MainCamera : MonoBehaviour
{
    [Header("Pathing")] public CinemachineSmoothPath cameraPath;

    public CinemachinePath walkPath;
    public GameObject player;
    public bool lookAtPlayer;

    [Header("Framing")] public Camera Camera;

    public Vector2 FollowPointFraming = new Vector2(0f, 0f);
    public float FollowingSharpness = 10000f;

    [Header("Distance")] public float DefaultDistance = 6f;

    public float MinDistance = 0f, MaxDistance = 10f;

    [Range(-90f, 90f)] public float DefaultVerticalAngle = 20f;

    [Range(-90f, 90f)] public float MinVerticalAngle = -90f;

    [Range(-90f, 90f)] public float MaxVerticalAngle = 90f;

    public float RotationSpeed = 1f, RotationSharpness = 10000f;
    public bool RotateWithPhysicsMover = false;

    [Header("Obstruction")] public float ObstructionCheckRadius = 0.2f;

    public LayerMask ObstructionLayers = -1;
    public float ObstructionSharpness = 10000f;
    public List<Collider> IgnoredColliders = new List<Collider>();

    public Transform Transform { get; private set; }
    public Transform FollowTransform { get; private set; }

    public Vector3 PlanarDirection { get; set; }
    public float TargetDistance { get; set; }

    private bool _distanceIsObstructed;
    private float _currentDistance, _targetVerticalAngle;
    private RaycastHit _obstructionHit;
    private int _obstructionCount;
    private RaycastHit[] _obstructions = new RaycastHit[MaxObstructions];
    private float _obstructionTime;
    private Vector3 _currentFollowPosition;
    private bool isInTransition = false;
    private float elapsedTransitionTime = 0.0f;
    [SerializeField] private float givenTransitionInterval = 0.25f;
    private Vector3 transitStartPosition, transitTargetPosition;
    private Quaternion transitStartRotation, transitTargetRotation;
    private float targetViewDistance;

    private const int MaxObstructions = 32;

    private void OnValidate()
    {
        DefaultDistance = Mathf.Clamp(DefaultDistance, MinDistance, MaxDistance);
        DefaultVerticalAngle = Mathf.Clamp(DefaultVerticalAngle, MinVerticalAngle, MaxVerticalAngle);
    }

    private void Awake()
    {
        Transform = this.transform;

        _currentDistance = DefaultDistance;
        TargetDistance = _currentDistance;

        _targetVerticalAngle = 0f;

        PlanarDirection = Vector3.forward;
    }

    public void SetFollowTransform(Transform t, float newDistance = -1) {
        targetViewDistance = newDistance == -1 ? _currentDistance : newDistance;
        // Debug.Log("Current distance is " + _currentDistance + " Target view distance is " + targetViewDistance);
        // The first camera binded object will be focused immediately instead of going through transition
        if (FollowTransform != null) isInTransition = true;

        // at this point the follow tranform is null so we can se it the prblem is that the transofrm ddoes not exist yet!
        FollowTransform = t;
        PlanarDirection = FollowTransform.forward;

        if (isInTransition)
        {
            transitStartPosition = this.transform.position;
            transitTargetPosition = FollowTransform.position - PlanarDirection * targetViewDistance;
            elapsedTransitionTime = 0;
            transitStartRotation = this.transform.rotation;
            transitTargetRotation = GetCameraRotation(PlanarDirection);
            StartCoroutine(Transition());
        }
        else
        {
            _currentFollowPosition = FollowTransform.position;
        }
    }

    // Set the transform that the camera will orbit around
    public void SetFollowTransform(Transform t, float fitWidth = 0, float fitHeight = 0)
    {
        if (fitWidth > 0 || fitHeight > 0)
        {
            SetFollowTransform(t, Mathf.Max(ZoomDistanceToFit(new Pair<float, float>(fitWidth, fitHeight)), DefaultDistance));
        }
        else
        {
            SetFollowTransform(t, -1);
        }
    }

    private float ZoomDistanceToFit(Pair<float, float> dimension)
    {
        Assert.IsFalse(dimension.head == 0 && dimension.tail == 0);
        var halfWidth = dimension.head / 2;
        var halfHeight = dimension.tail / 2;

        bool isWidthDirected = halfHeight == 0 || halfWidth / halfHeight >= Camera.aspect;
        if (isWidthDirected)
        {
            var translatedHeight = halfWidth / Camera.aspect;
            return translatedHeight / Mathf.Tan(Camera.fieldOfView / 2);
        }
        else
        {
            return halfHeight / Mathf.Tan(Camera.fieldOfView / 2);
        }
    }

    private Quaternion GetCameraRotation(Vector3 fwdVector)
    {
        Vector3 rightVector = Vector3.Cross(Vector3.up, fwdVector);
        Vector3 upVector = Vector3.Cross(fwdVector, rightVector);
        return Quaternion.LookRotation(fwdVector, upVector);
    }

    IEnumerator Transition()
    {
        // Transition camera smoothly to new following point
        while (elapsedTransitionTime < givenTransitionInterval)
        {
            elapsedTransitionTime += Time.deltaTime;
            transform.position = VectorCurves.CubicLerpVector(transitStartPosition, transitTargetPosition, elapsedTransitionTime / givenTransitionInterval);
            this.transform.rotation = RotationUtils.CubicLerpRotation(transitStartRotation, transitTargetRotation, elapsedTransitionTime / givenTransitionInterval);
            yield return null;
        }

        _currentFollowPosition = FollowTransform.position;
        _currentDistance = targetViewDistance;
        isInTransition = false;
    }

    public void Move(float deltaTime)
    {
        if (isInTransition) return;

        if (cameraPath)
        {
            // Match the walk path's progress
            float curWalkPoint = walkPath.FindClosestPoint(player.transform.position, 0, -1, 10);

            // Apply position
            this.transform.position = cameraPath.EvaluatePosition(curWalkPoint);
            if (lookAtPlayer)
            {
                this.transform.LookAt(FollowTransform);
            }
        }
        else if (FollowTransform)
        {
            PlanarDirection = FollowTransform.forward;

            // Find the smoothed follow position
            _currentFollowPosition = Vector3.Lerp(_currentFollowPosition, FollowTransform.position, 1f - Mathf.Exp(-FollowingSharpness * deltaTime));

            // Handle obstructions
            RaycastHit closestHit = new RaycastHit();
            closestHit.distance = Mathf.Infinity;
            _obstructionCount = Physics.SphereCastNonAlloc(_currentFollowPosition, ObstructionCheckRadius, -this.transform.forward, _obstructions, TargetDistance, ObstructionLayers, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < _obstructionCount; i++)
            {
                bool isIgnored = false;
                for (int j = 0; j < IgnoredColliders.Count; j++)
                {
                    if (IgnoredColliders[j] == _obstructions[i].collider)
                    {
                        isIgnored = true;
                        break;
                    }
                }
                for (int j = 0; j < IgnoredColliders.Count; j++)
                {
                    if (IgnoredColliders[j] == _obstructions[i].collider)
                    {
                        isIgnored = true;
                        break;
                    }
                }

                if (!isIgnored && _obstructions[i].distance < closestHit.distance && _obstructions[i].distance > 0)
                {
                    closestHit = _obstructions[i];
                }
            }

            // If obstructions detecter
            if (closestHit.distance < Mathf.Infinity)
            {
                _distanceIsObstructed = true;
            }
            // If no obstruction
            else
            {
                _distanceIsObstructed = false;
            }

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, GetCameraRotation(PlanarDirection), 1 - Mathf.Exp(-RotationSharpness * deltaTime));
            // Find the smoothed camera orbit position
            Vector3 targetPosition = _currentFollowPosition - (PlanarDirection * _currentDistance);

            // Handle framing
            targetPosition += this.transform.right * FollowPointFraming.x;
            targetPosition += this.transform.up * FollowPointFraming.y;

            // Apply position
            this.transform.position = targetPosition;
        }
    }
}