using System;
using Abyss.EventSystem;
using AnyPortrait;
using Tuples;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

// FIXME: When damaged seems to charge further
namespace Abyss.Player
{
	public class PlayerController : MonoBehaviour, PlayerControls.IPlayerActions
	{
		#region Fields

		private enum State // For reference only
		{
			Idle_Nil, Walk_Nil, Run_Nil, Jump_Nil, Dash_Nil, Damage_Nil, Attack_Nil, Death_Nil,
			Idle_Axe_t1, Walk_Axe_t1, Run_Axe_t1, Jump_Axe_t1, Dash_Axe_t1, Damage_Axe_t1, Attack_Axe_t1, Death_Axe_t1,
		}

		bool IsIdleState => currState.ToString().StartsWith("Idle");
		bool IsWalkState => currState.ToString().StartsWith("Walk");
		bool IsRunState => currState.ToString().StartsWith("Run");
		bool IsJumpState => currState.ToString().StartsWith("Jump");
		bool IsDashState => currState.ToString().StartsWith("Dash");
		bool IsDamageState => currState.ToString().StartsWith("Damage");
		bool IsAttackState => currState.ToString().StartsWith("Attack");
		bool IsDeathState => currState.ToString().StartsWith("Death");
		string BaseState => currState.ToString().Split('_')[0];

		private Rigidbody2D rb2D;

		// Animation support
		[Header("Animation")]
		[SerializeField] apPortrait portrait;
		[SerializeField] float crossFadeSeconds = .01f;

		// Movement support
		[Header("Movement")]
		[SerializeField] float walkSpeed = 4f;
		[SerializeField] float runSpeed = 8f;
		bool shouldRun;

		float moveDir;
		private float currHorizontalSpeed;
		public bool IsFacingLeft { get; private set; } = false;

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
		private bool isDashAvailable = true, hasGroundedSinceDash = true;
		private bool isDashing = false;
		private float remainingDashTime;
		private float dashCountdown;

		[Header("Damage")][SerializeField][Tooltip("Knock back and dash impulse should be the same order of magnitude to prevent player from dashing further when damaged")] float knockbackImpulse = 1000f;
		[Header("Weapon Mapping")][SerializeField][Tooltip("Should match animation name suffix in anyportrait")] Pair<WeaponItem, string>[] weaponMapping;
		[SerializeField] VisualEffect weaponSlash;
		[SerializeField] string attackEvent = "Attack", xDirParam = "xDir", sizeParam = "size";
		[SerializeField][Tooltip("COnversion between weapon radius and slash vfx size")] float slashSizeConversion = 8f / 1.75f;
		float _slashSize;

		public bool IsAttacking { get; private set; } = false;
		bool isTakingDamage = false, isDead = false;
		public Action OnAttackEnded;
		public Action OnAttemptInteract;

		private State currState;
		[NonSerialized] public string Weapon = "Nil";

		PlayerSfx _playerSfx;
		#endregion


		#region Methods

		private void Awake()
		{
			rb2D = GetComponent<Rigidbody2D>();
			_playerSfx = GetComponent<PlayerSfx>();

			portrait.Initialize();
			currState = Enum.Parse<State>($"Idle_{Weapon}");
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
				if (rb2D.velocity.x > 0 && IsFacingLeft)
					FlipSprite();
				else if (rb2D.velocity.x < 0 && !IsFacingLeft)
					FlipSprite();
			}

			if (!isDashAvailable)
				dashCountdown -= Time.deltaTime;

			if (dashCountdown <= 0 && hasGroundedSinceDash)
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
			{
				isGrounded = true;
				hasGroundedSinceDash = true;
			}
		}

		private void OnCollisionExit2D(Collision2D coll2D)
		{
			// Check if player is leaving the ground
			if (coll2D.gameObject.CompareTag("Ground"))
			{
				if (isDashing)
					hasGroundedSinceDash = false;
				isGrounded = false;
			}
		}

		void OnEnable()
		{
			EventManager.StartListening(PlayEvents.WeaponEquipped, EquipWeapon);
			EventManager.StartListening(PlayEvents.WeaponUnequipped, UnequipWeapon);
		}
		void OnDisable()
		{
			EventManager.StopListening(PlayEvents.WeaponEquipped, EquipWeapon);
			EventManager.StopListening(PlayEvents.WeaponUnequipped, UnequipWeapon);
		}

		public void EquipWeapon(object input)
		{
			int ind = Array.FindIndex(weaponMapping, pair => pair.Head == (WeaponItem)input);
			if (ind == -1)
				Debug.LogWarning("Weapon not found in mapping");
			else
			{
				Weapon = weaponMapping[ind].Tail;
				_slashSize = weaponMapping[ind].Head.Radius * slashSizeConversion;
				TransitionToState(Enum.Parse<State>($"{BaseState}_{Weapon}"));
			}
		}

		void UnequipWeapon(object input = null)
		{
			_slashSize = 0;
			Weapon = "Nil";
		}

		private void FlipSprite()
		{
			Vector3 currScale = gameObject.transform.localScale;
			currScale.x *= -1;
			gameObject.transform.localScale = currScale;

			IsFacingLeft = !IsFacingLeft;
		}

