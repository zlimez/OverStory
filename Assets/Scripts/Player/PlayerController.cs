using System;
using AnyPortrait;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Fields
    private enum State
    {
        Idle,
        Walk,
        Run,
        Jump,
        Dash
    }

    [Header("Animation")]
    [SerializeField]
    private apPortrait portrait;

    [SerializeField]
    private float crossFadeSeconds = .01f;

    private Rigidbody2D rb2D;

    // Movement support
    [Header("Movement")]
    [SerializeField]
    private float speed = 8f;
    private Vector2 currVelocity;
    private float currHorizontalSpeed;
    private bool isFacingLeft;

    // Jump support
    [SerializeField]
    private float jumpForce = 200f;
    private bool isGrounded;

    // Dash support
    [SerializeField]
    private float dashForce = 3000f;
    private bool isDashAvailable = true;
    private float dashCooldown = 1.2f;

    private State currentState;

    #endregion


    #region Methods

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        currVelocity = new Vector2();

        portrait.Initialize();
        currentState = State.Idle;
    }

    private void Update()
    {
        HandleState();
        if (rb2D.velocity.x > 0 && isFacingLeft)
        {
            FlipSprite();
        }
        else if (rb2D.velocity.x < 0 && !isFacingLeft)
        {
            FlipSprite();
        }

        if (!isDashAvailable)
        {
            dashCooldown -= Time.deltaTime;
        }
        if (dashCooldown <= 0)
        {
            isDashAvailable = true;
            dashCooldown = 2f;
        }
    }

    private void HandleState()
    {
        switch (currentState)
        {
            case State.Idle:
                if (currHorizontalSpeed != 0)
                {
                    TransitionToState(State.Walk);
                }
                break;

            case State.Walk:
                if (Mathf.Abs(currHorizontalSpeed) > speed * 0.5f)
                {
                    TransitionToState(State.Run);
                }
                else if (currHorizontalSpeed == 0)
                {
                    TransitionToState(State.Idle);
                }
                break;

            case State.Run:
                if (Mathf.Abs(currHorizontalSpeed) <= speed * 0.5f)
                {
                    TransitionToState(State.Walk);
                }
                break;

            case State.Jump:
                if (isGrounded && Mathf.Approximately(currVelocity.y, 0))
                {
                    TransitionToState(State.Idle);
                }
                break;

            case State.Dash:
                if (isDashAvailable)
                {
                    TransitionToState(State.Idle);
                }
                break;
        }

        // Update movement
        currVelocity.y = rb2D.velocity.y;
        currVelocity.x = currHorizontalSpeed;
        rb2D.velocity = currVelocity;
    }

    private void TransitionToState(State newState)
    {
        Debug.Log($"Changing to state {newState.ToString()}");
        currentState = newState;
        PlayAnimation(newState.ToString());
    }

    private void OnCollisionEnter2D(Collision2D coll2D)
    {
        if (coll2D.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D coll2D)
    {
        if (coll2D.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }

    private void FlipSprite()
    {
        Vector3 currScale = gameObject.transform.localScale;
        currScale.x *= -1;
        gameObject.transform.localScale = currScale;

        isFacingLeft = !isFacingLeft;
    }

    #endregion


    #region Event Handlers

    public void OnJump(InputAction.CallbackContext context)
    {
        if (
            isGrounded
            && (
                currentState == State.Idle
                || currentState == State.Walk
                || currentState == State.Run
            )
        )
        {
            rb2D.AddForce(Vector2.up * jumpForce);
            isGrounded = false;
            TransitionToState(State.Jump);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        currHorizontalSpeed = context.ReadValue<float>() * speed;

        if (currentState == State.Idle || currentState == State.Walk || currentState == State.Run)
        {
            TransitionToState(Mathf.Abs(currHorizontalSpeed) > 0 ? State.Walk : State.Idle);
        }
    }

    private void PlayAnimation(string animToPlay)
    {
        try
        {
            apAnimPlayData animData = portrait.CrossFadeAt(
                animToPlay,
                frame: portrait.GetAnimationCurrentFrame(animToPlay), // This so that the walk animation and run animation seamlessly transition at the same offset. Frame 3 is always when the back arm fully swings back
                crossFadeSeconds
            );
            if (animData == null)
            {
                Debug.LogWarning("Failed to play animation " + animToPlay);
            }
        }
        catch (Exception)
        {
            Debug.LogWarning(
                $"Error playing animation {animToPlay}. The portrait is likely not initialized"
            );
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (
            isDashAvailable
            && (
                currentState == State.Idle
                || currentState == State.Walk
                || currentState == State.Run
            )
        )
        {
            Vector2 forceToAdd = isFacingLeft
                ? Vector2.left * dashForce
                : Vector2.right * dashForce;
            rb2D.AddForce(forceToAdd);
            isDashAvailable = false;
            TransitionToState(State.Dash);
        }
    }

    #endregion
}
