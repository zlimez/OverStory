using System;
using Abyss.EventSystem;
using Abyss.Player.Spells;
using AnyPortrait;
using Utils.Tuples;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.VFX;

// FIXME: When damaged seems to charge further
namespace Abyss.Player
{
	public class PlayerController : MonoBehaviour, ActionInputs.IPlayerActions
	{
		#region Fields

		private enum State // For reference only
		{
			Idle_Nil, Walk_Nil, Run_Nil, Jump_Nil, Dash_Nil, Damage_Nil, Attack_Nil, Death_Nil,
			Idle_Axe_t1, Walk_Axe_t1, Run_Axe_t1, Jump_Axe_t1, Dash_Axe_t1, Damage_Axe_t1, Attack_Axe_t1, Death_Axe_t1,
		}

		public Transform Foot;
		public ArmController armController;
		[SerializeField] float groundCheckDist = 1f;
		[SerializeField] Vector2 groundCheckSize = new(1f, 1f);

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
		[SerializeField] float timeToApex, maxJumpHeight = 5;
		[SerializeField] float downGravityMult = 2f, upGravityMult = 1f, jumpCutoffMult, defGravMult = 1f, swingGravMult = 2f, slantedRepelGravMult = 1f;
		[SerializeField] float maxFallVelocity = 15f;
		[SerializeField][Tooltip("Extra time window given to player to jump the moment they leave ground i.e. leave a platform) ")] float jumpBuffer = 0.1f;
		[SerializeField][Tooltip("If player becomes grounded with this window after a jump command, the jump will take effect")] float preLandJumpBuffer = 0.1f;
		float _jmpBuffCd = 0f, _preLandJmpBuffCd = 0f, _gravMult = 1;
		bool _willJmp = false, _willTakeHit = false, _pressingJmp = false;
		// Hit params
		bool _hasKb; Vector2 _hitFrm; float _kbImp;
		public bool PressingRet { get; private set; } = false;
		public bool PressingLen { get; private set; } = false;
		public bool PressingAim { get; private set; } = false;
		public bool IsJumping, IsDashing = false, IsSwinging = false; // Distinct from Swinging in armController, IsSwinging is true for the duration player is in air starting from leaving ground and start swinging motion

		[Header("Dash")]
		[SerializeField] float dashSpeed = 25f;
		[SerializeField] float dashTime = 0.3f;
		bool _dashAvail = true, _isDashLeft, _isGrounded;
		float _dashTimeLeft;

		// Attacking
		[Header("Damage")]
		[SerializeField] float knockbackImpulse = 1000f;
		[SerializeField][Tooltip("Ember spell Fire Column Prefab")] GameObject fireColumn;
		[SerializeField] float postDmgInvulnTime = 0.5f;
		float _invulnTimeLeft = 0f;

		[Header("Weapon")]
		[SerializeField][Tooltip("Should match animation name suffix in anyportrait")] Pair<WeaponItem, string>[] weaponMapping;
		[SerializeField] VisualEffect weaponSlash;
		[SerializeField][Tooltip("Used by slash VFX")] string attackEvent = "Attack", xDirParam = "xDir", sizeParam = "size";
		[SerializeField][Tooltip("Conversion between weapon radius and slash vfx size")] float slashSizeConversion = 8f / 1.75f;
		float _slashSize;

		[SerializeField][Tooltip("Affects magnitude of wiggle during arm swing state")] float swingAngDec = 4f, wiggleForce = 10f;

		public bool IsAttacking { get; private set; } = false;
		bool _isTakingDamage = false, _isDead = false, _isResting = false, _isInVuln = false;
		public Action OnAttackEnded, OnAttemptInteract, OnGrounded;

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
			if (IsDashing)
			{
				if (_dashTimeLeft > 0f) _dashTimeLeft -= Time.deltaTime;
				else
				{
					IsDashing = false;
					_dashAvail = _isGrounded;
				}
			}

			if (_jmpBuffCd > 0) _jmpBuffCd = Mathf.Max(0, _jmpBuffCd - Time.deltaTime);
			if (_preLandJmpBuffCd > 0) _preLandJmpBuffCd = Mathf.Max(0, _preLandJmpBuffCd - Time.deltaTime);

			if (_invulnTimeLeft > 0) _invulnTimeLeft = Mathf.Max(0, _invulnTimeLeft - Time.deltaTime);
			else _isInVuln = false;

			if (!IsAttacking && !_isTakingDamage && !_isDead) HandleState();
			if (!IsSwinging && !armController.Repeling && !_isTakingDamage && ((_moveDir > 0 && IsFacingLeft) || (_moveDir < 0 && !IsFacingLeft))) FlipSprite();
		}

