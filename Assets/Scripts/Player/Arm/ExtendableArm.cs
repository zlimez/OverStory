using System;
using System.Collections.Generic;
using Abyss.EventSystem;
using Abyss.Settings;
using AI.FSM;
using UnityEngine;
using UnityEngine.Rendering;
using Utils;
using VerletPhysics;

namespace Abyss.Player
{
    [Serializable]
    public struct ArmArgs
    {
        public Camera Cam;
        public Transform Fist, DisconnectedFist, Shoulder;
        public GameObject FistPlaceholder;
        public LineRenderer RopeRenderer;
        public float SoftExtImpulse, HardExtRate, SoftExtRate, FixedRetractRate, FreeRetractRate;
        public float AimSensitivity;
        public float PlayerCollisionDetectionDistance, FistCollisionDetectionDistance;
        public float DisconnectedSpeed;
        public PlayerController PlayerCtr;
        public ClampedFloatParameter NoGravityRangeAtRepel;

        public RopeArgs RopeArguments;
    }
    
    public class ExtendableArm
    {
        public enum State
        {
            So_CoOg_Lo, Ha_CoOg_Lo, So_CoEx_Lo, Ha_CoEx_Lo, So_CoRe_Lo, Ha_CoRe_Lo,
            So_CoRe_Fi, Ha_CoEx_To, Ha_CoRe_To, So_CoOg_Fi, Ha_DiMo, So_DiMo, Ha_DiSt, 
            Ha_DiOg, So_DiOg, Ha_DiCa, So_DiCa
        }

        public enum Trigger
        {
            ExtRel, Disc, HardSoft, Ret, Aim, Grounded,
            Contact, DronePick, Kept, Hooked
        }

        private readonly Camera _cam;
        public readonly Rope Rope;
        private float _hardLen;

        private readonly Transform _fist, _diFist, _shoulder;
        private readonly GameObject _fistPh; // PlaceHolder for fist when it is in default state
        private readonly LineRenderer _armRenderer;
        private readonly PlayerController _playerCtr;
        public Vector2 TAim { get; private set; } // Not always equal to aim

        private readonly float _softExtImp, _hardExtRate, _softExtRate;
        private readonly float _fixedRetractRate;
        private readonly float _freeRetractRate;
        private readonly float _aimResp;

        private readonly float _playerColDetDist; // 反撐時偵測玩家與他物碰撞的距離
        private readonly ClampedFloatParameter _noGravRng; // 數值愈低玩家反撐且不受重力影響的角度愈廣

        private readonly float _diSpeed; // 脫離速度
        private readonly float _fistColDetDist; // 偵測拳頭與他物碰撞的距離

        private readonly FSM _fsm = new((int)State.So_CoOg_Lo);
        private readonly Queue<(Trigger, object)> _triggerQueue = new();
        
        private bool _isHard;
        private bool _hasNewEnd, _willAppImp;

        private Vector2 _newEnd;
        private readonly Rigidbody2D _fistRb;
        private readonly PolygonCollider2D _fistCol;
        private readonly ColNotifier _fistExt;
        
        #region Getters and Setters
        public Vector2 Aim { get; private set; } = Vector2.right;
        public State CurrState => (State)_fsm.CurrState;
        public bool ArmActive => CurrState != State.So_CoOg_Lo && CurrState != State.Ha_CoOg_Lo;
        public bool Swingable => CurrState == State.So_CoRe_Fi;
        public bool HaExtended => CurrState is State.Ha_CoEx_Lo or State.Ha_CoRe_Lo;
        public bool Repelling => CurrState is State.Ha_CoEx_To or State.Ha_CoRe_To;
        public bool VertRepelling { get; private set; }
        private bool Connected { get; set; } = true;
        private Vector2 Anchor => Rope.Start.position;
        public Vector2 AnchorDir => Anchor - (Vector2)Rope.End.position;
        #endregion

