using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using Chronellium.EventSystem;

public class SwimController : MonoBehaviour
{
    public MainCamera mainCamera;
    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private float propulsionStrength = 3f;
    [SerializeField] private float drag = 1f;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private float RotationSharpness = 10f;
    [SerializeField] private Transform cameraFollowPoint;
    public Transform Grabpoint;
    private Animator animator;
    public Animator SwimmerAnimator { get {return animator;} }
    public SpriteRenderer SwimmerRenderer { get {return playerSprite;} }
    private Rigidbody rb;
    private Vector3 currDirection;
    private bool lastFacingRight = true;
    // When movement is automated
    public bool Automated = false;
    public bool IsRotationFrozen = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = playerSprite.gameObject.GetComponent<Animator>();
    }

    void OnEnable()
    {
        mainCamera.SetFollowTransform(cameraFollowPoint, mainCamera.DefaultDistance);
    }

    void LateUpdate()
    {
        mainCamera.Move(Time.smoothDeltaTime);

        if (IsRotationFrozen) return;
        float zAngle = lastFacingRight ? Vector2.SignedAngle(Vector2.right, currDirection) : Vector2.SignedAngle(Vector2.left, currDirection);
        playerSprite.gameObject.transform.rotation = Quaternion.Slerp(playerSprite.gameObject.transform.rotation, Quaternion.Euler(0, 0, zAngle), 1 - Mathf.Exp(-RotationSharpness * Time.fixedDeltaTime));
    }

    void OnCollisionEnter(Collision other) {
        Debug.Log("Swim collided with " + other.gameObject.name);
    }

    void FixedUpdate()
    {
        // TODO: Add animator
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        currDirection = new Vector3(horizontalInput, verticalInput, 0f).normalized;

        if (!IsRotationFrozen && currDirection.sqrMagnitude > 0)
        {
            if (Mathf.Abs(horizontalInput) > 0)
            {
                bool isRight = Mathf.Sign(horizontalInput) == 1;
                playerSprite.flipX = !isRight;
                if (lastFacingRight != isRight) EventManager.InvokeEvent(StaticEvent.Common_PlayerChangeDirection, playerSprite.flipX);
                lastFacingRight = isRight;
            }
        }
        else if (currDirection.sqrMagnitude == 0 && rb.velocity.sqrMagnitude < 0.0025)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        Vector3 netForce = rb.velocity.sqrMagnitude < Mathf.Pow(maxSpeed, 2) ? currDirection * propulsionStrength - drag * rb.velocity : -drag * rb.velocity;
        rb.AddForce(netForce);
    }
}
