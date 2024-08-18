using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAddedController : MonoBehaviour
{
    public MainCamera mainCamera;
    public bool jumpEnabled = true;
    public Transform cameraFollowPoint;
    public SpriteRenderer playerSprite;
    public Vector3 goalVelocity;
    public float moveSpeed = 5f;
    [SerializeField]
    private float continiousJumpForce = 0.7f;
    [SerializeField]
    private float initialJumpForce = 5f;
    private float totalJumpForce = 0f;
    private float maxJumpForce = 8f;

    private Rigidbody rb;
    private Animator animator;
    public bool isGrounded = false;
    private bool isJumping = false;
    public float jumpInputBufferTime = 0.15f;
    private bool jumpInputBuffered = false;
    private float jumpInputBufferTimer = 0f;
    private float timeSinceLastJump = 0f;
    private bool hasPlayerReleaseJump = false;
    private bool hasTouchedWall = false;

    void OnEnable()
    {
        mainCamera.SetFollowTransform(cameraFollowPoint, mainCamera.DefaultDistance);
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = playerSprite.gameObject.GetComponent<Animator>();
    }

    void LateUpdate()
    {
        mainCamera.Move(Time.smoothDeltaTime);
    }

    void FixedUpdate()
    {
        // Allow moving while jumping
        //if (!isGrounded) return;
        // Check if grounded, because sometimes it still think that the character is jumping
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(transform.position, Vector3.down, out hitInfo, 1f);
        Debug.DrawRay(transform.position, Vector3.down, Color.red, 2f);
        if (hitInfo.collider != null && hitInfo.collider.gameObject.CompareTag("Ground") && timeSinceLastJump > 0.2f)
        {
            isGrounded = hit;
        }


        float hInput = Input.GetAxis("Horizontal");
        Vector2 vel = rb.velocity;

        bool sideWallHit = Physics.Raycast(transform.position + Vector3.up * 1, hInput * Vector3.right, out hitInfo, 0.5f);
        Debug.DrawRay(transform.position + Vector3.up * 5, hInput * Vector3.right, Color.green, 1f);
        // Sometimes the player get's stuck when moving towards a wall while jumpign
        if (!sideWallHit || (hitInfo.collider != null && hitInfo.collider.isTrigger))
        {
            if (!hasTouchedWall && isJumping)
            {

                // restrict the horizontal velocity when jumping
                vel.x = 0.7f * hInput * moveSpeed;
                if (hInput != 0)
                {
                    vel.y -= 0.01f; // Hack for some reason gravity is reduced when we are jump moving. this adds it back
                }

            }
            else
            {
                vel.x = hInput * moveSpeed;
            }
        }

        rb.velocity = vel;

        rb.velocity = Vector3.Lerp(rb.velocity, vel, 0.4f);

        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("Released Jump!!");
            hasPlayerReleaseJump = true;
        }

        // increase the jump impulse with longer hold 
        if (!hasPlayerReleaseJump && isJumping && Input.GetKey(KeyCode.Space))
        {
            if (!jumpEnabled) return;
            Debug.Log("Retry jump");
            if (totalJumpForce < maxJumpForce)
            {
                totalJumpForce += continiousJumpForce;
                rb.AddForce(Vector2.up * continiousJumpForce, ForceMode.Impulse);
            }
        }

        if (isGrounded && jumpInputBuffered && timeSinceLastJump > 0.2f)
        {
            if (!jumpEnabled) return;
            Debug.Log("start jump");
            animator.SetBool("isMoving", false);
            animator.SetBool("isJumping", true);
            totalJumpForce += initialJumpForce;
            rb.AddForce(Vector2.up * initialJumpForce, ForceMode.Impulse);
            jumpInputBuffered = false;
            isGrounded = false;
            isJumping = true;

            hasTouchedWall = false;

            hasPlayerReleaseJump = false;
            timeSinceLastJump = 0;
        }


        if (isJumping)
        {
            animator.SetBool("isMoving", false);
        }
        else
        {
            if (hInput == 0)
            {
                animator.SetBool("isMoving", false);
            }
            else
            {
                bool isRight = hInput > 0;
                playerSprite.flipX = !isRight;
                animator.SetBool("isMoving", true);
            }
        }

    }

    void Update()
    {
        // Debug.Log(" isGrounded " + isGrounded + " is Jumping " + isJumping);
        timeSinceLastJump += Time.deltaTime;
        if (jumpInputBuffered)
        {
            jumpInputBufferTimer -= Time.deltaTime;

            if (jumpInputBufferTimer <= 0f)
            {
                jumpInputBuffered = false;
            }
        }

        // This is to prevent the player being stuck when
        // performign jumping too close to a collider
        if (isJumping)
        {
            StartCoroutine(CheckForStuck());
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpInputBuffered = true;
            jumpInputBufferTimer = jumpInputBufferTime;
        }

    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            StopJumping();
        }
        foreach (ContactPoint contact in collision.contacts)
        {
            Vector3 contactPoint = contact.point;
            Vector3 contactNormal = contact.normal;

            // Get the direction of the collision by multiplying the contact normal with -1
            Vector3 collisionDirection = -1 * contactNormal;

            if (Mathf.Abs(collisionDirection.x) > 0.8)
            {
                hasTouchedWall = true;
            }
        }
    }

    IEnumerator CheckForStuck()
    {
        float yPos = this.transform.position.y;
        yield return new WaitForSeconds(Time.smoothDeltaTime * 2);
        if (this.transform.position.y == yPos)
        {
            // Player is stucked
            StopJumping();
        }
    }

    void StopJumping()
    {
        Debug.Log("Stop jump");
        isGrounded = true;
        isJumping = false;
        animator.SetBool("isJumping", false);
        totalJumpForce = 0f;
    }
}
