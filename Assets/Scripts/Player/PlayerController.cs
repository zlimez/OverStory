using System;
using Abyss.EventSystem;
using Abyss.Player.Spells;
using AnyPortrait;
using Utils;
using Utils.Tuples;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.VFX;
using UnityEngine.Rendering;
using VerletPhysics;
using System.Collections.Generic;

// FIXME: When damaged seems to charge further
// TODO: Combine arm and player controller most importantly their states?
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
		public ArmController MechArm;
		[SerializeField] float groundCheckDist = 1f;
		[SerializeField] Vector2 groundCheckSize = new(1f, 1f);

		[Header("Animation")]
		[SerializeField] apPortrait portrait;
		[SerializeField] float crossFadeSeconds = .01f;

		[Header("Movement")]
		[SerializeField] float walkSpeed = 4f;
		[SerializeField] float runSpeed = 8f;
		bool _shouldRun;

		float _moveDir, _currXSpeed;
		public bool IsFacingLeft { get; private set; } = false;

		[Header("Jump")]
		[SerializeField] float timeToApex, maxJumpHeight = 5;
		[SerializeField] float downGravityMult = 2f, upGravityMult = 1f, jumpCutoffMult, defGravMult = 1f, slantRepelGravAcc = 5f;
		[SerializeField] float maxFallVelocity = 15f;
		[SerializeField][Tooltip("Extra time window given to player to jump the moment they leave ground i.e. leave a platform) ")] float jumpBuffer = 0.1f;
		[SerializeField][Tooltip("If player becomes grounded with this window after a jump command, the jump will take effect")] float preLandJumpBuffer = 0.1f;

		float _jmpBuffCd = 0f, _preLandJmpBuffCd = 0f, _gravMult = 1;
		bool _willJmp = false, _willTakeHit = false, _pressingJmp = false;
		bool _hasKb; Vector2 _hitFrm; float _kbImp;

		[Header("Dash")]
		[SerializeField] float dashSpeed = 25f;
		[SerializeField] float dashTime = 0.3f;
		bool _dashAvail = true, _isLeftDash;
		float _dashTTL;

		[Header("Damage")]
		[SerializeField] float knockbackImpulse = 1000f;
		[SerializeField][Tooltip("Ember spell Fire Column Prefab")] GameObject fireColumn;
		[SerializeField] float postDmgInvulnTime = 0.5f;
		float _invulnTTL = 0f;

		[Header("Weapon")]
		[SerializeField][Tooltip("Should match animation name suffix in anyportrait")] Pair<WeaponItem, string>[] weaponMapping;
		[SerializeField] VisualEffect weaponSlash;
		[SerializeField][Tooltip("Used by slash VFX")] string attackEvent = "Attack", xDirParam = "xDir", sizeParam = "size";
		[SerializeField][Tooltip("Conversion between weapon radius and slash vfx size")] float slashSizeConversion = 8f / 1.75f;
		float _slashSize;

		[Header("Swing")]
		[SerializeField][Tooltip("Higher the value greater the penalty multiplier on wiggle force")] float swingAngDec = 4f;
		[SerializeField][Tooltip("Magnitude of wiggle when swinging")] float wiggleForce = 10f;
		[SerializeField][Tooltip("When checking whether to enter swing state player's vel sqr must be leq this threshold for swinging to take place")] float maxRemVelSqMag = 2f;
		public Vector2 ShoulderOffset;
		[Header("Repel")]
		[SerializeField][Tooltip("Higher the value less the y-velocity carries over when arm first strikes obstacle")] ClampedFloatParameter repYDamper = new(2, 1, 10);
		[NonSerialized] public bool RepStart = false;
		float _perpVel; // Used to calc vel perp to repel dir (arm ext)

		public bool PressingRet { get; private set; } = false;
		public bool PressingLen { get; private set; } = false;
		public bool PressingAim { get; private set; } = false;

		[NonSerialized] public bool IsJumping, IsDashing = false, IsSwinging = false;
		public bool IsAttacking { get; private set; } = false;
		bool _isTakingDamage = false, _isDead = false, _isResting = false, _isInVuln = false, _isGrounded;
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
			MechArm.PlayerCtr = this;

			portrait.Initialize();

			MechArm.Init();
			Collider2D[] cols = FindObjectsOfType<Collider2D>();
			List<Collider2D> armCols = new();
			foreach (Collider2D col in cols)
				if ((1 << col.gameObject.layer & Settings.LayerMask.OBSTACLE_LMASK) > 0) armCols.Add(col);
			MechArm.Arm.SetColliders(armCols);
			ShoulderOffset = MechArm.Arm.End.position - transform.position;

			currState = Enum.Parse<State>($"Idle_{Weapon}");
		}

		void Update()
		{
			if (IsFrozen) _moveDir = 0;
			if (IsDashing)
			{
				if (_dashTTL > 0f) _dashTTL -= Time.deltaTime;
				else
				{
					IsDashing = false;
					_dashAvail = _isGrounded;
				}
			}

			if (_jmpBuffCd > 0) _jmpBuffCd = Mathf.Max(0, _jmpBuffCd - Time.deltaTime);
			if (_preLandJmpBuffCd > 0) _preLandJmpBuffCd = Mathf.Max(0, _preLandJmpBuffCd - Time.deltaTime);

			if (_invulnTTL > 0) _invulnTTL = Mathf.Max(0, _invulnTTL - Time.deltaTime);
			else _isInVuln = false;

			if (!IsAttacking && !_isTakingDamage && !_isDead) HandleAnim();
			if (!IsSwinging && !MechArm.Repeling && !_isTakingDamage && ((_moveDir > 0 && IsFacingLeft) || (_moveDir < 0 && !IsFacingLeft))) FlipSprite();
		}

		void LateUpdate()
		{
			// MechArm.CompleteTick();
			// if (IsSwinging)
			// {
			// 	// float ang = Mathf.Clamp(Mathf.Acos(Vector2.Dot(Vector2.down, ((Vector2)transform.position - MechArm.Anchor).normalized)) * swingAngDec, 0, Mathf.PI / 2);
			// 	if (Mathf.Abs(_moveDir) > Const.EPS)
			// 	{
			// 		Vector2 endForce = _moveDir * wiggleForce * Vector2.right;
			// 		for (int i = 1; i <= MechArm.Arm.NumPoints; i++)
			// 			MechArm.Arm.QueueForce(endForce * Mathf.Pow((float)i / MechArm.Arm.NumPoints, 2), i - 1);
			// 		// Vector2 endForce = _moveDir * wiggleForce * Vector2.right;
			// 		// MechArm.Arm.QueueForce(endForce, MechArm.Arm.NumPoints-1);
			// 	}
			// }
			// if (IsSwinging) transform.position = MechArm.Arm.End.position - (Vector3)ShoulderOffset;
		}

		void FixedUpdate()
		{
			MechArm.PartTick(Time.fixedDeltaTime);

			SetBodyGrav();
			_isGrounded = IsGrounded();
			if (_isGrounded) OnGrounded?.Invoke();
			// might be worth putting jump into state machine consideration
			// TODO: Freeze movement even when grounded but rope at max stretch
			if (!IsSwinging && MechArm.Swingable && !_isGrounded && (!IsJumping || (IsJumping && rb2D.velocity.y < -0.01f)))
			{
				Debug.Log($"Start swinging anchor sqdist {MechArm.AnchorDir.sqrMagnitude} rope sqlen {MechArm.Arm.Len * MechArm.Arm.Len} grounded {_isGrounded}");
				IsSwinging = true;
				rb2D.isKinematic = true;
				rb2D.velocity = Vector2.zero;
				MechArm.Arm.QueuePin(Rope.PinPoint.Start);
			}
			else if (IsSwinging && (_isGrounded || !MechArm.Swingable))
			{
				Debug.Log($"Stop swinging anchor sqdist {MechArm.AnchorDir.sqrMagnitude} rope sqlen {MechArm.Arm.Len * MechArm.Arm.Len} grounded {_isGrounded}");
				IsSwinging = false; // Only when landed
				rb2D.isKinematic = false;
				if (!_isGrounded) rb2D.velocity = MechArm.Arm.EndVel / Time.fixedDeltaTime; // Release preserve momentum
				else if (MechArm.Swingable) MechArm.Arm.QueuePin(Rope.PinPoint.Both); // rope len greater than dist to anchor
			}

			if (_willTakeHit)
			{
				TakeHit();
				return;
			}


			if (MechArm.Repeling)
			{
				if (MechArm.CurrState == ArmController.State.Ha_CoEx_To)
				{
					if (MechArm.VertRepeling) rb2D.MovePosition(rb2D.position - MechArm.FixedRetractRate * Time.fixedDeltaTime * MechArm.Taim);
					else
					{
						Vector2 nad = MechArm.AnchorDir.normalized, perp = Vector2.Perpendicular(nad);
						if (perp.y > 0) perp = -perp;
						if (RepStart)
						{
							_perpVel = Vector2.Dot(Vector2.up * rb2D.velocity.y, perp) / repYDamper.value;
							RepStart = false;
						}
						else _perpVel += Vector2.Dot(-slantRepelGravAcc * Time.fixedDeltaTime * Vector2.up, perp);
						CheckRepelArmCol(perp);
						Vector2 nv = _perpVel * perp - nad * MechArm.FixedRetractRate;
						rb2D.velocity = new Vector2(nv.x, Mathf.Clamp(nv.y, -maxFallVelocity, maxFallVelocity));
					}
				}
				else
				{
					if (MechArm.VertRepeling)
					{
						if (PressingRet) rb2D.MovePosition(rb2D.position + MechArm.FixedRetractRate * Time.fixedDeltaTime * MechArm.Taim);
						else rb2D.velocity = Vector2.zero;
					}
					else
					{
						Vector2 nad = MechArm.AnchorDir.normalized, perp = Vector2.Perpendicular(nad);
						if (perp.y > 0) perp = -perp;
						_perpVel += Vector2.Dot(-slantRepelGravAcc * Time.fixedDeltaTime * Vector2.up, perp);
						CheckRepelArmCol(perp);
						Vector2 nv = _perpVel * perp;
						if (PressingRet) nv += nad * MechArm.FixedRetractRate;
						rb2D.velocity = new Vector2(nv.x, Mathf.Clamp(nv.y, -maxFallVelocity, maxFallVelocity));
					}
				}
			}
			else if (MechArm.HaExtended)
			{
				Rope arm = MechArm.Arm;
				Vector2 dir = arm.Start.position - arm.End.position;
				// TODO: Update fist position
				_currXSpeed = (_isDead || IsAttacking) ? 0 : _moveDir * (_shouldRun ? runSpeed : walkSpeed);
				Vector2 newVel = new(_currXSpeed, Mathf.Clamp(rb2D.velocity.y, -maxFallVelocity, maxFallVelocity));
				var hit = Physics2D.BoxCast((Vector2)arm.End.position + newVel * Time.fixedDeltaTime, new(0.5f, arm.Width), Vector2.SignedAngle(Vector2.right, dir), dir.normalized, dir.magnitude, Settings.LayerMask.OBSTACLE_LMASK);
				if (hit && Vector2.Dot(hit.normal, newVel) < 0)
				{
					Debug.Log($"Hit dot prod {Vector2.Dot(hit.normal, newVel)} {hit.normal} {newVel}");
					rb2D.velocity = Vector2.zero;
				}
				else rb2D.velocity = newVel;
			}
			else if (!_isTakingDamage)
			{
				if (IsDashing) rb2D.velocity = (_isLeftDash ? -1 : 1) * dashSpeed * Vector2.right;
				else
				{
					_currXSpeed = (_isDead || IsAttacking) ? 0 : _moveDir * (_shouldRun ? runSpeed : walkSpeed);
					Vector2 newVel = new(_currXSpeed, Mathf.Clamp(rb2D.velocity.y, -maxFallVelocity, maxFallVelocity));
					rb2D.velocity = newVel;
				}
			}

			MechArm.CompleteTick();
			if (IsSwinging)
			{
				// float ang = Mathf.Clamp(Mathf.Acos(Vector2.Dot(Vector2.down, ((Vector2)transform.position - MechArm.Anchor).normalized)) * swingAngDec, 0, Mathf.PI / 2);
				if (Mathf.Abs(_moveDir) > Const.EPS)
				{
					Vector2 endForce = _moveDir * wiggleForce * Vector2.right;
					for (int i = 1; i <= MechArm.Arm.NumPoints; i++)
						MechArm.Arm.QueueForce(endForce * Mathf.Pow((float)i / MechArm.Arm.NumPoints, 2), i - 1);
				}
				return;
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
				// Imm sets grav mult to def to prevent setbodygrav running before calcgrav, willJmp uses downGrav>defGrav to calc init jump vel
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

		void OnDestroy() => MechArm.Arm.Dispose();
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

		// Animation states
		void HandleAnim()
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
				_isLeftDash = IsFacingLeft;
				_dashTTL = dashTime;
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

		public void OnExtRel(InputAction.CallbackContext context) { if (context.performed) MechArm.QueueEvent(ArmController.Trigger.ExtRel); }
		public void OnDisc(InputAction.CallbackContext context) { if (context.performed) MechArm.QueueEvent(ArmController.Trigger.Disc); }
		public void OnHardSoft(InputAction.CallbackContext context) { if (context.performed) MechArm.QueueEvent(ArmController.Trigger.HardSoft); }

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
			_invulnTTL = postDmgInvulnTime;
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
		bool CanDash => !IsFrozen && !IsAttacking && !_isTakingDamage && !_isDead && _dashAvail && !MechArm.ArmActive;
		bool CanAttack => !IsFrozen && !IsAttacking && !IsDashing && !_isTakingDamage && !_isDead && !MechArm.ArmActive;
		bool CanCastSpell => !IsFrozen && !_isTakingDamage && !_isDead && !IsDashing && !MechArm.ArmActive;
		bool CanInteract => !IsFrozen && !IsAttacking && !_isTakingDamage && !_isDead && !IsDashing && !MechArm.ArmActive;
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
			_dashTTL = 0f;
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
			if (_isGrounded || IsSwinging || MechArm.Repeling)
			{
				_gravMult = MechArm.Repeling ? 0 : defGravMult;
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

		void CheckRepelArmCol(Vector2 perp)
		{
			Rope arm = MechArm.Arm;
			var dir = (Vector2)arm.Start.position + _perpVel * Time.fixedDeltaTime * perp - (Vector2)arm.End.position; // Sufficient approx rope anchor and rb is close
			if (Physics2D.BoxCast(arm.End.position, new(0.5f, arm.Width), Vector2.SignedAngle(Vector2.right, dir), dir.normalized, dir.magnitude, Settings.LayerMask.OBSTACLE_LMASK)) _perpVel = 0;
		}
#if UNITY_EDITOR
		void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(
				transform.position + Vector3.down * groundCheckDist,
				groundCheckSize
			);

			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(transform.position, transform.position + (Vector3)MechArm.Aim);
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, transform.position + (MechArm.Arm.End.position - MechArm.Arm.Start.position).normalized * MechArm.PlayerColDetDist);
		}
#endif
		#endregion
	}
}
