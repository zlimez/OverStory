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
using JetBrains.Annotations;

// FIXME: When damaged seems to charge further
// Boolean states have a priority type of logic build in a wrapper instead of manual tracking
namespace Abyss.Player
{
	public class PlayerController : MonoBehaviour, ActionInputs.IPlayerActions
	{
		#region Fields
		private enum AnimState // For reference only
		{
			Idle_Nil, Walk_Nil, Run_Nil, Jump_Nil, Dash_Nil, Damage_Nil, Attack_Nil, Death_Nil,
			Idle_Axe_t1, Walk_Axe_t1, Run_Axe_t1, Jump_Axe_t1, Dash_Axe_t1, Damage_Axe_t1, Attack_Axe_t1, Death_Axe_t1,
		}

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
		[SerializeField] private float minJumpDuration = 0.25f, maxJumpHeight = 5;
		[SerializeField] private float downGravityMult = 2f, upGravityMult = 1f, jumpCutoffMult, defGravMult = 1f, slantRepelGravAcc = 5f;
		[SerializeField] private float maxFallVelocity = 15f;
		[SerializeField][Tooltip("Extra time window given to player to jump the moment they leave ground i.e. leave a platform) ")]
		private float coyoteTime = 0.1f;
		[SerializeField][Tooltip("If player becomes grounded with this window after a jump command, the jump will take effect")]
		private float jumpBuffer = 0.1f;

		private float _coyoteCntDwn, _jmpBufferCntDwn, _timeSinceJump, _gravMult = 1;
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
		[SerializeField][Tooltip("Magnitude of wiggle when swinging")]
		private float wiggleForce = 10f;

		[Header("Repel")]
		[SerializeField][Tooltip("Higher the value less the y-velocity carries over when arm first strikes obstacle")]
		private ClampedFloatParameter repYDamper = new(2, 1, 10);
		[NonSerialized] public bool RepStart;
		private float _perpVel; // Used to calc vel perp to repel dir (arm ext)

		public bool PressingRet { get; private set; }
		public bool PressingLen { get; private set; }
		public bool PressingAim { get; private set; }

		private bool _isJumping, _isDashing, _isSwinging;
		public bool IsAttacking { get; private set; }
		private bool _isHurting, _isDead, _isResting, _isInvincible, _isGrounded;
		public Action OnAttackEnded, OnAttemptInteract, OnGrounded;

		private Rigidbody2D _rb2D;
		private AnimState _currAnimState;
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

			_currAnimState = Enum.Parse<AnimState>($"Idle_{_weapon}");
		}

		private void Update()
		{
			if (IsFrozen) _moveDir = 0;

			if (_isDashing)
			{
				if (_dashTtl > 0f) _dashTtl -= Time.deltaTime;
				else
				{
					_isDashing = false;
					_dashAvail = !_isJumping;
				}
			}

			if (_coyoteCntDwn > 0) _coyoteCntDwn = Mathf.Max(0, _coyoteCntDwn - Time.deltaTime);
			if (_jmpBufferCntDwn > 0) _jmpBufferCntDwn = Mathf.Max(0, _jmpBufferCntDwn - Time.deltaTime);
			if (_isJumping) _timeSinceJump += Time.deltaTime;

			if (_invincibleTtl > 0) _invincibleTtl = Mathf.Max(0, _invincibleTtl - Time.deltaTime);
			else _isInvincible = false;

			HandlePassiveAnimChange();
			AdjustSpriteFacing();

			_mechArm.Render(Time.deltaTime);
		}