		void FixedUpdate()
		{
			SetBodyGrav();
			_isGrounded = IsGrounded();

			if (_isGrounded && OnGrounded != null)
			{
				OnGrounded.Invoke();
				OnGrounded = null;
			}

			if (!IsSwinging && armController.Swinging && !_isGrounded) IsSwinging = true;
			else if (IsSwinging && _isGrounded) IsSwinging = false; // Only when landed

			if (_willTakeHit)
			{
				TakeHit();
				return;
			}

			if (IsSwinging)
			{
				if (armController.Swinging)
				{
					float ang = Mathf.Clamp(Mathf.Acos(Vector2.Dot(Vector2.down, ((Vector2)transform.position - armController.Anchor).normalized)) * swingAngDec, 0, Mathf.PI / 2);
					rb2D.AddForce(_moveDir * Mathf.Cos(ang) * wiggleForce * Vector2.right);
				}
				else rb2D.velocity = new(rb2D.velocity.x, Mathf.Clamp(rb2D.velocity.y, -maxFallVelocity, 100));
			}
			else if (!_isTakingDamage && !armController.Repeling)
			{
				if (!IsDashing)
				{
					_currXSpeed = (_isDead || IsAttacking) ? 0 : _moveDir * (_shouldRun ? runSpeed : walkSpeed);
					Vector2 newVel = new(_currXSpeed, Mathf.Clamp(rb2D.velocity.y, -maxFallVelocity, 100));
					// if (!(!armController.Swinging || (armController.Swinging && !PressingRet && armController.IsSoft && (armController.PlayerFree || Vector2.Dot(armController.AnchorDir, newVel) > 0)))) Debug.Log("Disabling move");
					if (armController.Swinging && PressingRet)
					{
						transform.position += 0.01f * Vector3.up;
						transform.position -= 0.01f * Vector3.up;
					}
					if (!armController.Swinging || (armController.Swinging && !PressingRet && armController.IsSoft && (armController.PlayerFree || Vector2.Dot(armController.AnchorDir, newVel) > 0)))
						rb2D.velocity = newVel;
				}
				else rb2D.velocity = (_isDashLeft ? -1 : 1) * dashSpeed * Vector2.right;
			}

			if (_willJmp)
			{
				Jump();
				return;
			}

			CalcGravity();
		}

