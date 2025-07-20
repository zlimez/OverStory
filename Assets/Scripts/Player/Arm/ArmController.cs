using System;
using VerletPhysics;
using UnityEngine;
using AI.FSM;
using Abyss.Settings;
using System.Collections.Generic;
using Abyss.EventSystem;
using UnityEngine.Rendering;
using Utils;

namespace Abyss.Player
{
    [Serializable]
    public class ArmController
    {
        public enum State : int
        {
            So_CoOg_Lo, Ha_CoOg_Lo, So_CoEx_Lo, Ha_CoEx_Lo,
            So_CoRe_Lo, Ha_CoRe_Lo,
            So_CoRe_Fi, Ha_CoEx_To, Ha_CoRe_To,
            So_CoOg_Fi, Ha_CoOg_Fi,
            Ha_DiMo, So_DiMo, Ha_DiSt, Ha_DiOg, So_DiOg, Ha_DiCa, So_DiCa
        }

        public enum Trigger : int
        {
            ExtRel, Disc, HardSoft, Ret, Aim, Grnded,
            Contact, DronePick, Kept, Hooked
        }

        public Rope arm;
        public float HardLen = 0f;

        public Transform Fist, DiFist, Shoulder;
        public GameObject FistPH; // PlaceHolder for fist when it is in default state
        public LineRenderer RopeRenderer;
        public PlayerController PlayerCtr;
        public DroneBT Drone;
        public Vector2 Taim { get; private set; } // Not always equal to aim

        public float SoftExtImp = 45f, HardExtRate = 5f, SoftExtRate = 5f;
        public float FixedRetractRate = 40f, FreeRetractRate = 20f;
        public float AimResp = 2f;

        public float PlayerColDetDist = 1.5f; // 反撐時偵測玩家與他物碰撞的距離
        public ClampedFloatParameter NoGravRng = new(0.75f, 0, 1); // 數值愈低玩家反撐且不受重力影響的角度愈廣

        public float DiSpeed = 15f; // 脫離速度
        public float FistColDetDist = 1f; // 偵測拳頭與他物碰撞的距離

        readonly FSM _fsm = new((int)State.So_CoOg_Lo);
        readonly Queue<(Trigger, object)> _triggerQueue = new();
        public bool IsHard { get; private set; } = false;
        bool _hasNewEnd = false, _willAppImp = false;
        Vector2 _newEnd, _aim = Vector2.right;
        Rigidbody2D _fistRb;
        PolygonCollider2D _fistCol;
        ColNotifier _fistExt;

        public State CurrState => (State)_fsm.CurrState;
        public bool ArmActive => CurrState != State.So_CoOg_Lo && CurrState != State.Ha_CoOg_Lo;
        public bool Swingable => CurrState == State.So_CoRe_Fi;
        public bool HaExtended => CurrState == State.Ha_CoEx_Lo || CurrState == State.Ha_CoRe_Lo;
        public bool Repeling => CurrState == State.Ha_CoEx_To || CurrState == State.Ha_CoRe_To;
        public bool VertRepeling = false;
        public bool Connected { get; private set; } = true;
        public Vector2 Anchor => Fist.transform.position;
        public Vector2 AnchorDir => Anchor - (Vector2)arm.End.position;

        public void Init()
        {
            arm.Init();
            arm.Pin(Rope.PinPoint.Both);
            arm.Start = Fist;
            arm.End = Shoulder;

            _fistRb = Fist.GetComponent<Rigidbody2D>();
            _fistExt = Fist.GetComponent<ColNotifier>();
            _fistCol = DiFist.GetComponent<PolygonCollider2D>();

            Fist.gameObject.SetActive(false);
            FistPH.SetActive(true);
            DiFist.gameObject.SetActive(false);

            AddTransitions();
            AddStateActions();
        }