        public ExtendableArm(ArmArgs armArgs)
        {
            _cam = armArgs.Cam;
            _fist = armArgs.Fist;
            _diFist = armArgs.DisconnectedFist;
            _shoulder = armArgs.Shoulder;
            _fistPh = armArgs.FistPlaceholder;
            _armRenderer = armArgs.RopeRenderer;
            _playerCtr = armArgs.PlayerCtr;
            _aimResp = armArgs.AimSensitivity;
            _softExtImp = armArgs.SoftExtImpulse;
            _softExtRate = armArgs.SoftExtRate;
            _hardExtRate = armArgs.HardExtRate;
            _fixedRetractRate = armArgs.FixedRetractRate;
            _freeRetractRate = armArgs.FreeRetractRate;
            _playerColDetDist = armArgs.PlayerCollisionDetectionDistance;
            _fistColDetDist = armArgs.FistCollisionDetectionDistance;
            _diSpeed = armArgs.DisconnectedSpeed;
            _noGravRng = armArgs.NoGravityRangeAtRepel;
            
            _fistRb = _fist.GetComponent<Rigidbody2D>();
            _fistExt = _fist.GetComponent<ColNotifier>();
            _fistCol = _diFist.GetComponent<PolygonCollider2D>();

            _fist.gameObject.SetActive(false);
            _fistPh.SetActive(true);
            _diFist.gameObject.SetActive(false);
            
            Rope = new Rope(armArgs.RopeArguments);
            Rope.Init();
            Rope.QueuePin(Rope.PinPoint.Both);

            AddTransitions();
            AddStateActions();
        }

        public void StartStep(float deltaTime)
        {
            if (!_isHard && Connected && CurrState != State.So_CoOg_Lo)
                Rope.StartStep(deltaTime);

            _fsm.Tick(deltaTime);
            if (_playerCtr.PressingRet) _fsm.ProcessEvent((int)Trigger.Ret, (true, deltaTime));
            else if (_playerCtr.PressingLen) _fsm.ProcessEvent((int)Trigger.Ret, (false, deltaTime));
            
            while (_triggerQueue.Count > 0)
            {
                var (trigger, arg) = _triggerQueue.Dequeue();
#if UNITY_EDITOR
                Debug.Log($"Processing {trigger}");
#endif
                _fsm.ProcessEvent((int)trigger, trigger == Trigger.Ret ? ((bool)arg, deltaTime) : arg);
            }
            
            if (_willAppImp)
            {
                _fistRb.AddForce(Aim * _softExtImp, ForceMode2D.Impulse);
                _willAppImp = false;
            }
            
            if (_playerCtr.PressingAim)
            {
                SetAim();
                _fsm.ProcessEvent((int)Trigger.Aim);
            }
        }

        public void Render()
        {
            switch (_isHard)
            {
                case true when _hardLen <= Const.EPS:
                case false when Rope.Len <= Const.EPS:
                    _armRenderer.positionCount = 0;
                    return;
                case true:
                    _armRenderer.startColor = Color.cyan;
                    _armRenderer.endColor = Color.cyan;

                    _armRenderer.positionCount = 2;
                    _armRenderer.SetPosition(0, _fist.position);
                    _armRenderer.SetPosition(1, _shoulder.position);
                    break;
                default:
                    _armRenderer.startColor = Color.white;
                    _armRenderer.endColor = Color.white;

                    Rope.Draw(_armRenderer);
                    break;
            }

        }

        public void CompleteStep() => Rope.CompleteStepIfBegan();

        #region Transition Actions

        private void OnDisc(object input = null)
        {
            _fist.gameObject.SetActive(true);
            _fist.transform.position = _fistPh.transform.position;
            _fistPh.SetActive(false);
            Connected = false;
            Rope.Start = _fist;
            Rope.End = _diFist;
            _fistRb.isKinematic = true;
            TAim = Aim;
        }

        private void Move(object deltaTime)
        {
            if (CheckFistCol(TAim)) QueueEvent(Trigger.Contact);
            else _fist.transform.position += _diSpeed * (float)deltaTime * (Vector3)TAim;
        }

        private void OnHaDiRet(object args)
        {
            var (isRet, deltaTime) = ((bool, float))args;
            _diFist.gameObject.SetActive(true);
            if (isRet)
            {
                _hardLen = Mathf.Max(0, _hardLen - _fixedRetractRate * deltaTime);
                if (_hardLen <= Const.EPS)
                {
                    QueueEvent(Trigger.Kept);
                    return;
                }
            }
            else if (_hardLen < Rope.MaxLength - Const.EPS) _hardLen = Mathf.Min(Rope.MaxLength, _hardLen + _hardExtRate * deltaTime);

            Rope.End.position = Rope.Start.position - _hardLen * (Vector3)TAim;
            Vector2 dir = Rope.Start.position - Rope.End.position, perp = Vector2.Perpendicular(dir.normalized);
            var colVtx = new Vector2[4];
            colVtx[0] = dir - perp * Rope.Width / 2;
            colVtx[1] = dir + perp * Rope.Width / 2;
            colVtx[2] = perp * Rope.Width / 2;
            colVtx[3] = -perp * Rope.Width / 2;
            for (var i = 0; i < 4; i++) colVtx[i] = new(colVtx[i].x / _diFist.transform.lossyScale.x, colVtx[i].y / _diFist.transform.lossyScale.y);
            _fistCol.SetPath(0, colVtx);
        }

