using System;
using System.Collections;
using Abyss.EventSystem;
using Abyss.Player.Spells;
using Abyss.SceneSystem;
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
		[SerializeField] float timeToApex, maxJumpHeight = 5;
		[SerializeField] float downGravityMult = 2f, upGravityMult = 1f, jumpCutoffMult, _gravMult;
		[SerializeField] float maxFallVelocity = 15f;
		[SerializeField][Tooltip("Extra time window given to player to jump the moment they leave ground i.e. leave a platform) ")] float jumpBuffer = 0.1f;
		[SerializeField][Tooltip("If player becomes grounded with this window after a jump command, the jump will take effect")] float preLandJumpBuffer = 0.1f;
		float _jmpBuffCd = 0f, _preLandJmpBuffCd = 0f;
		bool _willJmp = false, _pressingJmp = false;
		public bool IsGrounded, IsJumping;

		[Header("Dash")]
		[SerializeField] float dashSpeed = 25f;
		[SerializeField] float dashTime = 0.3f;
		bool _dashAvail = true, _isDashing = false, _isDashLeft;
		float _dashTimeLeft;

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

		void Awake()
		{
			rb2D = GetComponent<Rigidbody2D>();
			_playerSfx = GetComponent<PlayerSfx>();

			portrait.Initialize();
			currState = Enum.Parse<State>($"Idle_{Weapon}");
		}

		void Update()
		{
			if (IsFrozen) _moveDir = 0;

			if (_isDashing)
			{

				if (_dashTimeLeft > 0f)
					_dashTimeLeft -= Time.deltaTime;
				else
				{
					_isDashing = false;
					_dashAvail = IsGrounded;
				}
			}

			if (_jmpBuffCd > 0)
				_jmpBuffCd = Mathf.Max(0, _jmpBuffCd - Time.deltaTime);
			if (_preLandJmpBuffCd > 0)
				_preLandJmpBuffCd = Mathf.Max(0, _preLandJmpBuffCd - Time.deltaTime);

			if (!IsAttacking && !_isTakingDamage && !_isDead) HandleState();
			if (!_isTakingDamage)
			{
				if (_moveDir > 0 && IsFacingLeft)
					FlipSprite();
				else if (_moveDir < 0 && !IsFacingLeft)
					FlipSprite();
			}
		}

		void FixedUpdate()
		{
			SetBodyGrav();
			if (!_isDashing)
			{
				_currXSpeed = (_isDead || IsAttacking) ? 0 : _moveDir * (_shouldRun ? runSpeed : walkSpeed);
				if (!_isTakingDamage) rb2D.velocity = new(_currXSpeed, Mathf.Clamp(rb2D.velocity.y, -maxFallVelocity, 100));
			}
			else rb2D.velocity = (_isDashLeft ? -1 : 1) * dashSpeed * Vector2.right;

			if (_willJmp)
			{
				Jump();
				return;
			}

			CalcGravity();
		}

		void OnTriggerEnter2D(Collider2D coll2D)
		{
			// Check if player is on the ground
			if (coll2D.gameObject.layer == (int)AbyssSettings.Layers.Ground || coll2D.gameObject.layer == (int)AbyssSettings.Layers.Buildup)
			{
				IsGrounded = true;
				_dashAvail |= !_isDashing;
				IsJumping = false;
				if (_preLandJmpBuffCd > 0)
				{
					_willJmp = true;
					_preLandJmpBuffCd = 0;
				}
			}
		}

		void OnTriggerExit2D(Collider2D coll2D)
		{
			// Check if player is leaving the ground
			if (coll2D.gameObject.layer == (int)AbyssSettings.Layers.Ground || coll2D.gameObject.layer == (int)AbyssSettings.Layers.Buildup)
			{
				IsGrounded = false;
				if (!IsJumping)
					_jmpBuffCd = jumpBuffer;
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
				if (!_isDashing)
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
			if (!CanJumpReact) return;

			if (context.started)
			{
				if (IsGrounded || _jmpBuffCd > 0) _willJmp = true;
				else _preLandJmpBuffCd = preLandJumpBuffer;
			}
			else if (context.canceled) _pressingJmp = false;
		}

		public void OnRun(InputAction.CallbackContext context)
		{
			if (IsFrozen) return;

			if (context.started) _shouldRun = true;
			else if (context.canceled) _shouldRun = false;
		}

		public void OnMove(InputAction.CallbackContext context)
		{
			if (IsFrozen) return;
			_moveDir = context.ReadValue<float>();
		}

		public void OnDash(InputAction.CallbackContext context)
		{
			if (!CanDash) return;

			if (context.started)
			{
				_playerSfx.PlayDash();
				_isDashing = true;
				_isDashLeft = IsFacingLeft;
				_dashTimeLeft = dashTime;
				_dashAvail = false;
				TransitionToState(Enum.Parse<State>($"Dash_{Weapon}"));
			}
		}

		public void OnAttack(InputAction.CallbackContext context)
		{
			if (!CanAttack) return;

			if (context.started)
			{
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
			if (!CanInteract) return;
			if (context.started)
				OnAttemptInteract?.Invoke();
		}

		public void OnSpell1(InputAction.CallbackContext context)
		{
			if (!CanCastSpell) return;
			if (context.started) CastSpell(0);
		}

		public void OnSpell2(InputAction.CallbackContext context)
		{
			if (!CanCastSpell) return;
			if (context.started) CastSpell(1);
		}

		public void OnSpell3(InputAction.CallbackContext context)
		{
			if (!CanCastSpell) return;
			if (context.started) CastSpell(2);
		}

		public bool TakeHit(bool hasKnockback, Vector3 from, float kbImpulse)
		{
			if (_isTakingDamage || _isDead) return true;
			_playerSfx.PlayHurt();
			IsAttacking = false;
			InterruptDash();
			_pressingJmp = false;
			_isTakingDamage = true;
			if (hasKnockback) rb2D.AddForce(new Vector3(transform.position.x - from.x, 0, 0).normalized * (knockbackImpulse + kbImpulse), ForceMode2D.Impulse);
			TransitionToState(Enum.Parse<State>($"Damage_{Weapon}"));
			return false;
		}

		public void Die()
		{
			IsAttacking = false;
			InterruptDash();
			_pressingJmp = false;
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
		bool IsFrozen => (GameManager.Instance != null && GameManager.Instance.UI.IsOpen) || _isResting;

		bool CanDash => !IsFrozen && !IsAttacking && !_isTakingDamage && !_isDead && _dashAvail;
		bool CanAttack => !IsFrozen && !IsAttacking && !_isDashing && !_isTakingDamage && !_isDead;
		bool CanCastSpell => !IsFrozen && !_isTakingDamage && !_isDead && !_isDashing;
		bool CanInteract => !IsFrozen && !IsAttacking && !_isTakingDamage && !_isDead && !_isDashing;
		bool CanJumpReact => !IsFrozen && !IsAttacking && !_isTakingDamage && !_isDead && !_isDashing;

		bool IsIdleState => currState.ToString().StartsWith("Idle");
		bool IsWalkState => currState.ToString().StartsWith("Walk");
		bool IsRunState => currState.ToString().StartsWith("Run");
		bool IsJumpState => currState.ToString().StartsWith("Jump");
		bool IsDashState => currState.ToString().StartsWith("Dash");
		bool IsDamageState => currState.ToString().StartsWith("Damage");
		bool IsAttackState => currState.ToString().StartsWith("Attack");
		bool IsDeathState => currState.ToString().StartsWith("Death");
		string BaseState => currState.ToString().Split('_')[0];

		void InterruptDash()
		{
			_isDashing = false;
			_dashTimeLeft = 0f;
			_dashAvail = IsGrounded;
		}

		void Jump()
		{
			_playerSfx.PlayJump();
			_willJmp = false;
			IsJumping = true;
			_pressingJmp = true;
			var jumpVel = Mathf.Sqrt(-2f * Physics2D.gravity.y * rb2D.gravityScale * maxJumpHeight);
			rb2D.velocity = new Vector2(rb2D.velocity.x, jumpVel);
			TransitionToState(Enum.Parse<State>($"Jump_{Weapon}"));
		}

		void CalcGravity()
		{
			if (IsGrounded)
			{
				_gravMult = 1;
				return;
			}

			if (rb2D.velocity.y > 0.01f)
			{
				if (_pressingJmp && IsJumping) _gravMult = upGravityMult;
				else _gravMult = jumpCutoffMult;
			}
			else if (rb2D.velocity.y < -0.01f) _gravMult = downGravityMult;
		}

		void SetBodyGrav() => rb2D.gravityScale = -2 * maxJumpHeight / (timeToApex * timeToApex) / Physics2D.gravity.y * _gravMult;

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