        public void Tick(float deltaTime)
        {
            if (!IsHard && Connected && CurrState != State.So_CoOg_Lo) arm.Tick(deltaTime);
            _fsm.Tick(deltaTime);
            if (PlayerCtr.PressingRet) _fsm.ProcessEvent((int)Trigger.Ret, true);
            else if (PlayerCtr.PressingLen) _fsm.ProcessEvent((int)Trigger.Ret, false);
            while (_triggerQueue.Count > 0)
            {
                var (trigger, arg) = _triggerQueue.Dequeue();
                Debug.Log($"Processing {trigger}");
                _fsm.ProcessEvent((int)trigger, arg);
            }
            if (_willAppImp)
            {
                _fistRb.AddForce(_aim * SoftExtImp, ForceMode2D.Impulse);
                _willAppImp = false;
            }
            if (PlayerCtr.PressingAim)
            {
                SetAim();
                _fsm.ProcessEvent((int)Trigger.Aim);
            }
        }

        public void Draw()
        {
            if (IsHard)
            {
                RopeRenderer.positionCount = 2;
                RopeRenderer.SetPosition(0, arm.Start.position);
                RopeRenderer.SetPosition(1, arm.End.position);
            }
            arm.Draw(RopeRenderer);
        }

        #region Transition Actions
        void OnDisc(object input = null)
        {
            Fist.gameObject.SetActive(true);
            Fist.transform.position = FistPH.transform.position;
            FistPH.SetActive(false);
            Connected = false;
            arm.Start = Fist;
            arm.End = DiFist;
            _fistRb.isKinematic = true;
            Taim = _aim;
        }

        void Move(object deltaTime)
        {
            if (CheckFistCol(Taim)) QueueEvent(Trigger.Contact);
            else Fist.transform.position += DiSpeed * (float)deltaTime * (Vector3)Taim;
        }

        void OnHaDiRet(object args)
        {
            var largs = (List<object>)args;
            bool isRet = (bool)largs[0];
            float deltaTime = (float)largs[1];
            RopeRenderer.enabled = true;
            DiFist.gameObject.SetActive(true);
            if (isRet)
            {
                HardLen = Mathf.Max(0, HardLen - FixedRetractRate * deltaTime);
                if (HardLen <= Const.EPS)
                {
                    QueueEvent(Trigger.Kept);
                    return;
                }
            }
            else if (HardLen < arm.MaxLength - Const.EPS) HardLen = Mathf.Min(arm.MaxLength, HardLen + HardExtRate * deltaTime);

            arm.End.position = arm.Start.position - HardLen * (Vector3)Taim;
            Vector2 dir = arm.Start.position - arm.End.position, pdir = Vector2.Perpendicular(dir.normalized);
            Vector2[] colVtx = new Vector2[4];
            colVtx[0] = dir - pdir * arm.Width / 2;
            colVtx[1] = dir + pdir * arm.Width / 2;
            colVtx[2] = pdir * arm.Width / 2;
            colVtx[3] = -pdir * arm.Width / 2;
            for (int i = 0; i < 4; i++) colVtx[i] = new(colVtx[i].x / DiFist.transform.lossyScale.x, colVtx[i].y / DiFist.transform.lossyScale.y);
            _fistCol.SetPath(0, colVtx);
        }

        void OnDiKept(object input = null)
        {
            RopeRenderer.positionCount = 0;
            Connected = true;
            arm.Start = Fist;
            arm.End = Shoulder;
        }

        void OnDrone(object input = null)
        {
            DiFist.gameObject.SetActive(false);
            EventManager.InvokeEvent(PlayEvents.GetArm, Fist.transform);
        }
        void OnAttached(object input = null) { Fist.gameObject.SetActive(false); }

        void OnKept(object input = null)
        {
            Fist.gameObject.SetActive(false);
            FistPH.SetActive(true);
            RopeRenderer.positionCount = 0;
        }

        void OnHaSoTog(object input = null)
        {
            IsHard = !IsHard;
            RopeRenderer.startColor = IsHard ? Color.cyan : Color.white;
            RopeRenderer.endColor = IsHard ? Color.cyan : Color.white;
        }