        private void OnDiKept(object input = null)
        {
            Connected = true;
            Rope.Start = _fist;
            Rope.End = _shoulder;
        }

        private void OnDrone(object input = null)
        {
            _diFist.gameObject.SetActive(false);
            EventManager.InvokeEvent(PlayEvents.GetArm, _fist.transform);
        }
        private void OnAttached(object input = null) => _fist.gameObject.SetActive(false);

        private void OnKept(object input = null)
        {
            _fist.gameObject.SetActive(false);
            _fistPh.SetActive(true);
        }

        private void OnHaSoTog(object input = null) => _isHard = !_isHard;

        private void OnSoEx(object input = null)
        {
            InitEx();
            Rope.Init();
            Rope.QueuePin(Rope.PinPoint.Both);

            _fistRb.isKinematic = false;
            _willAppImp = true;
            _fistExt.OnContact = other =>
            {
                if (!other.gameObject.CompareTag(Tag.Hookable)) return;
                QueueEvent(Trigger.Hooked);
            };
            _fistExt.OnCollision = _ => QueueEvent(Trigger.Contact);
        }

        private void OnSoHook(object input = null)
        {
            SoStop();
            _fistRb.velocity = Vector3.zero;
        }

        private void SoStop(object input = null)
        {
            _fistRb.isKinematic = true;
            _fistRb.velocity = Vector3.zero;
        }

        private void SoEx(object input = null)
        {
            var dist = (Rope.Start.position - Rope.End.position).magnitude;
            if (dist > Rope.Len) Rope.QueueExtend(dist - Rope.Len);
            if (dist > Rope.MaxLength) QueueEvent(Trigger.Ret, true);
        }

        private void OnSoRet(object args)
        {
            var (isRet, deltaTime) = ((bool, float))args;
            if (isRet) Rope.QueueRetract(_freeRetractRate * deltaTime);
            else Rope.QueueExtend(_softExtRate * deltaTime);
            if (Rope.Len <= Const.EPS) QueueEvent(Trigger.Kept);
        }

        private void OnFiRet(object args)
        {
            var (isRet, deltaTime) = ((bool, float))args;
            if (isRet) Rope.QueueRetract(_fixedRetractRate * deltaTime);
            else Rope.QueueExtend(_softExtRate * deltaTime);
        }

        private void HaEx(object deltaTime)
        {
            _hardLen = Mathf.Min(Rope.MaxLength, _hardLen + _hardExtRate * (float)deltaTime);
            if (CheckFistCol(TAim))
            {
                QueueEvent(Trigger.Contact);
                return;
            }
            // NOTE: newEnd stores temp calc res to possibly be used by aim in the same turn: see sequence of invocation in fixed update
            _hasNewEnd = true;
            _newEnd = Rope.End.position + (Vector3)TAim * _hardLen;
            _fistRb.MovePosition(_newEnd);
            if (_hardLen > Rope.MaxLength - Const.EPS) QueueEvent(Trigger.Ret, true);
        }

        private void OnHaCont(object input = null)
        {
            VertRepelling = Vector2.Dot(Vector2.down, Aim) > _noGravRng.value;
            _playerCtr.RepStart = true;
        }

        private void OnHaRet(object args)
        {
            var (isRet, deltaTime) = ((bool, float))args;
            if (isRet) _hardLen = Mathf.Max(0, _hardLen - _freeRetractRate * deltaTime);
            else if (_hardLen < Rope.MaxLength - Const.EPS) _hardLen = Mathf.Min(Rope.MaxLength, _hardLen + _hardExtRate * deltaTime);

            _hasNewEnd = true;
            _newEnd = Rope.End.position + (Rope.Start.position - Rope.End.position).normalized * _hardLen;
            _fistRb.MovePosition(_newEnd);
            if (Mathf.Abs(_hardLen) <= Const.EPS) QueueEvent(Trigger.Kept);
        }