		// TODO: Freeze movement even when grounded but rope at max stretch
		private void FixedUpdate()
		{
			_mechArm.StartStep(Time.fixedDeltaTime);

			SetBodyGrav();
			_isGrounded = IsGroundedThen();
			if (_isGrounded) OnGrounded?.Invoke();
			if (!_isSwinging && _mechArm.Swingable && !_isGrounded && (!_isJumping || _isJumping && _rb2D.velocity.y < -0.01f))
			{
// #if UNITY_EDITOR
// 				Debug.Log($"Start swinging anchor sqDist {_mechArm.AnchorDir.sqrMagnitude} rope sqLen {_mechArm.Rope.Len * _mechArm.Rope.Len} grounded {_isGrounded}");
// #endif
				_isSwinging = true;
				_rb2D.isKinematic = true;
				_rb2D.velocity = Vector2.zero;
				_mechArm.Rope.QueuePin(Rope.PinPoint.Start);
			}
			else if (_isSwinging && (_isGrounded || !_mechArm.Swingable))
			{
// #if UNITY_EDITOR
// 				Debug.Log($"Stop swinging anchor sqDist {_mechArm.AnchorDir.sqrMagnitude} rope sqLen {_mechArm.Rope.Len * _mechArm.Rope.Len} grounded {_isGrounded}");
// #endif
				_isSwinging = false;
				_rb2D.isKinematic = false;
				if (!_isGrounded) _rb2D.velocity = _mechArm.Rope.EndVel / Time.fixedDeltaTime; // Release preserve momentum
				else if (_mechArm.Swingable) _mechArm.Rope.QueuePin(Rope.PinPoint.Both); // rope len greater than dist to anchor
			}


			if (_mechArm.Repelling) HandleArmRepel();
			else if (_mechArm.HardExtended) HandleArmHardExtension();
			else if (_isSwinging) Swing();
			else HandleHorizontalMovement();

			TryTakeHit();
			TryJump();
			CalcGravity();
			_mechArm.CompleteStep();
		}

		private void OnTriggerExit2D(Collider2D coll2D)
		{
			if (coll2D.gameObject.layer != (int)Settings.Layer.Ground &&
			    coll2D.gameObject.layer != (int)Settings.Layer.Buildup) return;
			if (!_isJumping) _coyoteCntDwn = coyoteTime;
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
			AnimStateTo(Enum.Parse<AnimState>($"{BaseAnim}_{_weapon}"));
		}

		private void UnequipWeapon(object input = null)
		{
			_slashSize = 0;
			_weapon = "Nil";
			AnimStateTo(Enum.Parse<AnimState>($"{BaseAnim}_{_weapon}"));
		}

		private void HandlePassiveAnimChange()
		{
			if (InAttackAnim && !IsAttacking || InHurtAnim && !_isHurting)
			{
				if (!_isGrounded)
					AnimStateTo(Enum.Parse<AnimState>($"Jump_{_weapon}"));
				else if (Mathf.Abs(_currXSpeed) > walkSpeed + 0.1f)
					AnimStateTo(Enum.Parse<AnimState>($"Run_{_weapon}"));
				else if (_currXSpeed == 0)
					AnimStateTo(Enum.Parse<AnimState>($"Idle_{_weapon}"));
				else AnimStateTo(Enum.Parse<AnimState>($"Walk_{_weapon}"));
			}
			else if (InIdleAnim)
			{
				if (_currXSpeed != 0)
					AnimStateTo(Enum.Parse<AnimState>($"Walk_{_weapon}"));
			}
			else if (InWalkAnim)
			{
				if (Mathf.Abs(_currXSpeed) > walkSpeed + .1f)
					AnimStateTo(Enum.Parse<AnimState>($"Run_{_weapon}"));
				else if (_currXSpeed == 0)
					AnimStateTo(Enum.Parse<AnimState>($"Idle_{_weapon}"));
			}
			else if (InRunAnim)
			{
				if (Mathf.Abs(_currXSpeed) < walkSpeed + 0.1f)
					AnimStateTo(Enum.Parse<AnimState>($"Walk_{_weapon}"));
			}
			else if (InDashAnim)
			{
				if (_isDashing) return;
				if (Mathf.Abs(_currXSpeed) > walkSpeed + 0.1f)
					AnimStateTo(Enum.Parse<AnimState>($"Run_{_weapon}"));
				else if (_currXSpeed == 0)
					AnimStateTo(Enum.Parse<AnimState>($"Idle_{_weapon}"));
				else AnimStateTo(Enum.Parse<AnimState>($"Walk_{_weapon}"));
			}
			else if (InJumpAnim && _isGrounded && Mathf.Abs(_rb2D.velocity.y) < .1f)
			{
				if (Mathf.Abs(_currXSpeed) > walkSpeed + 0.1f)
					AnimStateTo(Enum.Parse<AnimState>($"Run_{_weapon}"));
				else if (_currXSpeed == 0)
					AnimStateTo(Enum.Parse<AnimState>($"Idle_{_weapon}"));
				else AnimStateTo(Enum.Parse<AnimState>($"Walk_{_weapon}"));
			}
		}
		#endregion

