using System;
using System.Collections;
using Abyss.EventSystem;
using Abyss.Player.Spells;
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

		public Transform Foot;

		// Animation support
		[Header("Animation")]
		[SerializeField] apPortrait portrait;
		[SerializeField] float crossFadeSeconds = .01f;

		// Movement support
		[Header("Movement")]
		[SerializeField] float walkSpeed = 4f;
		[SerializeField] float runSpeed = 8f;
		bool _shouldRun;

		float _moveDir, _currXSpeed;
		public bool IsFacingLeft { get; private set; } = false;

		[Header("Jump")]
		[SerializeField] float jumpForce = 100f;
		[SerializeField] float initialJumpImpulse, extraDescentAcceleration = 5f;
		[SerializeField][Tooltip("Extra time window given to player to jump the moment they leave ground i.e. leave a platform) ")] float jumpBuffer = 0.1f;
		[SerializeField][Tooltip("If player becomes grounded with this window after a jump command, the jump will take effect")] float preLandJumpBuffer = 0.1f;
		float _jumpBufferCountdown = 0f, _preLandJumpBufferCountdown = 0f;
		const float MAX_JUMP_TIME = 0.3f;
		float _jumpTimeLeft = MAX_JUMP_TIME;
		public bool IsGrounded, IsJumping;
		public float InitJumpImpulse => initialJumpImpulse;

		[Header("Dash")]
		[SerializeField] float dashImpulse = 2000f;
		[SerializeField] float dashCooldown = 1.0f, dashTime = 0.3f;
		bool dashAvail = true, groundedSinceDash = true, isDashing = false;
		float _dashTimeLeft, _dashCountdown;

		readonly int GROUND_LAYER = 7;

		// Attacking
		[Header("Damage")]
		[SerializeField][Tooltip("Knock back and dash impulse should be the same order of magnitude to prevent player from dashing further when damaged")] float knockbackImpulse = 1000f;
		[SerializeField][Tooltip("Ember spell Fire Column Prefab")] GameObject fireColumn;

		[Header("Weapon")]
		[SerializeField][Tooltip("Should match animation name suffix in anyportrait")] Pair<WeaponItem, string>[] weaponMapping;
		[SerializeField] VisualEffect weaponSlash;
		[SerializeField][Tooltip("Used by slash VFX")] string attackEvent = "Attack", xDirParam = "xDir", sizeParam = "size";
		[SerializeField][Tooltip("Conversion between weapon radius and slash vfx size")] float slashSizeConversion = 8f / 1.75f;
		float _slashSize;

		public bool IsAttacking { get; private set; } = false;
		bool _isTakingDamage = false, _isDead = false, _isResting = false;
		public Action OnAttackEnded, OnAttemptInteract;


		Rigidbody2D rb2D;
		State currState;
		[NonSerialized] public string Weapon = "Nil";

		PlayerSfx _playerSfx;
		#endregion


		#region Lifecyle Methods

		private void Awake()
		{
			rb2D = GetComponent<Rigidbody2D>();
			_playerSfx = GetComponent<PlayerSfx>();

			portrait.Initialize();
			currState = Enum.Parse<State>($"Idle_{Weapon}");
			_dashCountdown = dashCooldown;
		}

		private void Update()
		{
			if (IsFrozen) _moveDir = 0;

			if (!isDashing)
			{
				_currXSpeed = (_isDead || IsAttacking) ? 0 : _moveDir * (_shouldRun ? runSpeed : walkSpeed);
				if (!_isTakingDamage) rb2D.velocity = new(_currXSpeed, rb2D.velocity.y);
			}

			if (_jumpBufferCountdown > 0)
				_jumpBufferCountdown = Mathf.Max(0, _jumpBufferCountdown - Time.deltaTime);
			if (_preLandJumpBufferCountdown > 0)
				_preLandJumpBufferCountdown = Mathf.Max(0, _preLandJumpBufferCountdown - Time.deltaTime);

			if (!IsAttacking && !_isTakingDamage && !_isDead) HandleState();
			if (!_isTakingDamage)
			{
				if (rb2D.velocity.x > 0 && IsFacingLeft)
					FlipSprite();
				else if (rb2D.velocity.x < 0 && !IsFacingLeft)
					FlipSprite();
			}

			if (!dashAvail)
				_dashCountdown -= Time.deltaTime;

			if (_dashCountdown <= 0 && groundedSinceDash)
			{
				dashAvail = true;
				_dashCountdown = dashCooldown;
			}
		}

		private void FixedUpdate()
		{
			if (IsJumping && _jumpTimeLeft > 0f)
			{
				// Continue jump to max
				rb2D.AddForce(Vector2.up * jumpForce);
				_jumpTimeLeft -= Time.fixedDeltaTime;
			}

			if (!IsGrounded && rb2D.velocity.y < 0)
				rb2D.AddForce(extraDescentAcceleration * rb2D.mass * Vector2.down);

			if (isDashing)
			{
				// Debug.Log($"Dashing {remainingDashTime}");
				if (_dashTimeLeft > 0f)
					_dashTimeLeft -= Time.fixedDeltaTime;
				else isDashing = false;
			}
		}

		private void OnCollisionEnter2D(Collision2D coll2D)
		{
			// Check if player is on the ground
			if (coll2D.gameObject.layer == (int)AbyssSettings.Layers.Ground)
			{
				IsGrounded = true;
				groundedSinceDash = true;
				if (_preLandJumpBufferCountdown > 0)
				{
					Jump();
					_preLandJumpBufferCountdown = 0;
				}
			}
		}

		private void OnCollisionExit2D(Collision2D coll2D)
		{
			// Check if player is leaving the ground
			if (coll2D.gameObject.layer == (int)AbyssSettings.Layers.Ground)
			{
				if (isDashing)
					groundedSinceDash = false;
				IsGrounded = false;
				if (!IsJumping)
					_jumpBufferCountdown = jumpBuffer;
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
		#endregion

		#region Other Methods
		public void Rest()
		{
			_isResting = true;
			UnequipWeapon();
		}

		public void Unrest(WeaponItem weaponItem)
		{
			_isResting = false;
			if (weaponItem != null) EquipWeapon(weaponItem);
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

		public void UnequipWeapon(object input = null)
		{
			_slashSize = 0;
			Weapon = "Nil";
			TransitionToState(Enum.Parse<State>($"{BaseState}_{Weapon}"));
		}


		// Animation stuff
		void HandleState()
		{
			if (IsAttackState)
			{
				if (!IsGrounded)
					TransitionToState(Enum.Parse<State>($"Jump_{Weapon}"));
				else if (Mathf.Abs(_currXSpeed) > walkSpeed + 0.1f)
					TransitionToState(Enum.Parse<State>($"Run_{Weapon}"));
				else if (_currXSpeed == 0)
					TransitionToState(Enum.Parse<State>($"Idle_{Weapon}"));
				else TransitionToState(Enum.Parse<State>($"Walk_{Weapon}"));
			}
			else if (IsDamageState)
			{
				if (!IsGrounded)
					TransitionToState(Enum.Parse<State>($"Jump_{Weapon}"));
				else if (Mathf.Abs(_currXSpeed) > walkSpeed + 0.1f)
					TransitionToState(Enum.Parse<State>($"Run_{Weapon}"));
				else if (_currXSpeed == 0)
					TransitionToState(Enum.Parse<State>($"Idle_{Weapon}"));
				else TransitionToState(Enum.Parse<State>($"Walk_{Weapon}"));
			}
			else if (IsIdleState)
			{
				if (_currXSpeed != 0)
					TransitionToState(Enum.Parse<State>($"Walk_{Weapon}"));
			}
			else if (IsWalkState)
			{
				if (Mathf.Abs(_currXSpeed) > walkSpeed + .1f)
					TransitionToState(Enum.Parse<State>($"Run_{Weapon}"));
				else if (_currXSpeed == 0)
					TransitionToState(Enum.Parse<State>($"Idle_{Weapon}"));
			}
			else if (IsRunState)
			{
				if (Mathf.Abs(_currXSpeed) < walkSpeed + 0.1f)
					TransitionToState(Enum.Parse<State>($"Walk_{Weapon}"));
			}
			else if (IsDashState)
			{
				if (!isDashing)
				{
					if (Mathf.Abs(_currXSpeed) > walkSpeed + 0.1f)
						TransitionToState(Enum.Parse<State>($"Run_{Weapon}"));
					else if (_currXSpeed == 0)
						TransitionToState(Enum.Parse<State>($"Idle_{Weapon}"));
					else TransitionToState(Enum.Parse<State>($"Walk_{Weapon}"));
				}
			}
			else if (IsJumpState)
			{
				if (IsGrounded && Mathf.Abs(rb2D.velocity.y) < .1f)
				{
					if (Mathf.Abs(_currXSpeed) > walkSpeed + 0.1f)
						TransitionToState(Enum.Parse<State>($"Run_{Weapon}"));
					else if (_currXSpeed == 0)
						TransitionToState(Enum.Parse<State>($"Idle_{Weapon}"));
					else TransitionToState(Enum.Parse<State>($"Walk_{Weapon}"));
				}
			}
		}
		#endregion


		#region Event Handlers

		public void OnJump(InputAction.CallbackContext context)
		{
			if (IsFrozen) return;
			if (IsAttacking || _isTakingDamage || _isDead) return;
			if (context.started)
			{
				if (IsGrounded || _jumpBufferCountdown > 0) Jump();
				else _preLandJumpBufferCountdown = preLandJumpBuffer;
			}
			else if (context.canceled) // On button released
			{
				// Stop jump and reset jumpTime
				IsJumping = false;
				_jumpTimeLeft = 0f;
			}
		}

		public void OnRun(InputAction.CallbackContext context)
		{
			if (IsFrozen) return;
			if (context.started)
				_shouldRun = true;
			else if (context.canceled)
				_shouldRun = false;
		}

		public void OnMove(InputAction.CallbackContext context)
		{
			if (IsFrozen) return;
			_moveDir = context.ReadValue<float>();
		}

		public void OnDash(InputAction.CallbackContext context)
		{
			if (IsFrozen) return;
			if (IsAttacking || _isTakingDamage || _isDead) return;
			if (context.started && dashAvail)
			{
				_playerSfx.PlayDash();
				isDashing = true;
				_dashTimeLeft = dashTime;
				dashAvail = false;
				groundedSinceDash = false;
				rb2D.AddForce((IsFacingLeft ? Vector2.left : Vector2.right) * dashImpulse, ForceMode2D.Impulse);
				TransitionToState(Enum.Parse<State>($"Dash_{Weapon}"));
			}
		}

		public void OnAttack(InputAction.CallbackContext context)
		{
			if (IsFrozen) return;
			if (isDashing || _isTakingDamage || _isDead) return;
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
			if (IsFrozen) return;
			if (context.started)
				OnAttemptInteract?.Invoke();
		}

		public void OnSpell1(InputAction.CallbackContext context)
		{
			if (IsFrozen) return;
			if (context.started)
				CastSpell(0);
		}

		public void OnSpell2(InputAction.CallbackContext context)
		{
			if (IsFrozen) return;
			if (context.started)
				CastSpell(1);
		}

		public void OnSpell3(InputAction.CallbackContext context)
		{
			if (IsFrozen) return;
			if (context.started)
				CastSpell(2);
		}

		public bool TakeHit(bool hasKnockback, Vector3 from, float kbImpulse)
		{
			if (_isTakingDamage || _isDead) return true;
			IsAttacking = false;
			_playerSfx.PlayHurt();
			if (isDashing)
			{
				// NOTE: compensate for dash -> damage then in Update method isTakingDamage check preserves dashing momentum
				isDashing = false;
				_dashTimeLeft = 0f;
				if (hasKnockback) rb2D.AddForce((rb2D.velocity.x > 0 ? Vector2.left : Vector2.right) * (knockbackImpulse + kbImpulse), ForceMode2D.Impulse);
			}
			IsJumping = false;
			_jumpTimeLeft = 0f;
			_isTakingDamage = true;
			if (hasKnockback) rb2D.AddForce(new Vector3(transform.position.x - from.x, 0, 0).normalized * (knockbackImpulse + kbImpulse), ForceMode2D.Impulse);
			TransitionToState(Enum.Parse<State>($"Damage_{Weapon}"));
			return false;
		}

		public void Die()
		{
			IsAttacking = false;
			isDashing = false;
			_dashTimeLeft = 0f;
			IsJumping = false;
			_jumpTimeLeft = 0f;
			_isTakingDamage = false;
			_isDead = true;
			TransitionToState(Enum.Parse<State>($"Death_{Weapon}"));
		}
		#endregion

		#region Animation Event Handlers
		void DamageEnd() => _isTakingDamage = false;

		void AttackEnd()
		{
			IsAttacking = false;
			OnAttackEnded?.Invoke();
		}

		void DeathEnd() => EventManager.InvokeEvent(PlayEvents.PlayerDeath);
		#endregion

		#region Helper Methods
		bool IsFrozen => GameManager.Instance.UI.IsOpen || _isResting;

		void Jump()
		{
			_playerSfx.PlayJump();
			rb2D.AddForce(Vector2.up * initialJumpImpulse, ForceMode2D.Impulse);
			_jumpTimeLeft = MAX_JUMP_TIME;
			IsJumping = true;
			TransitionToState(Enum.Parse<State>($"Jump_{Weapon}"));
		}

		void FlipSprite()
		{
			Vector3 currScale = gameObject.transform.localScale;
			currScale.x *= -1;
			gameObject.transform.localScale = currScale;

			IsFacingLeft = !IsFacingLeft;
			EventManager.InvokeEvent(PlayEvents.PlayerSpeakFlip);
		}

		void TransitionToState(State newState)
		{
			if (currState == newState) return;
			currState = newState;
			PlayAnimation(newState.ToString());
		}

		void PlayAnimation(string animToPlay)
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

		void CastSpell(int ind)
		{
			SpellItem[] spellItems = GameManager.Instance.PlayerPersistence.SpellItems;
			if (spellItems[ind] == null || !spellItems[ind].CanCast) return;

			var spellObj = Instantiate(spellItems[ind].itemPrefab, transform.position, Quaternion.identity);
			spellObj.GetComponent<Spell>().Cast(IsFacingLeft);
		}
		#endregion
	}
}