        private void OnHaExLo(object input = null)
        {
            InitEx();
            _fistRb.isKinematic = true;
            TAim = Aim;
        }

        private void OnRepelStop(object input = null) => _playerCtr.OnGrounded += OnGroundAftRepel;

        private void Repel(object input = null)
        {
            _hardLen = (Rope.Start.position - Rope.End.position).magnitude;
            Vector2 repDir = (Rope.End.position - Rope.Start.position).normalized;
            if (_hardLen > Rope.MaxLength - Const.EPS || Physics2D.Raycast(_playerCtr.transform.position, repDir, _playerColDetDist, Settings.LayerMask.OBSTACLE_LMASK)) QueueEvent(Trigger.Ret, true);
        }

        private void OnHaToRet(object input = null) => _hardLen = (Rope.Start.position - Rope.End.position).magnitude;

        private void OnHaLoAim(object deltaTime)
        {
            var cAim = (_hasNewEnd ? _newEnd : _fistRb.transform.position) - Rope.End.position;
            _hasNewEnd = false;
            Vector3 naim = Quaternion.Euler(0, 0, Vector2.SignedAngle(cAim, Aim) * _aimResp * (float)deltaTime) * cAim, nnaim = naim.normalized;
            if (!Physics2D.BoxCast(Rope.End.position, new(0.5f, Rope.Width), Vector2.SignedAngle(Vector2.right, naim), nnaim, naim.magnitude, Settings.LayerMask.OBSTACLE_LMASK))
            {
                TAim = nnaim;
                _fistRb.MovePosition(Rope.End.position + naim);
            }
        }

        private void EntCoReLo(object input = null) => Rope.QueuePin(Rope.PinPoint.End);
        private void EntHaReLo(object input = null) => _fist.transform.parent = _shoulder;
        private void ExitHaReLo(object input = null) => _fist.transform.parent = null;
        #endregion

        #region Helpers

        private void AddTransition(State start, Trigger trigger, State next, Action<object> action = null, bool biDir = false, Func<object, bool> cond = null) 
            => _fsm.AddTransition(new((int)start, (int)trigger, (int)next, action, cond), biDir);

        private void AddEntryAction(State state, Action<object> action) => _fsm.AddEntryAction((int)state, action);
        private void AddInAction(State state, Action<object> action) => _fsm.AddInAction((int)state, action);
        private void AddExitAction(State state, Action<object> action) => _fsm.AddExitAction((int)state, action);

        private void InitEx()
        {
            _fist.gameObject.SetActive(true);
            _fist.transform.position = _fistPh.transform.position;
            _fistPh.SetActive(false);
        }

        public void QueueEvent(Trigger trigger, object arg = null) => _triggerQueue.Enqueue((trigger, arg));
        private void SetAim() => Aim = ((Vector2)(_cam.ScreenToWorldPoint(Input.mousePosition) - Rope.End.position)).normalized;
        private bool CheckFistCol(Vector2 dir) => Physics2D.Raycast(_fist.transform.position, dir.normalized, _fistColDetDist, Abyss.Settings.LayerMask.OBSTACLE_LMASK);

        private void OnGroundAftRepel()
        {
            QueueEvent(Trigger.Grounded);
            _playerCtr.OnGrounded -= OnGroundAftRepel;
        }

        private void AddStateActions()
        {
            AddInAction(State.Ha_DiMo, Move);
            AddInAction(State.So_DiMo, Move);
            AddInAction(State.So_CoEx_Lo, SoEx);
            AddInAction(State.Ha_CoEx_Lo, HaEx);
            AddInAction(State.Ha_CoEx_To, Repel);

            AddEntryAction(State.So_CoRe_Lo, EntCoReLo);
            AddEntryAction(State.Ha_CoRe_Lo, EntHaReLo);
            AddExitAction(State.Ha_CoRe_Lo, ExitHaReLo);
        }