		// Animation stuff
		private void HandleState()
		{
			if (IsAttackState)
			{
				if (!isGrounded)
					TransitionToState(Enum.Parse<State>($"Jump_{Weapon}"));
				else if (Mathf.Abs(currHorizontalSpeed) > walkSpeed + 0.1f)
					TransitionToState(Enum.Parse<State>($"Run_{Weapon}"));
				else if (currHorizontalSpeed == 0)
					TransitionToState(Enum.Parse<State>($"Idle_{Weapon}"));
				else TransitionToState(Enum.Parse<State>($"Walk_{Weapon}"));
			}
			else if (IsDamageState)
			{
				if (!isGrounded)
					TransitionToState(Enum.Parse<State>($"Jump_{Weapon}"));
				else if (Mathf.Abs(currHorizontalSpeed) > walkSpeed + 0.1f)
					TransitionToState(Enum.Parse<State>($"Run_{Weapon}"));
				else if (currHorizontalSpeed == 0)
					TransitionToState(Enum.Parse<State>($"Idle_{Weapon}"));
				else TransitionToState(Enum.Parse<State>($"Walk_{Weapon}"));
			}
			else if (IsIdleState)
			{
				if (currHorizontalSpeed != 0)
					TransitionToState(Enum.Parse<State>($"Walk_{Weapon}"));
			}
			else if (IsWalkState)
			{
				if (Mathf.Abs(currHorizontalSpeed) > walkSpeed + .1f)
					TransitionToState(Enum.Parse<State>($"Run_{Weapon}"));
				else if (currHorizontalSpeed == 0)
					TransitionToState(Enum.Parse<State>($"Idle_{Weapon}"));
			}
			else if (IsRunState)
			{
				if (Mathf.Abs(currHorizontalSpeed) < walkSpeed + 0.1f)
					TransitionToState(Enum.Parse<State>($"Walk_{Weapon}"));
			}
			else if (IsDashState)
			{
				if (!isDashing)
				{
					if (Mathf.Abs(currHorizontalSpeed) > walkSpeed + 0.1f)
						TransitionToState(Enum.Parse<State>($"Run_{Weapon}"));
					else if (currHorizontalSpeed == 0)
						TransitionToState(Enum.Parse<State>($"Idle_{Weapon}"));
					else TransitionToState(Enum.Parse<State>($"Walk_{Weapon}"));
				}
			}
			else if (IsJumpState)
			{
				if (isGrounded && Mathf.Abs(rb2D.velocity.y) < .1f)
				{
					if (Mathf.Abs(currHorizontalSpeed) > walkSpeed + 0.1f)
						TransitionToState(Enum.Parse<State>($"Run_{Weapon}"));
					else if (currHorizontalSpeed == 0)
						TransitionToState(Enum.Parse<State>($"Idle_{Weapon}"));
					else TransitionToState(Enum.Parse<State>($"Walk_{Weapon}"));
				}
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
				TransitionToState(Enum.Parse<State>($"Jump_{Weapon}"));
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

		public void OnMove(InputAction.CallbackContext context) => moveDir = context.ReadValue<float>();

		public void OnDash(InputAction.CallbackContext context)
		{
			if (IsAttacking || isTakingDamage || isDead) return;
			if (context.started && isDashAvailable)
			{
				_playerSfx.PlayDash();
				isDashing = true;
				remainingDashTime = dashTime;
				isDashAvailable = false;
				hasGroundedSinceDash = false;
				rb2D.AddForce((IsFacingLeft ? Vector2.left : Vector2.right) * dashImpulse, ForceMode2D.Impulse);
				TransitionToState(Enum.Parse<State>($"Dash_{Weapon}"));
			}
		}

		public void OnAttack(InputAction.CallbackContext context)
		{
			if (isDashing || isTakingDamage || isDead) return;
			if (context.started)
			{
				if (IsAttacking) return;
				IsAttacking = true;
				currState = Enum.Parse<State>($"Attack_{Weapon}");
				portrait.Play($"Attack_{Weapon}"); // CrossFade is glitchy here
				weaponSlash.SetInt(xDirParam, IsFacingLeft ? -1 : 1);
				weaponSlash.SetFloat(sizeParam, _slashSize);
				weaponSlash.SendEvent(attackEvent);
			}
		}

		public void OnInteract(InputAction.CallbackContext context)
		{
			if (context.started)
				OnAttemptInteract?.Invoke();
		}

		public bool TakeHit(bool hasKnockback, Vector3 from, float kbImpulse)
		{
			if (isTakingDamage || isDead) return true;
			IsAttacking = false;
			_playerSfx.PlayHurt();
			if (isDashing)
			{
				// NOTE: compensate for dash -> damage then in Update method isTakingDamage check preserves dashing momentum
				isDashing = false;
				remainingDashTime = 0f;
				if (hasKnockback) rb2D.AddForce((rb2D.velocity.x > 0 ? Vector2.left : Vector2.right) * (knockbackImpulse + kbImpulse), ForceMode2D.Impulse);
			}
			isJumping = false;
			remainingJumpTime = 0f;
			isTakingDamage = true;
			if (hasKnockback) rb2D.AddForce(new Vector3(transform.position.x - from.x, 0, 0).normalized * (knockbackImpulse + kbImpulse), ForceMode2D.Impulse);
			TransitionToState(Enum.Parse<State>($"Damage_{Weapon}"));
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
			TransitionToState(Enum.Parse<State>($"Death_{Weapon}"));
		}

		void DamageEnd() => isTakingDamage = false;

		void AttackEnd()
		{
			IsAttacking = false;
			OnAttackEnded?.Invoke();
		}

		void DeathEnd() => EventManager.InvokeEvent(PlayEvents.PlayerDeath);
		#endregion
	}
}
