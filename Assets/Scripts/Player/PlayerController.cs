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
		[SerializeField] private ArmArgs armArguments;
		private ExtendableArm _mechArm;
		
		[SerializeField] private float groundCheckDist = 1f;
		[SerializeField] private Vector2 groundCheckSize = new(1f, 1f);

		[Header("Animation")]
		[SerializeField] private apPortrait portrait;
		[SerializeField] private float crossFadeSeconds = .01f;

		[Header("Movement")]
		[SerializeField] private float walkSpeed = 4f;
		[SerializeField] private float runSpeed = 8f;
		private bool _shouldRun;

		private float _moveDir, _currXSpeed;
		public bool IsFacingLeft { get; private set; }

		[Header("Jump")]
		[SerializeField] private float timeToApex;
		[SerializeField] private float maxJumpHeight = 5;
		[SerializeField] private float downGravityMult = 2f, upGravityMult = 1f, jumpCutoffMult, defGravMult = 1f, slantRepelGravAcc = 5f;
		[SerializeField] private float maxFallVelocity = 15f;
		[SerializeField][Tooltip("Extra time window given to player to jump the moment they leave ground i.e. leave a platform) ")]
		private float jumpBuffer = 0.1f;
		[SerializeField][Tooltip("If player becomes grounded with this window after a jump command, the jump will take effect")]
		private float preLandJumpBuffer = 0.1f;

		private float _jmpBuffCd, _preLandJmpBuffCd, _gravMult = 1;
		private bool _willJmp, _willTakeHit, _pressingJmp;
		private bool _hasKb;
		private Vector2 _hitFrm;
		private float _kbImp;

		[Header("Dash")]
		[SerializeField] private float dashSpeed = 25f;
		[SerializeField] private float dashTime = 0.3f;
		private bool _dashAvail = true, _isLeftDash;
		private float _dashTtl;

		[Header("Damage")]
		[SerializeField] private float knockbackImpulse = 1000f;
		[SerializeField][Tooltip("Ember spell Fire Column Prefab")] private GameObject fireColumn;
		[SerializeField] private float postDmgInvincibleTime = 0.5f;
		private float _invincibleTtl;

		[Header("Weapon")]
		[SerializeField][Tooltip("Should match animation name suffix in AnyPortrait")]
		private Pair<WeaponItem, string>[] weaponMapping;
		[SerializeField] private VisualEffect weaponSlash;
		[SerializeField][Tooltip("Used by slash VFX")] string attackEvent = "Attack", xDirParam = "xDir", sizeParam = "size";
		[SerializeField][Tooltip("Conversion between weapon radius and slash vfx size")]
		private float slashSizeConversion = 8f / 1.75f;
		private float _slashSize;

		[Header("Swing")]
		[SerializeField][Tooltip("Higher the value greater the penalty multiplier on wiggle force")] 
		private float swingAngDec = 4f;
		[SerializeField][Tooltip("Magnitude of wiggle when swinging")] 
		private float wiggleForce = 10f;
		[SerializeField][Tooltip("When checking whether to enter swing state player's vel sqr must be leq this threshold for swinging to take place")] 
		private float maxRemVelSqMag = 2f;

		[Header("Repel")]
		[SerializeField][Tooltip("Higher the value less the y-velocity carries over when arm first strikes obstacle")]
		private ClampedFloatParameter repYDamper = new(2, 1, 10);
		[NonSerialized] public bool RepStart = false;
		private float _perpVel; // Used to calc vel perp to repel dir (arm ext)

		public bool PressingRet { get; private set; }
		public bool PressingLen { get; private set; }
		public bool PressingAim { get; private set; }

		[NonSerialized] public bool IsJumping, IsDashing, IsSwinging;
		public bool IsAttacking { get; private set; }
		private bool _isTakingDamage, _isDead, _isResting, _isInVuln, _isGrounded;
		public Action OnAttackEnded, OnAttemptInteract, OnGrounded;

		private Rigidbody2D _rb2D;
		private State _currState;
		private PlayerSfx _playerSfx;
		[NonSerialized] private string _weapon = "Nil";
		#endregion

		#region Lifecyle Methods
		private void Awake()
		{
			_rb2D = GetComponent<Rigidbody2D>();
			_playerSfx = GetComponent<PlayerSfx>();
			portrait.Initialize();
			
			var cols = FindObjectsOfType<Collider2D>();
			List<Collider2D> armCols = new();
			foreach (var col in cols)
				if ((1 << col.gameObject.layer & Settings.LayerMask.OBSTACLE_LMASK) > 0) armCols.Add(col);
			_mechArm = new (armArguments);
			_mechArm.Rope.SetColliders(armCols);

			_currState = Enum.Parse<State>($"Idle_{_weapon}");
		}

		private void Update()
		{
			if (IsFrozen) _moveDir = 0;
			if (IsDashing)
			{
				if (_dashTtl > 0f) _dashTtl -= Time.deltaTime;
				else
				{
					IsDashing = false;
					_dashAvail = _isGrounded;
				}
			}

			if (_jmpBuffCd > 0) _jmpBuffCd = Mathf.Max(0, _jmpBuffCd - Time.deltaTime);
			if (_preLandJmpBuffCd > 0) _preLandJmpBuffCd = Mathf.Max(0, _preLandJmpBuffCd - Time.deltaTime);

			if (_invincibleTtl > 0) _invincibleTtl = Mathf.Max(0, _invincibleTtl - Time.deltaTime);
			else _isInVuln = false;

			if (!IsAttacking && !_isTakingDamage && !_isDead) HandleAnim();
			if (!IsSwinging && !_mechArm.Repelling && !_isTakingDamage && ((_moveDir > 0 && IsFacingLeft) || (_moveDir < 0 && !IsFacingLeft))) FlipSprite();
		}

		// TODO: Freeze movement even when grounded but rope at max stretch
		private void FixedUpdate()
		{
			_mechArm.PartTick(Time.fixedDeltaTime);

			SetBodyGrav();
			_isGrounded = IsGrounded();
			if (_isGrounded) OnGrounded?.Invoke();
			if (!IsSwinging && _mechArm.Swingable && !_isGrounded && (!IsJumping || (IsJumping && _rb2D.velocity.y < -0.01f)))
			{
#if UNITY_EDITOR
				Debug.Log($"Start swinging anchor sqDist {_mechArm.AnchorDir.sqrMagnitude} rope sqLen {_mechArm.Rope.Len * _mechArm.Rope.Len} grounded {_isGrounded}");
#endif
				IsSwinging = true;
				_rb2D.isKinematic = true;
				_rb2D.velocity = Vector2.zero;
				_mechArm.Rope.QueuePin(Rope.PinPoint.Start);
			}
			else if (IsSwinging && (_isGrounded || !_mechArm.Swingable))
			{
#if UNITY_EDITOR
				Debug.Log($"Stop swinging anchor sqDist {_mechArm.AnchorDir.sqrMagnitude} rope sqLen {_mechArm.Rope.Len * _mechArm.Rope.Len} grounded {_isGrounded}");
#endif
				IsSwinging = false; // Only when landed
				_rb2D.isKinematic = false;
				if (!_isGrounded) _rb2D.velocity = _mechArm.Rope.EndVel / Time.fixedDeltaTime; // Release preserve momentum
				else if (_mechArm.Swingable) _mechArm.Rope.QueuePin(Rope.PinPoint.Both); // rope len greater than dist to anchor
			}

			if (_willTakeHit)
			{
				TakeHit();
				return;
			}


			if (_mechArm.Repelling)
			{
				if (_mechArm.CurrState == ExtendableArm.State.Ha_CoEx_To)
				{
					if (_mechArm.VertRepelling) _rb2D.MovePosition(_rb2D.position - armArguments.FixedRetractRate * Time.fixedDeltaTime * _mechArm.TAim);
					else
					{
						Vector2 nad = _mechArm.AnchorDir.normalized, perp = Vector2.Perpendicular(nad);
						if (perp.y > 0) perp = -perp;
						if (RepStart)
						{
							_perpVel = Vector2.Dot(Vector2.up * _rb2D.velocity.y, perp) / repYDamper.value;
							RepStart = false;
						}
						else _perpVel += Vector2.Dot(-slantRepelGravAcc * Time.fixedDeltaTime * Vector2.up, perp);
						CheckRepelArmCol(perp);
						var nv = _perpVel * perp - nad * armArguments.FixedRetractRate;
						_rb2D.velocity = new Vector2(nv.x, Mathf.Clamp(nv.y, -maxFallVelocity, maxFallVelocity));
					}
				}
				else
				{
					if (_mechArm.VertRepelling)
					{
						if (PressingRet) _rb2D.MovePosition(_rb2D.position + armArguments.FixedRetractRate * Time.fixedDeltaTime * _mechArm.TAim);
						else _rb2D.velocity = Vector2.zero;
					}
					else
					{
						Vector2 nad = _mechArm.AnchorDir.normalized, perp = Vector2.Perpendicular(nad);
						if (perp.y > 0) perp = -perp;
						_perpVel += Vector2.Dot(-slantRepelGravAcc * Time.fixedDeltaTime * Vector2.up, perp);
						CheckRepelArmCol(perp);
						Vector2 nv = _perpVel * perp;
						if (PressingRet) nv += nad * armArguments.FixedRetractRate;
						_rb2D.velocity = new Vector2(nv.x, Mathf.Clamp(nv.y, -maxFallVelocity, maxFallVelocity));
					}
				}
			}
			else if (_mechArm.HaExtended)
			{
				Rope arm = _mechArm.Rope;
				Vector2 dir = arm.Start.position - arm.End.position;
				// TODO: Update fist position
				_currXSpeed = (_isDead || IsAttacking) ? 0 : _moveDir * (_shouldRun ? runSpeed : walkSpeed);
				Vector2 newVel = new(_currXSpeed, Mathf.Clamp(_rb2D.velocity.y, -maxFallVelocity, maxFallVelocity));
				var hit = Physics2D.BoxCast((Vector2)arm.End.position + newVel * Time.fixedDeltaTime, new(0.5f, arm.Width), Vector2.SignedAngle(Vector2.right, dir), dir.normalized, dir.magnitude, Settings.LayerMask.OBSTACLE_LMASK);
				if (hit && Vector2.Dot(hit.normal, newVel) < 0)
				{
#if UNITY_EDITOR
					Debug.Log($"Hit dot prod {Vector2.Dot(hit.normal, newVel)} {hit.normal} {newVel}");
#endif
					_rb2D.velocity = Vector2.zero;
				}
				else _rb2D.velocity = newVel;
			}
			else if (!_isTakingDamage)
			{
				if (IsDashing) _rb2D.velocity = (_isLeftDash ? -1 : 1) * dashSpeed * Vector2.right;
				else
				{
					_currXSpeed = (_isDead || IsAttacking) ? 0 : _moveDir * (_shouldRun ? runSpeed : walkSpeed);
					Vector2 newVel = new(_currXSpeed, Mathf.Clamp(_rb2D.velocity.y, -maxFallVelocity, maxFallVelocity));
					_rb2D.velocity = newVel;
				}
			}

			_mechArm.CompleteTick();
			if (IsSwinging)
			{
				if (!(Mathf.Abs(_moveDir) > Const.EPS)) return;
				var endForce = _moveDir * wiggleForce * Vector2.right;
				for (var i = 1; i <= _mechArm.Rope.PointCnt; i++)
					_mechArm.Rope.QueueForce(endForce * Mathf.Pow((float)i / _mechArm.Rope.PointCnt, 2), i - 1);
				return;
			}

			if (_willJmp)
			{
				Jump();
				return;
			}

			CalcGravity();
		}

		private void OnTriggerEnter2D(Collider2D coll2D)
		{
			if (coll2D.gameObject.layer != (int)Settings.Layer.Ground &&
			    coll2D.gameObject.layer != (int)Settings.Layer.Buildup) return;
			// Imm sets grav mult to def to prevent SetBodyGrav running before CalcGrav, willJmp uses downGrav>defGrav to calc init jump vel
			_gravMult = defGravMult;
			_dashAvail |= !IsDashing;
			IsJumping = false;
			if (_preLandJmpBuffCd <= 0) return;
			_willJmp = true;
			_preLandJmpBuffCd = 0;
		}

		private void OnTriggerExit2D(Collider2D coll2D)
		{
			if (coll2D.gameObject.layer != (int)Settings.Layer.Ground &&
			    coll2D.gameObject.layer != (int)Settings.Layer.Buildup) return;
			if (!IsJumping) _jmpBuffCd = jumpBuffer;
		}

		private void OnEnable()
		{
			EventManager.Subscribe(PlayEvents.WeaponEquipped, EquipWeapon);
			EventManager.Subscribe(PlayEvents.WeaponUnequipped, UnequipWeapon);
		}

		private void OnDisable()
		{
			EventManager.Unsubscribe(PlayEvents.WeaponEquipped, EquipWeapon);
			EventManager.Unsubscribe(PlayEvents.WeaponUnequipped, UnequipWeapon);
		}

		private void OnDestroy() => _mechArm.Rope.Dispose();
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
			if (weaponItem) EquipWeapon(weaponItem);
		}

		public void EquipWeapon(object input)
		{
			var ind = Array.FindIndex(weaponMapping, pair => pair.Head == (WeaponItem)input);
			if (ind == -1) throw new Exception("Weapon not found in mapping");
			_weapon = weaponMapping[ind].Tail;
			_slashSize = weaponMapping[ind].Head.Radius * slashSizeConversion;
			TransitionToState(Enum.Parse<State>($"{BaseState}_{_weapon}"));
		}

		private void UnequipWeapon(object input = null)
		{
			_slashSize = 0;
			_weapon = "Nil";
			TransitionToState(Enum.Parse<State>($"{BaseState}_{_weapon}"));
		}

		// Animation states
		private void HandleAnim()
		{
			if (IsAttackState)
			{
				if (!_isGrounded)
					TransitionToState(Enum.Parse<State>($"Jump_{_weapon}"));
				else if (Mathf.Abs(_currXSpeed) > walkSpeed + 0.1f)
					TransitionToState(Enum.Parse<State>($"Run_{_weapon}"));
				else if (_currXSpeed == 0)
					TransitionToState(Enum.Parse<State>($"Idle_{_weapon}"));
				else TransitionToState(Enum.Parse<State>($"Walk_{_weapon}"));
			}
			else if (IsDamageState)
			{
				if (!_isGrounded)
					TransitionToState(Enum.Parse<State>($"Jump_{_weapon}"));
				else if (Mathf.Abs(_currXSpeed) > walkSpeed + 0.1f)
					TransitionToState(Enum.Parse<State>($"Run_{_weapon}"));
				else if (_currXSpeed == 0)
					TransitionToState(Enum.Parse<State>($"Idle_{_weapon}"));
				else TransitionToState(Enum.Parse<State>($"Walk_{_weapon}"));
			}
			else if (IsIdleState)
			{
				if (_currXSpeed != 0)
					TransitionToState(Enum.Parse<State>($"Walk_{_weapon}"));
			}
			else if (IsWalkState)
			{
				if (Mathf.Abs(_currXSpeed) > walkSpeed + .1f)
					TransitionToState(Enum.Parse<State>($"Run_{_weapon}"));
				else if (_currXSpeed == 0)
					TransitionToState(Enum.Parse<State>($"Idle_{_weapon}"));
			}
			else if (IsRunState)
			{
				if (Mathf.Abs(_currXSpeed) < walkSpeed + 0.1f)
					TransitionToState(Enum.Parse<State>($"Walk_{_weapon}"));
			}
			else if (IsDashState)
			{
				if (IsDashing) return;
				if (Mathf.Abs(_currXSpeed) > walkSpeed + 0.1f)
					TransitionToState(Enum.Parse<State>($"Run_{_weapon}"));
				else if (_currXSpeed == 0)
					TransitionToState(Enum.Parse<State>($"Idle_{_weapon}"));
				else TransitionToState(Enum.Parse<State>($"Walk_{_weapon}"));
			}
			else if (IsJumpState)
			{
				if (!_isGrounded || Mathf.Abs(_rb2D.velocity.y) >= .1f) return;
				if (Mathf.Abs(_currXSpeed) > walkSpeed + 0.1f)
					TransitionToState(Enum.Parse<State>($"Run_{_weapon}"));
				else if (_currXSpeed == 0)
					TransitionToState(Enum.Parse<State>($"Idle_{_weapon}"));
				else TransitionToState(Enum.Parse<State>($"Walk_{_weapon}"));
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
				_dashTtl = dashTime;
				_dashAvail = false;
				TransitionToState(Enum.Parse<State>($"Dash_{_weapon}"));
			}
		}

		public void OnAttack(InputAction.CallbackContext context)
		{
			if (context.performed)
			{
				IsAttacking = true;
				_currState = Enum.Parse<State>($"Attack_{_weapon}");
				portrait.Play($"Attack_{_weapon}"); // NOTE: CrossFade is glitchy here
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

		public void OnExtRel(InputAction.CallbackContext context)
		{
			if (context.performed) 
				_mechArm.QueueEvent(ExtendableArm.Trigger.ExtRel);
		}

		public void OnDisc(InputAction.CallbackContext context)
		{
			if (context.performed) 
				_mechArm.QueueEvent(ExtendableArm.Trigger.Disc);
		}

		public void OnHardSoft(InputAction.CallbackContext context)
		{
			if (context.performed) 
				_mechArm.QueueEvent(ExtendableArm.Trigger.HardSoft);
		}

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
			TransitionToState(Enum.Parse<State>($"Death_{_weapon}"));
		}
		#endregion

		#region Animation Event Handlers
		private void DamageEnd()
		{
			_isTakingDamage = false;
			_isInVuln = true;
			_invincibleTtl = postDmgInvincibleTime;
		}

		private void AttackEnd()
		{
			IsAttacking = false;
			OnAttackEnded?.Invoke();
		}

		private void DeathEnd() => EventManager.InvokeEvent(PlayEvents.PlayerDeath);
		#endregion

		#region Helper Methods
		private bool IsFrozen => (GameManager.Instance != null && GameManager.Instance.UI.IsOpen) || _isResting;
		private bool CanDash => !IsFrozen && !IsAttacking && !_isTakingDamage && !_isDead && _dashAvail && !_mechArm.ArmActive;
		private bool CanAttack => !IsFrozen && !IsAttacking && !IsDashing && !_isTakingDamage && !_isDead && !_mechArm.ArmActive;
		private bool CanCastSpell => !IsFrozen && !_isTakingDamage && !_isDead && !IsDashing && !_mechArm.ArmActive;
		private bool CanInteract => !IsFrozen && !IsAttacking && !_isTakingDamage && !_isDead && !IsDashing && !_mechArm.ArmActive;
		private bool CanJump => !IsFrozen && !IsAttacking && !_isTakingDamage && !_isDead && !IsDashing;

		private bool IsIdleState => _currState.ToString().StartsWith("Idle");
		private bool IsWalkState => _currState.ToString().StartsWith("Walk");
		private bool IsRunState => _currState.ToString().StartsWith("Run");
		private bool IsJumpState => _currState.ToString().StartsWith("Jump");
		private bool IsDashState => _currState.ToString().StartsWith("Dash");
		private bool IsDamageState => _currState.ToString().StartsWith("Damage");
		private bool IsAttackState => _currState.ToString().StartsWith("Attack");
		private bool IsDeathState => _currState.ToString().StartsWith("Death");
		private string BaseState => _currState.ToString().Split('_')[0];

		private void InterruptDash()
		{
			Debug.Log("Dash interrupted");
			IsDashing = false;
			_dashTtl = 0f;
			_dashAvail = IsGrounded();
		}

		private void Jump()
		{
			_playerSfx.PlayJump();
			_willJmp = false;
			IsJumping = true;
			_pressingJmp = true;
			var jumpVel = Mathf.Sqrt(-2f * Physics2D.gravity.y * _rb2D.gravityScale * maxJumpHeight);
			_rb2D.velocity = new Vector2(_rb2D.velocity.x, jumpVel);
			TransitionToState(Enum.Parse<State>($"Jump_{_weapon}"));
		}

		void TakeHit()
		{
			_playerSfx.PlayHurt();
			IsAttacking = false;
			_pressingJmp = false;
			_isTakingDamage = true;
			if (_hasKb) _rb2D.AddForce(new Vector2(transform.position.x - _hitFrm.x, 0).normalized * (knockbackImpulse + _kbImp) - _rb2D.velocity * _rb2D.mass, ForceMode2D.Impulse);
			else _rb2D.velocity = new Vector2(0, _rb2D.velocity.y);
			TransitionToState(Enum.Parse<State>($"Damage_{_weapon}"));
		}

		void CalcGravity()
		{
			if (_isGrounded || IsSwinging || _mechArm.Repelling)
			{
				_gravMult = _mechArm.Repelling ? 0 : defGravMult;
				return;
			}

			if (_rb2D.velocity.y > 0.01f)
			{
				if (_pressingJmp && IsJumping) _gravMult = upGravityMult;
				else _gravMult = jumpCutoffMult;
			}
			else if (_rb2D.velocity.y < -0.01f) _gravMult = downGravityMult;
		}

		void SetBodyGrav() => _rb2D.gravityScale = -2 * maxJumpHeight / (timeToApex * timeToApex) / Physics2D.gravity.y * _gravMult;

		void FlipSprite()
		{
			Vector3 currScale = gameObject.transform.localScale;
			currScale.x *= -1;
			gameObject.transform.localScale = currScale;

			IsFacingLeft = !IsFacingLeft;
			EventManager.InvokeEvent(PlayEvents.PlayerSpriteFlip);
		}

		private void TransitionToState(State newState)
		{
			if (_currState == newState) return;
			_currState = newState;
			PlayAnimation(newState.ToString());
		}

		private void PlayAnimation(string animToPlay)
		{
			try
			{
				var animData = portrait.CrossFade(animToPlay, crossFadeSeconds);
				if (animData == null) throw new Exception("Failed to play animation " + animToPlay);
			}
			catch (Exception)
			{
#if UNITY_EDITOR
				Debug.LogWarning($"Error playing animation {animToPlay}. The portrait is likely not initialized");
#endif
			}
		}

		private void CastSpell(int ind)
		{
			var spellItems = GameManager.Instance.PlayerPersistence.SpellItems;
			if (spellItems[ind] == null || !spellItems[ind].CanCast) return;

			var spellObj = Instantiate(spellItems[ind].itemPrefab, transform.position, Quaternion.identity);
			spellObj.GetComponent<Spell>().Cast(IsFacingLeft);
		}

		private bool IsGrounded()
		{
			var boxHit = Physics2D.BoxCast(
				transform.position,
				groundCheckSize,
				0f,
				Vector2.down,
				groundCheckDist,
				Settings.LayerMask.GROUND_LMASK
			);

			return boxHit.collider;
		}

		private void CheckRepelArmCol(Vector2 perp)
		{
			var rope = _mechArm.Rope;
			var dir = (Vector2)rope.Start.position + _perpVel * Time.fixedDeltaTime * perp - (Vector2)rope.End.position; // Sufficient approx rope anchor and rb is close
			if (Physics2D.BoxCast(
				    rope.End.position, new(0.5f, rope.Width), 
				    Vector2.SignedAngle(Vector2.right, dir), dir.normalized, 
				    dir.magnitude, Settings.LayerMask.OBSTACLE_LMASK)) 
				_perpVel = 0;
		}
#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(
				transform.position + Vector3.down * groundCheckDist,
				groundCheckSize
			);

			if (_mechArm == null) return;
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(transform.position, transform.position + (Vector3)_mechArm.Aim);
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, 
				transform.position + (_mechArm.Rope.End.position - 
				                      _mechArm.Rope.Start.position).normalized * armArguments.PlayerCollisionDetectionDistance);
		}
#endif
		#endregion
	}
}