        private void AddTransitions()
        {
            AddTransition(State.So_CoOg_Lo, Trigger.HardSoft, State.Ha_CoOg_Lo, OnHaSoTog, true);
            AddTransition(State.So_CoOg_Lo, Trigger.ExtRel, State.So_CoEx_Lo, OnSoEx);
            AddTransition(State.So_CoOg_Lo, Trigger.Disc, State.So_DiMo, OnDisc);

            AddTransition(State.So_CoEx_Lo, Trigger.Ret, State.So_CoRe_Lo, SoStop);
            AddTransition(State.So_CoEx_Lo, Trigger.Hooked, State.So_CoRe_Fi, OnSoHook);
            AddTransition(State.So_CoEx_Lo, Trigger.Contact, State.So_CoRe_Lo, SoStop);
            // AddTransition(State.So_CoEx_Lo, Trigger.HardSoft, State.Ha_CoRe_Lo);

            AddTransition(State.So_CoRe_Lo, Trigger.Kept, State.So_CoOg_Lo, OnKept);
            AddTransition(State.So_CoRe_Lo, Trigger.Ret, State.So_CoRe_Lo, OnSoRet);
            // AddTransition(State.So_CoRe_Lo, Trigger.HardSoft, State.Ha_CoRe_Lo, null, true);

            AddTransition(State.Ha_CoOg_Lo, Trigger.ExtRel, State.Ha_CoEx_Lo, OnHaExLo);
            AddTransition(State.Ha_CoOg_Lo, Trigger.Disc, State.Ha_DiMo, OnDisc);

            AddTransition(State.So_CoRe_Fi, Trigger.ExtRel, State.So_CoRe_Lo);
            AddTransition(State.So_CoRe_Fi, Trigger.Ret, State.So_CoRe_Fi, OnFiRet); // NOTE: Swinging mechanic
            AddTransition(State.So_CoRe_Fi, Trigger.Kept, State.So_CoOg_Fi, OnKept);
            // AddTransition(State.So_CoRe_Fi, Trigger.HardSoft, State.Ha_CoRe_Fi, null, true);

            AddTransition(State.Ha_CoEx_Lo, Trigger.Aim, State.Ha_CoEx_Lo, OnHaLoAim);
            AddTransition(State.Ha_CoEx_Lo, Trigger.Ret, State.Ha_CoRe_Lo);
            AddTransition(State.Ha_CoEx_Lo, Trigger.Contact, State.Ha_CoEx_To, OnHaCont);
            AddTransition(State.Ha_CoEx_Lo, Trigger.Disc, State.Ha_DiSt); // TODO: Clarify if acts like a separate entity and drops what happens?
            // AddTransition(State.Ha_CoEx_Lo, Trigger.HardSoft, State.So_CoRe_Lo);

            AddTransition(State.Ha_DiMo, Trigger.Contact, State.Ha_DiOg);
            AddTransition(State.So_DiMo, Trigger.Contact, State.So_DiOg);

            AddTransition(State.Ha_DiSt, Trigger.Ret, State.Ha_DiSt, OnHaDiRet);
            AddTransition(State.Ha_DiSt, Trigger.Kept, State.Ha_DiOg, OnDiKept);
            AddTransition(State.Ha_DiOg, Trigger.Ret, State.Ha_DiSt, null, false, isRet => !(bool)isRet);

            AddTransition(State.Ha_CoEx_To, Trigger.Ret, State.Ha_CoRe_To, OnRepelStop);
            AddTransition(State.Ha_CoEx_To, Trigger.Disc, State.Ha_DiSt);

            AddTransition(State.Ha_DiOg, Trigger.ExtRel, State.Ha_DiCa, OnDrone);
            AddTransition(State.So_DiOg, Trigger.ExtRel, State.So_DiCa, OnDrone);

            AddTransition(State.Ha_DiCa, Trigger.Kept, State.Ha_CoOg_Lo, OnAttached);
            AddTransition(State.So_DiCa, Trigger.Kept, State.So_CoOg_Lo, OnAttached);

            AddTransition(State.Ha_CoRe_To, Trigger.Ret, State.Ha_CoRe_To, OnHaToRet);
            AddTransition(State.Ha_CoRe_To, Trigger.Grounded, State.Ha_CoRe_Lo);

            AddTransition(State.Ha_CoRe_Lo, Trigger.Kept, State.Ha_CoOg_Lo, OnKept);
            AddTransition(State.Ha_CoRe_Lo, Trigger.Ret, State.Ha_CoRe_Lo, OnHaRet);
            AddTransition(State.Ha_CoRe_Lo, Trigger.Aim, State.Ha_CoRe_Lo, OnHaLoAim);

            AddTransition(State.So_CoOg_Fi, Trigger.ExtRel, State.So_CoOg_Lo);
        }

        #endregion
    }
}
