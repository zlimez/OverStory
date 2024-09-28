using System;
using AnyPortrait;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, PlayerControls.IPlayerActions
{
	#region Fields

	private enum State
	{
		Idle,
		Walk,
		Run,
		Jump,
		Dash,
		Damage,
		Attack
	}

	// GameObject Components
	private Rigidbody2D rb2D;

	// Animation support
	[Header("Animation")]
	[SerializeField] apPortrait portrait;

	[SerializeField] float crossFadeSeconds = .01f;

	// Movement support
	[Header("Movement")]
	[SerializeField] float walkSpeed = 4f;
	[SerializeField] float runSpeed = 8f;
	private Vector2 currVelocity;
	bool shouldRun;

	float moveDir;
	private float currHorizontalSpeed;
	private bool isFacingLeft;

	// Jump support
	[SerializeField] private float jumpForce = 100f;
	// [SerializeField]
	[SerializeField] private float initialJumpForce = 1500f;
	private const float MAX_JUMP_TIME = 0.3f;
	private float remainingJumpTime = MAX_JUMP_TIME;
	private bool isGrounded;
	private bool isJumping;

	// Dash support
	[SerializeField] private float dashForce = 2000f;
	private Vector2 dashForceToAdd;
	private bool isDashAvailable = true;
	private bool isDashing = false;
	private const float DASH_TIME = 0.25f;
	private float remainingDashTime = DASH_TIME;
	[SerializeField] float dashCooldown = 1.0f;
	private float dashCountdown;

	private State currState;
	bool isAttacking = false;

	#endregion


	#region Methods

	private void Awake()
	{
		rb2D = GetComponent<Rigidbody2D>();
		currVelocity = new Vector2();

		portrait.Initialize();
		currState = State.Idle;
		dashCountdown = dashCooldown;
	}

	private void Update()
	{
		HandleState();
		if (rb2D.velocity.x > 0 && isFacingLeft)
			FlipSprite();
		else if (rb2D.velocity.x < 0 && !isFacingLeft)
			FlipSprite();

		if (!isDashAvailable)
			dashCountdown -= Time.deltaTime;

		if (dashCountdown <= 0)
		{
			isDashAvailable = true;
			dashCountdown = dashCooldown;
		}
	}

	private void FixedUpdate()
	{
		if (isJumping && remainingJumpTime > 0f)
		{
			// Continue jump to max
			rb2D.AddForce(Vector2.up * jumpForce);
			remainingJumpTime -= Time.fixedDeltaTime;
		}

		if (isDashing)
		{
			if (remainingDashTime > 0f)
			{
				dashForceToAdd = isFacingLeft
					? Vector2.left * dashForce
					: Vector2.right * dashForce;
				rb2D.AddForce(dashForceToAdd);
				remainingDashTime -= Time.fixedDeltaTime;
			}
			else isDashing = false;
		}
	}



	private void OnCollisionEnter2D(Collision2D coll2D)
	{
		// Check if player is on the ground
		if (coll2D.gameObject.CompareTag("Ground"))
			isGrounded = true;
	}

	private void OnCollisionExit2D(Collision2D coll2D)
	{
		// Check if player is leaving the ground
		if (coll2D.gameObject.CompareTag("Ground"))
			isGrounded = false;
	}

	private void FlipSprite()
	{
		Vector3 currScale = gameObject.transform.localScale;
		currScale.x *= -1;
		gameObject.transform.localScale = currScale;

		isFacingLeft = !isFacingLeft;
	}

	// Animation stuff
	private void HandleState()
	{
		if (isAttacking) return;
		// Update movement
		currHorizontalSpeed = moveDir * (shouldRun ? runSpeed : walkSpeed);
		currVelocity.y = rb2D.velocity.y;
		currVelocity.x = currHorizontalSpeed;
		// Debug.Log("curr h speed: " + currHorizontalSpeed);
		rb2D.velocity = currVelocity;

		switch (currState)
		{
			case State.Attack:
				if (!isGrounded)
					TransitionToState(State.Jump);
				else if (isDashing)
					TransitionToState(State.Dash);
				else if (Mathf.Abs(currHorizontalSpeed) > walkSpeed + 0.1f)
					TransitionToState(State.Run);
				else if (currHorizontalSpeed == 0)
					TransitionToState(State.Idle);
				else TransitionToState(State.Walk);
				break;

			case State.Idle:
				if (currHorizontalSpeed != 0)
					TransitionToState(State.Walk);
				break;

			case State.Walk:
				if (Mathf.Abs(currHorizontalSpeed) > walkSpeed + .1f)
					TransitionToState(State.Run);
				else if (currHorizontalSpeed == 0)
					TransitionToState(State.Idle);
				break;

			case State.Run:
				if (Mathf.Abs(currHorizontalSpeed) < walkSpeed + 0.1f)
					TransitionToState(State.Walk);
				break;

			case State.Jump:
				if (isGrounded && Mathf.Abs(currVelocity.y) < .1f)
					TransitionToState(State.Idle);
				break;

			case State.Dash:
				if (!isDashing)
					TransitionToState(State.Idle);

				break;
		}
	}

	private void TransitionToState(State newState)
	{
		if (currState == newState) return;
		currState = newState;
		PlayAnimation(newState.ToString());
	}

	private void PlayAnimation(string animToPlay)
	{
		try
		{
			apAnimPlayData animData = portrait.CrossFade(animToPlay, crossFadeSeconds);
			if (animData == null)
				Debug.LogWarning("Failed to play animation " + animToPlay);
		}
		catch (Exception)
		{
			Debug.LogWarning(
				$"Error playing animation {animToPlay}. The portrait is likely not initialized"
			);
		}
	}

	#endregion


	#region Event Handlers

	public void OnJump(InputAction.CallbackContext context)
	{
		if (isAttacking) return;
		if (context.started)
		{
			// Start jumping
			if (isGrounded
			&& (
				currState == State.Idle
				|| currState == State.Walk
				|| currState == State.Run
			))
			{
				rb2D.AddForce(Vector2.up * initialJumpForce);
				remainingJumpTime = MAX_JUMP_TIME;
				isJumping = true;
				TransitionToState(State.Jump);
			}
		}
		else if (context.canceled)
		{
			// Stop jump and reset jumpTime
			isJumping = false;
			remainingJumpTime = 0f;
		}
	}

	public void OnRun(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			shouldRun = true;
			Debug.Log("Run started");
		}
		else if (context.canceled)
		{
			shouldRun = false;
			Debug.Log("Run ended");
		}
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		moveDir = context.ReadValue<float>();
		// Debug.Log("curr h speed: " + currHorizontalSpeed);
	}

	public void OnDash(InputAction.CallbackContext context)
	{
		if (isAttacking) return;
		if (isDashAvailable
			&& (
				currState == State.Idle
				|| currState == State.Walk
				|| currState == State.Run
				|| currState == State.Jump
			)
		)
		{
			isDashing = true;
			remainingDashTime = DASH_TIME;
			isDashAvailable = false;
			TransitionToState(State.Dash);
		}
	}

	public void OnAttack(InputAction.CallbackContext context)
	{
		if (isDashing) return;
		if (context.started)
		{
			if (isAttacking) return;
			isAttacking = true;
			currState = State.Attack;
			portrait.Play(State.Attack.ToString()); // CrossFade is glitchy here
		}
	}

	void AttackEnd()
	{
		isAttacking = false;
	}
	#endregion
}