		void OnTriggerEnter2D(Collider2D coll2D)
		{
			if (coll2D.gameObject.layer == (int)Settings.Layer.Ground || coll2D.gameObject.layer == (int)Settings.Layer.Buildup)
			{
				// Imm sets grav mult to def to prevent the case whr setbodygrav runs before calcgrav, willJmp uses downGrav > defGrav to calc init jump vel
				_gravMult = defGravMult;
				_dashAvail |= !IsDashing;
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
			if (coll2D.gameObject.layer == (int)Settings.Layer.Ground || coll2D.gameObject.layer == (int)Settings.Layer.Buildup)
				if (!IsJumping) _jmpBuffCd = jumpBuffer;
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
				if (!_isGrounded)
					TransitionToState(Enum.Parse<State>($"Jump_{Weapon}"));
				else if (Mathf.Abs(_currXSpeed) > walkSpeed + 0.1f)
					TransitionToState(Enum.Parse<State>($"Run_{Weapon}"));
				else if (_currXSpeed == 0)
					TransitionToState(Enum.Parse<State>($"Idle_{Weapon}"));
				else TransitionToState(Enum.Parse<State>($"Walk_{Weapon}"));
			}
			else if (IsDamageState)
			{
				if (!_isGrounded)
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
				if (!IsDashing)
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
				if (_isGrounded && Mathf.Abs(rb2D.velocity.y) < .1f)
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
			if (!CanJump) return;

			if (context.performed)
			{
				if (IsGrounded() || _jmpBuffCd > 0) _willJmp = true;
				else _preLandJmpBuffCd = preLandJumpBuffer;
			}
			else if (context.canceled) _pressingJmp = false;
		}

		public void OnRun(InputAction.CallbackContext context)
		{
			if (IsFrozen) return;

			if (context.performed) _shouldRun = true;
			else if (context.canceled) _shouldRun = false;
		}

		public void OnMove(InputAction.CallbackContext context)
		{
			if (IsFrozen) return;
			_moveDir = context.ReadValue<float>();
		}

		public void OnDash(InputAction.CallbackContext context)
		{
			if (CanDash && context.performed)
			{
				_playerSfx.PlayDash();
				IsDashing = true;
				_isDashLeft = IsFacingLeft;
				_dashTimeLeft = dashTime;
				_dashAvail = false;
				TransitionToState(Enum.Parse<State>($"Dash_{Weapon}"));
			}
		}

		public void OnAttack(InputAction.CallbackContext context)
		{
			if (context.performed)
			{
				IsAttacking = true;
				currState = Enum.Parse<State>($"Attack_{Weapon}");
				portrait.Play($"Attack_{Weapon}"); // NOTE: CrossFade is glitchy here
				weaponSlash.SetInt(xDirParam, IsFacingLeft ? -1 : 1);
				weaponSlash.SetFloat(sizeParam, _slashSize);
				weaponSlash.SendEvent(attackEvent);
			}
		}

		public void OnAim(InputAction.CallbackContext context)
		{
			if (context.performed) PressingAim = true;
			else if (context.canceled) PressingAim = false;
		}

		public void OnExtRel(InputAction.CallbackContext context) { if (context.performed) armController.QueueEvent(ArmController.Trigger.ExtRel); }
		public void OnDisc(InputAction.CallbackContext context) { if (context.performed) armController.QueueEvent(ArmController.Trigger.Disc); }
		public void OnHardSoft(InputAction.CallbackContext context) { if (context.performed) armController.QueueEvent(ArmController.Trigger.HardSoft); }

		public void OnInteractRet(InputAction.CallbackContext context)
		{
			if (context.performed)
			{
				if (CanInteract && context.interaction is TapInteraction) OnAttemptInteract?.Invoke();
				else if (context.interaction is PressInteraction) PressingRet = true;
			}
			else if (context.canceled) PressingRet = false;
		}

		public void OnLengthen(InputAction.CallbackContext context)
		{
			if (context.performed) PressingLen = true;
			else if (context.canceled) PressingLen = false;
		}

		public void OnSpell1(InputAction.CallbackContext context)
		{
			if (!CanCastSpell) return;
			if (context.performed) CastSpell(0);
		}

		public void OnSpell2(InputAction.CallbackContext context)
		{
			if (!CanCastSpell) return;
			if (context.performed) CastSpell(1);
		}

		public void OnSpell3(InputAction.CallbackContext context)
		{
			if (!CanCastSpell) return;
			if (context.performed) CastSpell(2);
		}

		public bool OnHit(bool hasKb, Vector2 from, float kbImpulse)
		{
			if (_isTakingDamage || _isDead || _isInVuln) return true;
			_willTakeHit = true;
			_hasKb = hasKb; _hitFrm = from; _kbImp = kbImpulse;
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
		void DamageEnd()
		{
			_isTakingDamage = false;
			_isInVuln = true;
			_invulnTimeLeft = postDmgInvulnTime;
		}

		void AttackEnd()
		{
			IsAttacking = false;
			OnAttackEnded?.Invoke();
		}

		void DeathEnd() => EventManager.InvokeEvent(PlayEvents.PlayerDeath);
		#endregion

		#region Helper Methods
		bool IsFrozen => (GameManager.Instance != null && GameManager.Instance.UI.IsOpen) || _isResting;
		bool CanDash => !IsFrozen && !IsAttacking && !_isTakingDamage && !_isDead && _dashAvail && !armController.ArmActive;
		bool CanAttack => !IsFrozen && !IsAttacking && !IsDashing && !_isTakingDamage && !_isDead && !armController.ArmActive;
		bool CanCastSpell => !IsFrozen && !_isTakingDamage && !_isDead && !IsDashing && !armController.ArmActive;
		bool CanInteract => !IsFrozen && !IsAttacking && !_isTakingDamage && !_isDead && !IsDashing && !armController.ArmActive;
		bool CanJump => !IsFrozen && !IsAttacking && !_isTakingDamage && !_isDead && !IsDashing;

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
			Debug.Log("Dash interrupted");
			IsDashing = false;
			_dashTimeLeft = 0f;
			_dashAvail = IsGrounded();
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

		void TakeHit()
		{
			_playerSfx.PlayHurt();
			IsAttacking = false;
			_pressingJmp = false;
			_isTakingDamage = true;
			if (_hasKb) rb2D.AddForce(new Vector2(transform.position.x - _hitFrm.x, 0).normalized * (knockbackImpulse + _kbImp) - rb2D.velocity * rb2D.mass, ForceMode2D.Impulse);
			else rb2D.velocity = new Vector2(0, rb2D.velocity.y);
			TransitionToState(Enum.Parse<State>($"Damage_{Weapon}"));
		}

		void CalcGravity()
		{
			if (_isGrounded || IsSwinging || armController.Repeling)
			{
				_gravMult = IsSwinging ? swingGravMult : armController.Repeling ? armController.VertRepeling ? 0 : slantedRepelGravMult : defGravMult;
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
			EventManager.InvokeEvent(PlayEvents.PlayerSpriteFlip);
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
				Debug.LogWarning($"Error playing animation {animToPlay}. The portrait is likely not initialized");
			}
		}

		void CastSpell(int ind)
		{
			SpellItem[] spellItems = GameManager.Instance.PlayerPersistence.SpellItems;
			if (spellItems[ind] == null || !spellItems[ind].CanCast) return;

			var spellObj = Instantiate(spellItems[ind].itemPrefab, transform.position, Quaternion.identity);
			spellObj.GetComponent<Spell>().Cast(IsFacingLeft);
		}

		bool IsGrounded()
		{
			// BoxCast version (more reliable for platforms)
			RaycastHit2D boxHit = Physics2D.BoxCast(
				transform.position,
				groundCheckSize,
				0f,
				Vector2.down,
				groundCheckDist,
				Settings.LayerMask.GROUND_LMASK
			);

			return boxHit.collider != null;
		}

#if UNITY_EDITOR
		void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(
				transform.position + Vector3.down * groundCheckDist,
				groundCheckSize
			);
		}
#endif
		#endregion
	}
}
