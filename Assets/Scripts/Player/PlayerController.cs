using System;
using Abyss.EventSystem;
using AnyPortrait;
using UnityEngine;
using UnityEngine.InputSystem;

// FIXME: When damaged seems to charge further
namespace Abyss.Player
{
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
			Attack,
			Death
		}

		// GameObject Components
		private Rigidbody2D rb2D;

		// Animation support
		[Header("Animation")]
		[SerializeField] apPortrait portrait;
		PlayerSfx _playerSfx;

		[SerializeField] float crossFadeSeconds = .01f;

		// Movement support
		[Header("Movement")]
		[SerializeField] float walkSpeed = 4f;
		[SerializeField] float runSpeed = 8f;
		bool shouldRun;

		float moveDir;
		private float currHorizontalSpeed;
		private bool isFacingLeft;

		[Header("Jump")]
		[SerializeField] private float jumpForce = 100f;
		// [SerializeField]
		[SerializeField] private float initialJumpImpulse = 1500f;
		private const float MAX_JUMP_TIME = 0.3f;
		private float remainingJumpTime = MAX_JUMP_TIME;
		public bool isGrounded;
		private bool isJumping;

		[Header("Dash")]
		[SerializeField] float dashImpulse = 2000f;
		[SerializeField] float dashCooldown = 1.0f;
		[SerializeField] float dashTime = 0.3f;
		private bool isDashAvailable = true;
		private bool isDashing = false;
		private float remainingDashTime;
		private float dashCountdown;

		[Header("Damage")]
		[SerializeField][Tooltip("Knock back and dash impulse should be the same order of magnitude to prevent player from dashing further when damaged")] float knockbackImpulse = 1000f;

		public bool IsAttacking { get; private set; } = false;
		bool isTakingDamage = false, isDead = false;
		public Action OnAttackEnded;
		public Action OnAttemptInteract;

		private State currState;

		#endregion


		#region Methods

		private void Awake()
		{
			rb2D = GetComponent<Rigidbody2D>();
			_playerSfx = GetComponent<PlayerSfx>();

			portrait.Initialize();
			currState = State.Idle;
			dashCountdown = dashCooldown;
		}

		private void Update()
		{
			if (!isDashing)
			{
				currHorizontalSpeed = (isDead || IsAttacking) ? 0 : moveDir * (shouldRun ? runSpeed : walkSpeed);
				if (!isTakingDamage) rb2D.velocity = new(currHorizontalSpeed, rb2D.velocity.y);
			}

			if (!IsAttacking && !isTakingDamage && !isDead) HandleState();
			if (!isTakingDamage)
			{
				if (rb2D.velocity.x > 0 && isFacingLeft)
					FlipSprite();
				else if (rb2D.velocity.x < 0 && !isFacingLeft)
					FlipSprite();
			}

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
				// Debug.Log($"Dashing {remainingDashTime}");
				if (remainingDashTime > 0f)
					remainingDashTime -= Time.fixedDeltaTime;
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
			switch (currState)
			{
				case State.Attack:
					if (!isGrounded)
						TransitionToState(State.Jump);
					else if (Mathf.Abs(currHorizontalSpeed) > walkSpeed + 0.1f)
						TransitionToState(State.Run);
					else if (currHorizontalSpeed == 0)
						TransitionToState(State.Idle);
					else TransitionToState(State.Walk);
					break;

				case State.Damage:
					if (!isGrounded)
						TransitionToState(State.Jump);
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
					if (isGrounded && Mathf.Abs(rb2D.velocity.y) < .1f)
					{
						if (Mathf.Abs(currHorizontalSpeed) > walkSpeed + 0.1f)
							TransitionToState(State.Run);
						else if (currHorizontalSpeed == 0)
							TransitionToState(State.Idle);
						else TransitionToState(State.Walk);
					}
					break;

				case State.Dash:
					if (!isDashing)
					{
						if (Mathf.Abs(currHorizontalSpeed) > walkSpeed + 0.1f)
							TransitionToState(State.Run);
						else if (currHorizontalSpeed == 0)
							TransitionToState(State.Idle);
						else TransitionToState(State.Walk);
					}
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
			if (IsAttacking || isTakingDamage || isDead) return;
			if (context.started && isGrounded)
			{
				_playerSfx.PlayJump();
				rb2D.AddForce(Vector2.up * initialJumpImpulse, ForceMode2D.Impulse);
				remainingJumpTime = MAX_JUMP_TIME;
				isJumping = true;
				TransitionToState(State.Jump);
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
				shouldRun = true;
			else if (context.canceled)
				shouldRun = false;
		}

		public void OnMove(InputAction.CallbackContext context)
		{
			moveDir = context.ReadValue<float>();
		}

		public void OnDash(InputAction.CallbackContext context)
		{
			if (IsAttacking || isTakingDamage || isDead) return;
			if (context.started && isDashAvailable)
			{
				_playerSfx.PlayDash();
				isDashing = true;
				remainingDashTime = dashTime;
				isDashAvailable = false;
				rb2D.AddForce((isFacingLeft ? Vector2.left : Vector2.right) * dashImpulse, ForceMode2D.Impulse);
				TransitionToState(State.Dash);
			}
		}

		public void OnAttack(InputAction.CallbackContext context)
		{
			if (isDashing || isTakingDamage || isDead) return;
			if (context.started)
			{
				if (IsAttacking) return;
				IsAttacking = true;
				currState = State.Attack;
				portrait.Play(State.Attack.ToString()); // CrossFade is glitchy here
			}
		}

		public void OnInteract(InputAction.CallbackContext context)
		{
			if (context.started)
				OnAttemptInteract?.Invoke();
		}

		public bool TakeHit(bool hasKnockback, Vector3 from)
		{
			if (isTakingDamage || isDead) return true;
			IsAttacking = false;
			_playerSfx.PlayHurt();
			if (isDashing)
			{
				// NOTE: compensate for dash -> damage then in Update method isTakingDamage check preserves dashing momentum
				isDashing = false;
				remainingDashTime = 0f;
				if (hasKnockback) rb2D.AddForce((rb2D.velocity.x > 0 ? Vector2.left : Vector2.right) * knockbackImpulse, ForceMode2D.Impulse);
			}
			isJumping = false;
			remainingJumpTime = 0f;
			isTakingDamage = true;
			if (hasKnockback) rb2D.AddForce(new Vector3(transform.position.x - from.x, 0, 0).normalized * knockbackImpulse, ForceMode2D.Impulse);
			TransitionToState(State.Damage);
			return false;
		}

		public void Die()
		{
			IsAttacking = false;
			isDashing = false;
			remainingDashTime = 0f;
			isJumping = false;
			remainingJumpTime = 0f;
			isTakingDamage = false;
			isDead = true;
			TransitionToState(State.Death);
		}

		void DamageEnd()
		{
			isTakingDamage = false;
		}

		void AttackEnd()
		{
			IsAttacking = false;
			OnAttackEnded?.Invoke();
		}

		void DeathEnd()
		{
			EventManager.InvokeEvent(PlayEventCollection.PlayerDeath);
		}
		#endregion
	}
}