        void OnSoEx(object input = null)
        {
            InitEx();
            arm.Init();
            arm.Pin(Rope.PinPoint.Both);

            _fistRb.isKinematic = false;
            _willAppImp = true;
            _fistExt.OnContact = (other) =>
            {
                if (!other.gameObject.CompareTag(Tag.Hookable)) return;
                QueueEvent(Trigger.Hooked);
            };
            _fistExt.OnCollision = (col) => QueueEvent(Trigger.Contact);
        }

        void OnSoHook(object input = null)
        {
            SoStop();
            _fistRb.velocity = Vector3.zero;
        }

        void SoStop(object input = null)
        {
            _fistRb.isKinematic = true;
            _fistExt.OnContact = null;
            _fistExt.OnCollision = null;
        }

        void SoEx(object input = null)
        {
            float dist = (arm.Start.position - arm.End.position).magnitude;
            if (dist > arm.Len) arm.Extend(dist - arm.Len);
            if (dist > arm.MaxLength) QueueEvent(Trigger.Ret, true);
        }

        void OnSoRet(object args)
        {
            var largs = (List<object>)args;
            bool isRet = (bool)largs[0];
            float deltaTime = (float)largs[1];

            if (isRet) arm.Retract(FreeRetractRate * deltaTime);
            else arm.Extend(SoftExtRate * deltaTime);
            if (arm.Len <= Const.EPS) QueueEvent(Trigger.Kept);
        }

        void OnFiRet(object args)
        {
            var largs = (List<object>)args;
            bool isRet = (bool)largs[0];
            float deltaTime = (float)largs[1];

            if (isRet) arm.Retract(FixedRetractRate * deltaTime);
            else arm.Extend(SoftExtRate * deltaTime);
        }

        void HaEx(object deltaTime)
        {
            HardLen = Mathf.Min(arm.MaxLength, HardLen + HardExtRate * (float)deltaTime);
            if (CheckFistCol(Taim))
            {
                QueueEvent(Trigger.Contact);
                return;
            }
            // NOTE: newEnd stores temp calc res to possibly be used by aim in the same turn: see sequence of invocation in fixed update
            _hasNewEnd = true;
            _newEnd = arm.End.position + (Vector3)Taim * HardLen;
            _fistRb.MovePosition(_newEnd);
            if (HardLen > arm.MaxLength - Const.EPS) QueueEvent(Trigger.Ret, true);
        }

        void OnHaCont(object input = null)
        {
            VertRepeling = Vector2.Dot(Vector2.down, _aim) > NoGravRng.value;
            PlayerCtr.RepStart = true;
        }

        void OnHaRet(object args)
        {
            var largs = (List<object>)args;
            bool isRet = (bool)largs[0];
            float deltaTime = (float)largs[1];

            if (isRet) HardLen = Mathf.Max(0, HardLen - FreeRetractRate * deltaTime);
            else if (HardLen < arm.MaxLength - Const.EPS) HardLen = Mathf.Min(arm.MaxLength, HardLen + HardExtRate * deltaTime);

            _hasNewEnd = true;
            _newEnd = arm.End.position + (arm.Start.position - arm.End.position).normalized * HardLen;
            _fistRb.MovePosition(_newEnd);
            if (Mathf.Abs(HardLen) <= Const.EPS) QueueEvent(Trigger.Kept);
        }

        void OnHaExLo(object input = null)
        {
            InitEx();
            _fistRb.isKinematic = true;
            Taim = _aim;
        }

        void OnRepelStop(object input = null) => PlayerCtr.OnGrounded += OnGrndAftRepel;

        void Repel(object input = null)
        {
            HardLen = (arm.Start.position - arm.End.position).magnitude;
            Vector2 repdir = (arm.End.position - arm.Start.position).normalized;
            if (HardLen > arm.MaxLength - Const.EPS || Physics2D.Raycast(PlayerCtr.transform.position, repdir, PlayerColDetDist, Settings.LayerMask.OBSTACLE_LMASK)) QueueEvent(Trigger.Ret, true);
        }

        void OnHaToRet(object input = null) => HardLen = (arm.Start.position - arm.End.position).magnitude;

