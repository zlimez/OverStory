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
		Dash
	}
	
	// GameObject Components
	private Rigidbody2D rb2D;

	// Animation support
	[Header("Animation")]
	[SerializeField]
	private apPortrait portrait;

	[SerializeField]
	private float crossFadeSeconds = .01f;

	// Movement support
	[Header("Movement")]
	[SerializeField]
	private float walkSpeed = 4f;

	[SerializeField]
	private float runSpeed = 8f;
	private Vector2 currVelocity;
	private bool shouldRun;

	private float currHorizontalSpeed;
	private bool isFacingLeft;

	// Jump support
	[SerializeField]
	private float jumpForce = 25f;
	[SerializeField]
	private float initialJumpForce = 1800f;
	private const float MAX_JUMP_TIME = 0.3f;
	private float remainingJumpTime = MAX_JUMP_TIME;
	private bool isGrounded;
	private bool isJumping;

	// Dash support
	[SerializeField]
	private float dashForce = 3000f;
	private bool isDashAvailable = true;
	private const float MAX_DASH_COOLDOWN = 1.2f;
	private float dashCooldown = MAX_DASH_COOLDOWN;

	private State currState;

	#endregion


	#region Methods

	private void Awake()
	{
		rb2D = GetComponent<Rigidbody2D>();
		currVelocity = new Vector2();

		portrait.Initialize();
		currState = State.Idle;
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
			dashCooldown = MAX_DASH_COOLDOWN;
		}
		
		if (isJumping && remainingJumpTime > 0f) 
		{
			// Continue jump to max
			rb2D.AddForce(Vector2.up * jumpForce);
			remainingJumpTime -= Time.deltaTime;
		}
	}

	private void HandleState()
	{
		switch (currState)
		{
			case State.Idle:
				if (currHorizontalSpeed != 0)
				{
					TransitionToState(State.Walk);
				}
				break;

			case State.Walk:
				if (Mathf.Abs(currHorizontalSpeed) > walkSpeed + .1f)
				{
					TransitionToState(State.Run);
				}
				else if (currHorizontalSpeed == 0)
				{
					TransitionToState(State.Idle);
				}
				break;

			case State.Run:
				if (Mathf.Abs(currHorizontalSpeed) <= walkSpeed - 0.1f)
				{
					TransitionToState(State.Walk);
				}
				break;

			case State.Jump:
				if (isGrounded && Mathf.Abs(currVelocity.y) < .1f)
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
		// Debug.Log($"Changing to state {newState.ToString()}");
		currState = newState;
		PlayAnimation(newState.ToString());
	}

	private void OnCollisionEnter2D(Collision2D coll2D)
	{
		// Check if player is on the ground
		if (coll2D.gameObject.tag == "Ground")
		{
			isGrounded = true;
		}
	}

	private void OnCollisionExit2D(Collision2D coll2D)
	{
		// Check if player is leaving the ground
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

	#endregion


	#region Event Handlers

	public void OnJump(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			Debug.Log("Jump context - started");
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
			Debug.Log("Jump context - canceled");
			// Stop jump and reset jumpTime
			isJumping = false;
			remainingJumpTime = 0f;
		}
		
		/*
		// TODO: remove old code below
		if (isGrounded
			&& (
				currState == State.Idle
				|| currState == State.Walk
				|| currState == State.Run
			)
		)
		{
			rb2D.AddForce(Vector2.up * jumpForce);
			isGrounded = false;
			TransitionToState(State.Jump);
		}
		*/
	}

	public void OnRun(InputAction.CallbackContext context)
	{
		shouldRun = context.ReadValueAsButton();

		if (shouldRun && currState == State.Walk)
		{
			currHorizontalSpeed = shouldRun ? runSpeed : walkSpeed;
			currHorizontalSpeed *= isFacingLeft ? -1 : 1;
			TransitionToState(State.Run);
		}
		else if (!shouldRun && currState == State.Run)
		{
			TransitionToState(State.Walk);
		}
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		float maxSpeed = shouldRun ? runSpeed : walkSpeed;
		currHorizontalSpeed = context.ReadValue<float>() * maxSpeed;

		if (currState == State.Idle || currState == State.Walk || currState == State.Run)
		{
			TransitionToState(Mathf.Abs(currHorizontalSpeed) > 0 ? State.Walk : State.Idle);
		}
	}

	public void OnDash(InputAction.CallbackContext context)
	{
		if (isDashAvailable
			&& (
				currState == State.Idle
				|| currState == State.Walk
				|| currState == State.Run
				|| currState == State.Jump
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

	public void OnLook(InputAction.CallbackContext context)
	{
		throw new NotImplementedException();
	}

	public void OnFire(InputAction.CallbackContext context)
	{
		throw new NotImplementedException();
	}

	#endregion
}