		#region Event Handlers
		public void OnJump(InputAction.CallbackContext context)
		{
			if (!CanJump) return;
			if (context.performed)
			{
				if (IsGroundedThen() || _coyoteCntDwn > 0) _willJmp = true;
				else _jmpBufferCntDwn = jumpBuffer;
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
			if (!CanDash || !context.performed) return;

			_playerSfx.PlayDash();
			_isDashing = true;
			_isLeftDash = IsFacingLeft;
			_dashTtl = dashTime;
			_dashAvail = false;
			AnimStateTo(Enum.Parse<AnimState>($"Dash_{_weapon}"));
		}

		public void OnAttack(InputAction.CallbackContext context)
		{
			if (!CanAttack || !context.performed) return;

			IsAttacking = true;
			_currAnimState = Enum.Parse<AnimState>($"Attack_{_weapon}");
			portrait.Play($"Attack_{_weapon}"); // NOTE: CrossFade is glitchy here
			weaponSlash.SetInt(xDirParam, IsFacingLeft ? -1 : 1);
			weaponSlash.SetFloat(sizeParam, _slashSize);
			weaponSlash.SendEvent(attackEvent);
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
			if (_isHurting || _isDead || _isInvincible) return true;
			_willTakeHit = true;
			_hasKb = hasKb; _hitFrm = from; _kbImp = kbImpulse;
			return false;
		}

		public void Die()
		{
			IsAttacking = false;
			InterruptDash();
			_pressingJmp = false;
			_isHurting = false;
			_isDead = true;
			AnimStateTo(Enum.Parse<AnimState>($"Death_{_weapon}"));
		}
		#endregion

		#region Animation Event Handlers
		[UsedImplicitly]
		private void DamageEnd()
		{
			_isHurting = false;
			_isInvincible = true;
			_invincibleTtl = postDmgInvincibleTime;
		}

		[UsedImplicitly]
		private void AttackEnd()
		{
			IsAttacking = false;
			OnAttackEnded?.Invoke();
		}

		[UsedImplicitly]
		private void DeathEnd() => EventManager.InvokeEvent(PlayEvents.PlayerDeath);
		#endregion

		#region Helper Methods
		private bool IsFrozen => (GameManager.Instance != null && GameManager.Instance.UI.IsOpen) || _isResting;
		private bool CanDash => !IsFrozen && !IsAttacking && !_isHurting && !_isDead && _dashAvail && !_mechArm.ArmActive;
		private bool CanAttack => !IsFrozen && !IsAttacking && !_isDashing && !_isHurting && !_isDead && !_mechArm.ArmActive;
		private bool CanCastSpell => !IsFrozen && !_isHurting && !_isDead && !_isDashing && !_mechArm.ArmActive;
		private bool CanInteract => !IsFrozen && !IsAttacking && !_isHurting && !_isDead && !_isDashing && !_mechArm.ArmActive;
		private bool CanJump => !IsFrozen && !IsAttacking && !_isHurting && !_isDead && !_isDashing;

		private bool InIdleAnim => _currAnimState.ToString().StartsWith("Idle");
		private bool InWalkAnim => _currAnimState.ToString().StartsWith("Walk");
		private bool InRunAnim => _currAnimState.ToString().StartsWith("Run");
		private bool InJumpAnim => _currAnimState.ToString().StartsWith("Jump");
		private bool InDashAnim => _currAnimState.ToString().StartsWith("Dash");
		private bool InHurtAnim => _currAnimState.ToString().StartsWith("Damage");
		private bool InAttackAnim => _currAnimState.ToString().StartsWith("Attack");
		private string BaseAnim => _currAnimState.ToString().Split('_')[0];

		private void InterruptDash()
		{
#if UNITY_EDITOR
			Debug.Log("Dash interrupted");
#endif
			_isDashing = false;
			_dashTtl = 0f;
		}

		private void TryJump()
		{
			if (!_willJmp) return;

			_timeSinceJump = 0;
			_willJmp = false;
			_isJumping = true;
			_pressingJmp = true;
			var jumpVel = Mathf.Sqrt(-2f * Physics2D.gravity.y * _rb2D.gravityScale * maxJumpHeight);
			_rb2D.velocity = new Vector2(_rb2D.velocity.x, jumpVel);

			_playerSfx.PlayJump();
			AnimStateTo(Enum.Parse<AnimState>($"Jump_{_weapon}"));
		}

		private void TryTakeHit()
		{
			if (!_willTakeHit) return;

			_willTakeHit = false;
			IsAttacking = false;
			_pressingJmp = false;
			_isHurting = true;
			if (_hasKb) _rb2D.AddForce(new Vector2(transform.position.x - _hitFrm.x, 0).normalized * (knockbackImpulse + _kbImp) - _rb2D.velocity * _rb2D.mass, ForceMode2D.Impulse);
			else _rb2D.velocity = new Vector2(0, _rb2D.velocity.y);

			_playerSfx.PlayHurt();
			AnimStateTo(Enum.Parse<AnimState>($"Damage_{_weapon}"));
		}

		private void Swing()
		{
			if (Mathf.Abs(_moveDir) <= Const.EPS) return;

			var endForce = _moveDir * wiggleForce * Vector2.right;
			for (var i = 1; i <= _mechArm.Rope.PointCnt; i++)
				_mechArm.Rope.QueueForce(endForce * Mathf.Pow((float)i / _mechArm.Rope.PointCnt, 2), i - 1);
		}

		private void CalcGravity()
		{
			if (_isGrounded || _isSwinging || _mechArm.Repelling)
			{
				_gravMult = _mechArm.Repelling ? 0 : defGravMult;
				return;
			}

			_gravMult = _rb2D.velocity.y switch
			{
				> 0.01f when _pressingJmp && _isJumping => upGravityMult,
				> 0.01f => jumpCutoffMult,
				< -0.01f => downGravityMult,
				_ => _gravMult
			};
		}

		private void SetBodyGrav() => _rb2D.gravityScale = -2 * maxJumpHeight / (timeToApex * timeToApex) / Physics2D.gravity.y * _gravMult;

		private void AdjustSpriteFacing()
		{
			if (_isSwinging || _mechArm.Repelling || _isHurting || _moveDir <= Const.EPS && IsFacingLeft || _moveDir >= -Const.EPS && !IsFacingLeft) return;

			var currScale = gameObject.transform.localScale;
			currScale.x *= -1;
			gameObject.transform.localScale = currScale;

			IsFacingLeft = !IsFacingLeft;
			EventManager.InvokeEvent(PlayEvents.PlayerSpriteFlip);
		}

		private void AnimStateTo(AnimState newAnimState)
		{
			if (_currAnimState == newAnimState) return;
			_currAnimState = newAnimState;
			PlayAnimation(newAnimState.ToString());
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
			SpellItem[] spellItems = GameManager.Instance.PlayerPersistence.SpellItems;
			if (spellItems[ind] == null || !spellItems[ind].CanCast) return;

			var spellObj = Instantiate(spellItems[ind].itemPrefab, transform.position, Quaternion.identity);
			spellObj.GetComponent<Spell>().Cast(IsFacingLeft);
		}

		private bool IsGroundedThen()
		{
			var boxHit = Physics2D.BoxCast(
				transform.position,
				groundCheckSize,
				0f,
				Vector2.down,
				groundCheckDist,
				Settings.LayerMask.GROUND_LMASK
			);

			if (boxHit.collider && _isJumping && _timeSinceJump >= minJumpDuration)
			{
				_isJumping = false;
				_dashAvail = !_isDashing;

				if (_jmpBufferCntDwn > 0)
				{
					_willJmp = true;
					_jmpBufferCntDwn = 0;
				}
			}

			return boxHit.collider;
		}

		private void HandleHorizontalMovement()
		{
			if (_isHurting) return;

			if (_isDashing) _rb2D.velocity = (_isLeftDash ? -1 : 1) * dashSpeed * Vector2.right;
			else
			{
				_currXSpeed = _isDead || IsAttacking ? 0 : _moveDir * (_shouldRun ? runSpeed : walkSpeed);
				Vector2 newVel = new(_currXSpeed, Mathf.Clamp(_rb2D.velocity.y, -maxFallVelocity, maxFallVelocity));
				_rb2D.velocity = newVel;
			}
		}

		private void HandleArmRepel()
		{
			if (_mechArm.CurrState == ExtendableArm.State.Ha_CoEx_To)
			{
				if (_mechArm.VertRepelling) _rb2D.MovePosition(_rb2D.position - armArguments.FixedRetractRate * Time.fixedDeltaTime * _mechArm.CurrAim);
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
					CheckArmCollisionInRepel(perp);
					var nv = _perpVel * perp - nad * armArguments.FixedRetractRate;
					_rb2D.velocity = new Vector2(nv.x, Mathf.Clamp(nv.y, -maxFallVelocity, maxFallVelocity));
				}
			}
			else
			{
				if (_mechArm.VertRepelling)
				{
					if (PressingRet) _rb2D.MovePosition(_rb2D.position + armArguments.FixedRetractRate * Time.fixedDeltaTime * _mechArm.CurrAim);
					else _rb2D.velocity = Vector2.zero;
				}
				else
				{
					Vector2 nad = _mechArm.AnchorDir.normalized, perp = Vector2.Perpendicular(nad);
					if (perp.y > 0) perp = -perp;
					_perpVel += Vector2.Dot(-slantRepelGravAcc * Time.fixedDeltaTime * Vector2.up, perp);
					CheckArmCollisionInRepel(perp);
					var nv = _perpVel * perp;
					if (PressingRet) nv += nad * armArguments.FixedRetractRate;
					_rb2D.velocity = new Vector2(nv.x, Mathf.Clamp(nv.y, -maxFallVelocity, maxFallVelocity));
				}
			}
		}

		private void CheckArmCollisionInRepel(Vector2 perp)
		{
			var rope = _mechArm.Rope;
			var dir = (Vector2)rope.Start.position + _perpVel * Time.fixedDeltaTime * perp - (Vector2)rope.End.position; // Sufficient approx rope anchor and rb is close
			if (Physics2D.BoxCast(rope.End.position, new(0.5f, rope.Width),
			                      Vector2.SignedAngle(Vector2.right, dir), dir.normalized,
			                      dir.magnitude, Settings.LayerMask.OBSTACLE_LMASK)) _perpVel = 0;
		}

		private void HandleArmHardExtension()
		{
			var arm = _mechArm.Rope;
			Vector2 dir = arm.Start.position - arm.End.position;
			// TODO: Update fist position
			_currXSpeed = _isDead || IsAttacking ? 0 : _moveDir * (_shouldRun ? runSpeed : walkSpeed);
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
			Gizmos.DrawLine(transform.position, transform.position + (Vector3)_mechArm.UserAim);
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, 
				transform.position + (_mechArm.Rope.End.position - 
				                      _mechArm.Rope.Start.position).normalized * armArguments.PlayerCollisionDetectionDistance);
		}
#endif
		#endregion
	}
}