        void OnHaLoAim(object deltaTime)
        {
            Vector3 caim = (_hasNewEnd ? _newEnd : _fistRb.transform.position) - arm.End.position;
            _hasNewEnd = false;
            Vector3 naim = Quaternion.Euler(0, 0, Vector2.SignedAngle(caim, _aim) * AimResp * (float)deltaTime) * caim, nnaim = naim.normalized;
            if (!Physics2D.BoxCast(arm.End.position, new(0.5f, arm.Width), Vector2.SignedAngle(Vector2.right, naim), nnaim, naim.magnitude, Settings.LayerMask.OBSTACLE_LMASK))
            {
                Taim = nnaim;
                _fistRb.MovePosition(arm.End.position + naim);
            }
        }

        // void DragFist(object input = null) => Fist.transform.position = arm.PointPos(0);
        void EntCoReLo(object input = null) => arm.Pin(Rope.PinPoint.End);
        void EntHaReLo(object input = null) => Fist.transform.parent = Shoulder;
        void ExitHaReLo(object input = null) => Fist.transform.parent = null;
        #endregion

        #region Helpers
        public void AddTransition(State start, Trigger trigger, State next, Action<object> action = null, bool bidir = false, Func<object, bool> cond = null) => _fsm.AddTransition(new((int)start, (int)trigger, (int)next, action, cond), bidir);
        public void AddEntryAction(State state, Action<object> action) => _fsm.AddEntryAction((int)state, action);
        public void AddInAction(State state, Action<object> action) => _fsm.AddInAction((int)state, action);
        public void AddExitAction(State state, Action<object> action) => _fsm.AddExitAction((int)state, action);

        void InitEx()
        {
            Fist.gameObject.SetActive(true);
            Fist.transform.position = FistPH.transform.position;
            FistPH.SetActive(false);
        }

        public void QueueEvent(Trigger trigger, object arg = null) => _triggerQueue.Enqueue((trigger, arg));
        public void SetAim() => _aim = ((Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition) - arm.End.position)).normalized;
        bool CheckFistCol(Vector2 dir) => Physics2D.Raycast(Fist.transform.position, dir.normalized, FistColDetDist, Abyss.Settings.LayerMask.OBSTACLE_LMASK);

        void OnGrndAftRepel()
        {
            QueueEvent(Trigger.Grnded);
            PlayerCtr.OnGrounded -= OnGrndAftRepel;
        }

        void AddStateActions()
        {
            AddInAction(State.Ha_DiMo, Move);
            AddInAction(State.So_DiMo, Move);
            AddInAction(State.So_CoEx_Lo, SoEx);
            AddInAction(State.Ha_CoEx_Lo, HaEx);
            AddInAction(State.Ha_CoEx_To, Repel);
            // AddInAction(State.So_CoRe_Lo, DragFist);

            AddEntryAction(State.So_CoRe_Lo, EntCoReLo);
            AddEntryAction(State.Ha_CoRe_Lo, EntHaReLo);
            AddExitAction(State.Ha_CoRe_Lo, ExitHaReLo);
        }

        void AddTransitions()
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
            AddTransition(State.Ha_CoRe_To, Trigger.Grnded, State.Ha_CoRe_Lo);

            AddTransition(State.Ha_CoRe_Lo, Trigger.Kept, State.Ha_CoOg_Lo, OnKept);
            AddTransition(State.Ha_CoRe_Lo, Trigger.Ret, State.Ha_CoRe_Lo, OnHaRet);
            AddTransition(State.Ha_CoRe_Lo, Trigger.Aim, State.Ha_CoRe_Lo, OnHaLoAim);

            AddTransition(State.So_CoOg_Fi, Trigger.ExtRel, State.So_CoOg_Lo);
        }

        #endregion
        // #if UNITY_EDITOR
        //         void OnDrawGizmos()
        //         {
        //             Gizmos.color = Color.red;
        //             Gizmos.DrawLine(transform.position, transform.position + (Vector3)_aim);
        //             Gizmos.color = Color.green;
        //             Gizmos.DrawLine(PlayerCtr.transform.position, PlayerCtr.transform.position + (Earm.End.position - Earm.Start.position).normalized * PlayerColDetDist);
        //         }
        // #endif
    }
}