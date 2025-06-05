using System;
using GogoGaga.OptimizedRopesAndCables;
using UnityEngine;
using AI.FSM;
using Abyss.Settings;
using Abyss.Player;
using System.Collections.Generic;

public class ArmController : MonoBehaviour
{
    public enum State : int
    {
        So_CoOg_Lo, Ha_CoOg_Lo, So_CoEx_Lo, Ha_CoEx_Lo,
        So_CoRe_Lo, Ha_CoRe_Lo,
        So_CoRe_Fi, Ha_CoRe_Fi, Ha_CoEx_To, Ha_CoRe_To,
        So_CoOg_Fi, Ha_CoOg_Fi,
        Ha_DiMo, So_DiMo, Ha_DiSt, So_DiSt, Ha_DiOg, So_DiOg
    }

    public enum Trigger : int
    {
        ExtRel, Disc, HardSoft, Ret, Aim, Grnded,
        Contact, DronePick, Kept, Hooked
    }

    public Rope rope;
    public LineRenderer ropeRenderer;
    public DistanceJoint2D bodyJoint; // Placed on the player
    public GameObject fist, fistPH; // TODO: When fired disable default fist spawn fist instance as the moving part only when kept switch back
    public PlayerController playerController;
    public Vector2 aim = Vector2.right;
    Rigidbody2D _fistRb;
    DistanceJoint2D _fistJoint;
    ColNotifier _fistExt;
    Vector2 _taim;

    [Header("Extension Settings")]
    [SerializeField] float hardExtRate = 5f;
    [SerializeField] float softExtRate = 5f, fixedRetractRate = 40f, freeRetractRate = 20f, maxLength = 10f;
    [SerializeField] float aimResp = 2f;
    [Header("Fist Settings")]
    [SerializeField] float softExtImpulse = 10f;
    [SerializeField] float fistMass = 1f, gravScale = 2f;
    [Header("Rope Settings")]
    [SerializeField] int minSegCount = 10;
    [SerializeField] int maxSegCount = 50;
    [SerializeField] float segLength = 0.2f;
    [Header("Repel Settings")]
    [SerializeField][Tooltip("Raycast distance used to check for player stuck when repelled")] float colDetDist = 1f;

    Action _phyActions;
    readonly Queue<(Trigger, object)> _triggerQueue = new();
    bool _isHard = false, _hasNewEnd = false;
    Vector2 _newEnd;

    readonly FSM _fsm = new((int)State.So_CoOg_Lo);

    public State CurrState => (State)_fsm.CurrState;
    public bool ArmActive => _fsm.CurrState != (int)State.So_CoOg_Lo && _fsm.CurrState != (int)State.Ha_CoOg_Lo;
    public bool Swinging => CurrState == State.So_CoRe_Fi || CurrState == State.Ha_CoRe_Fi;
    public bool Repeling => CurrState == State.Ha_CoEx_To || CurrState == State.Ha_CoRe_To;
    public bool Returning => CurrState == State.So_CoRe_Lo || CurrState == State.Ha_CoRe_Lo;
    public bool IsSoft => CurrState.ToString().StartsWith("So");
    public Vector2 Anchor => _fistRb.transform.position;
    public Vector2 AnchorDir => Anchor - ((Vector2)bodyJoint.transform.position + bodyJoint.anchor);
    public float Dist2Fist => AnchorDir.magnitude;
    public float JointLen => rope.ropeLength;
    public bool PlayerFree => Dist2Fist < JointLen - Utils.Const.EPS;

    void Awake()
    {
        rope = GetComponent<Rope>();
        ropeRenderer = GetComponent<LineRenderer>();
        rope.enabled = false;
        ropeRenderer.enabled = false;

        bodyJoint.maxDistanceOnly = true;
        bodyJoint.autoConfigureDistance = false;
        bodyJoint.enabled = false;

        _fistRb = fist.GetComponent<Rigidbody2D>();
        _fistJoint = fist.GetComponent<DistanceJoint2D>();
        _fistExt = fist.GetComponent<ColNotifier>();
        _fistJoint.maxDistanceOnly = true;
        _fistJoint.autoConfigureDistance = false;
        _fistJoint.enabled = false;
        _fistRb.mass = fistMass;
        _fistJoint.connectedBody = GetComponent<Rigidbody2D>();

        fist.SetActive(false);
        fistPH.SetActive(true);

        AddTransition(State.So_CoOg_Lo, Trigger.HardSoft, State.Ha_CoOg_Lo, OnHaSoTog, true);
        AddTransition(State.So_CoOg_Lo, Trigger.ExtRel, State.So_CoEx_Lo, OnSoEx);
        AddTransition(State.So_CoOg_Lo, Trigger.Disc, State.So_DiMo);

        AddTransition(State.So_CoEx_Lo, Trigger.Ret, State.So_CoRe_Lo, OnSoRet);
        AddTransition(State.So_CoEx_Lo, Trigger.Hooked, State.So_CoRe_Fi);
        AddTransition(State.So_CoEx_Lo, Trigger.Contact, State.So_CoRe_Lo);
        AddTransition(State.So_CoEx_Lo, Trigger.Disc, State.So_DiMo);
        // AddTransition(State.So_CoEx_Lo, Trigger.HardSoft, State.Ha_CoRe_Lo);

        AddTransition(State.So_CoRe_Lo, Trigger.Kept, State.So_CoOg_Lo, OnKept);
        AddTransition(State.So_CoRe_Lo, Trigger.Ret, State.So_CoRe_Lo, OnSoRet);
        // AddTransition(State.So_CoRe_Lo, Trigger.HardSoft, State.Ha_CoRe_Lo, null, true);

        AddTransition(State.Ha_CoOg_Lo, Trigger.ExtRel, State.Ha_CoEx_Lo, OnHaExLo);
        AddTransition(State.Ha_CoOg_Lo, Trigger.Disc, State.Ha_DiMo);

        AddTransition(State.So_CoRe_Fi, Trigger.ExtRel, State.So_CoRe_Lo, OnSoRel);
        AddTransition(State.So_CoRe_Fi, Trigger.Ret, State.So_CoRe_Fi, OnFiRet); // NOTE: Swinging mechanic
        AddTransition(State.So_CoRe_Fi, Trigger.Kept, State.So_CoOg_Fi, OnKept);
        // AddTransition(State.So_CoRe_Fi, Trigger.HardSoft, State.Ha_CoRe_Fi, null, true);

        AddTransition(State.Ha_CoRe_Fi, Trigger.ExtRel, State.Ha_CoRe_Lo, OnRel);
        AddTransition(State.Ha_CoRe_Fi, Trigger.Ret, State.Ha_CoRe_Fi, OnFiRet);
        AddTransition(State.Ha_CoRe_Fi, Trigger.Kept, State.Ha_CoOg_Fi, OnKept);
        // AddTransition(State.Ha_CoRe_Fi, Trigger.Aim, State.Ha_CoRe_Fi);

        AddTransition(State.Ha_CoEx_Lo, Trigger.Aim, State.Ha_CoEx_Lo, OnHaLoAim);
        AddTransition(State.Ha_CoEx_Lo, Trigger.Ret, State.Ha_CoRe_Lo, OnHaRet);
        AddTransition(State.Ha_CoEx_Lo, Trigger.Hooked, State.Ha_CoRe_Fi);
        AddTransition(State.Ha_CoEx_Lo, Trigger.Contact, State.Ha_CoEx_To);
        AddTransition(State.Ha_CoEx_Lo, Trigger.Disc, State.Ha_DiMo);
        // AddTransition(State.Ha_CoEx_Lo, Trigger.HardSoft, State.So_CoRe_Lo);

        AddTransition(State.Ha_DiMo, Trigger.Contact, State.Ha_DiSt);
        AddTransition(State.So_DiMo, Trigger.Contact, State.So_DiSt);

        AddTransition(State.Ha_DiSt, Trigger.Ret, State.Ha_DiSt);
        AddTransition(State.Ha_DiSt, Trigger.Kept, State.Ha_DiOg);

        AddTransition(State.So_DiSt, Trigger.Ret, State.So_DiSt);
        AddTransition(State.So_DiSt, Trigger.Kept, State.So_DiOg);

        AddTransition(State.Ha_CoEx_To, Trigger.Ret, State.Ha_CoRe_To, OnRepelStop);
        AddTransition(State.Ha_CoEx_To, Trigger.Disc, State.Ha_DiSt);

        AddTransition(State.Ha_DiOg, Trigger.DronePick, State.Ha_CoOg_Lo);
        AddTransition(State.So_DiOg, Trigger.DronePick, State.So_CoOg_Lo);

        AddTransition(State.Ha_CoRe_To, Trigger.Ret, State.Ha_CoRe_To, OnHaToRet);
        AddTransition(State.Ha_CoRe_To, Trigger.Grnded, State.Ha_CoRe_Lo);

        AddTransition(State.Ha_CoRe_Lo, Trigger.Kept, State.Ha_CoOg_Lo, OnKept);
        AddTransition(State.Ha_CoRe_Lo, Trigger.Ret, State.Ha_CoRe_Lo, OnHaRet);
        AddTransition(State.Ha_CoRe_Lo, Trigger.Aim, State.Ha_CoRe_Lo, OnHaLoAim);

        AddTransition(State.Ha_CoOg_Fi, Trigger.ExtRel, State.Ha_CoOg_Lo, OnRel);
        AddTransition(State.Ha_CoOg_Fi, Trigger.HardSoft, State.So_CoOg_Fi, OnHaSoTog, true);

        AddTransition(State.So_CoOg_Fi, Trigger.ExtRel, State.So_CoOg_Lo, OnRel);
    }

    void FixedUpdate()
    {
        if (playerController.PressingRet) _fsm.ProcessEvent((int)Trigger.Ret, true);
        else if (playerController.PressingLen) _fsm.ProcessEvent((int)Trigger.Ret, false);
        while (_triggerQueue.Count > 0)
        {
            var (trigger, arg) = _triggerQueue.Dequeue();
            Debug.Log($"Processing {trigger}");
            _fsm.ProcessEvent((int)trigger, arg);
        }
        _phyActions?.Invoke();
        if (playerController.PressingAim)
        {
            SetAim();
            _fsm.ProcessEvent((int)Trigger.Aim);
        }
    }

    #region Transition Actions
    void OnKept(object input = null)
    {
        fist.SetActive(false);
        fistPH.SetActive(true);
        _fistJoint.enabled = false;
        ropeRenderer.enabled = false;
        rope.enabled = false;
    }

    void OnRel(object input = null)
    {
        bodyJoint.enabled = false;
        _fistJoint.enabled = true;
        _fistJoint.distance = rope.ropeLength;
        _fistRb.isKinematic = false;
        _fistRb.gravityScale = 0;
    }

    void OnSoRel(object input = null)
    {
        OnRel();
        _fistRb.gravityScale = gravScale;
    }

    void OnHaSoTog(object input = null)
    {
        bodyJoint.maxDistanceOnly = !bodyJoint.maxDistanceOnly;
        _fistJoint.maxDistanceOnly = !_fistJoint.maxDistanceOnly;
        _isHard = !_isHard;
        ropeRenderer.startColor = _isHard ? Color.cyan : Color.white;
        ropeRenderer.endColor = _isHard ? Color.cyan : Color.white;
    }

    void OnSoEx(object input = null)
    {
        InitLaunch();
        _fistRb.isKinematic = false;
        _fistRb.gravityScale = gravScale;
        _phyActions += AppImp;
        _phyActions += Ext;
        _fistExt.OnContact = (other) =>
        {
            if (!other.gameObject.CompareTag(Tag.Hookable)) return;
            _phyActions -= Ext;
            _fistExt.OnContact = null;
            _fistExt.OnCollision = null;
            _fistRb.isKinematic = true;
            _fistRb.velocity = Vector3.zero;
            bodyJoint.connectedBody = _fistRb;
            bodyJoint.distance = rope.ropeLength;
            bodyJoint.enabled = true;
            QueueEvent(Trigger.Hooked);
        };

        _fistExt.OnCollision = (col) =>
        {
            _phyActions -= Ext;
            _fistExt.OnContact = null;
            _fistExt.OnCollision = null;
            _fistJoint.distance = rope.ropeLength;
            _fistJoint.enabled = true;
            QueueEvent(Trigger.Contact);
        };
    }

    void Ext()
    {
        rope.ropeLength = Mathf.Min(maxLength, (rope.EndPoint.position - rope.StartPoint.position).magnitude);
        rope.linePoints = Math.Clamp(Mathf.CeilToInt(rope.ropeLength / segLength), minSegCount, maxSegCount);
        if (rope.ropeLength > maxLength - Utils.Const.EPS)
        {
            _phyActions -= Ext;
            QueueEvent(Trigger.Ret, true);
        }
    }

    void OnSoRet(object isRet)
    {
        _phyActions -= Ext;
        // TODO: consider rope ground intersections at intersection points spawn distance joints
        if ((bool)isRet) rope.ropeLength = Mathf.Max(0, rope.ropeLength - freeRetractRate * Time.fixedDeltaTime);
        else if (rope.ropeLength < maxLength - Utils.Const.EPS) rope.ropeLength = Mathf.Min(maxLength, rope.ropeLength + softExtRate * Time.fixedDeltaTime);
        rope.linePoints = Math.Clamp(Mathf.CeilToInt(rope.ropeLength / segLength), minSegCount, maxSegCount);

        _fistJoint.enabled = true;
        _fistJoint.distance = rope.ropeLength;
        _fistExt.OnContact = null;
        _fistExt.OnCollision = null;
        // NOTE: DO NOT COMMENT THIS OUT FORCES PHYSICS TO UPDATE
        transform.position += 0.01f * Vector3.up;
        transform.position -= 0.01f * Vector3.up;
        if (Mathf.Abs(rope.ropeLength) <= Utils.Const.EPS) QueueEvent(Trigger.Kept);
    }

    void OnHaRet(object isRet)
    {
        _phyActions -= HaExt;

        if ((bool)isRet) rope.ropeLength = Mathf.Max(0, rope.ropeLength - freeRetractRate * Time.fixedDeltaTime);
        else if (rope.ropeLength < maxLength - Utils.Const.EPS) rope.ropeLength = Mathf.Min(maxLength, rope.ropeLength + hardExtRate * Time.fixedDeltaTime);
        rope.linePoints = Math.Clamp(Mathf.CeilToInt(rope.ropeLength / segLength), minSegCount, maxSegCount);

        _fistJoint.enabled = true;
        _fistJoint.distance = rope.ropeLength;
        _fistExt.OnContact = null;
        _fistExt.OnCollision = null;
        // NOTE: DO NOT COMMENT THIS OUT FORCES PHYSICS TO UPDATE
        transform.position += 0.01f * Vector3.up;
        transform.position -= 0.01f * Vector3.up;
        if (Mathf.Abs(rope.ropeLength) < Utils.Const.EPS) QueueEvent(Trigger.Kept);
    }

    void OnFiRet(object isRet)
    {
        if ((bool)isRet) rope.ropeLength = Mathf.Max(0, rope.ropeLength - fixedRetractRate * Time.fixedDeltaTime);
        else if (rope.ropeLength < maxLength - Utils.Const.EPS) rope.ropeLength = Mathf.Min(maxLength, rope.ropeLength + softExtRate * Time.fixedDeltaTime);
        rope.linePoints = Math.Clamp(Mathf.CeilToInt(rope.ropeLength / segLength), minSegCount, maxSegCount);
        bodyJoint.distance = rope.ropeLength;
        // NOTE: DO NOT COMMENT THIS OUT FORCES PHYSICS TO UPDATE
        transform.position += 0.01f * Vector3.up;
        transform.position -= 0.01f * Vector3.up;
    }

    void OnHaExLo(object input = null)
    {
        InitLaunch();
        _fistRb.isKinematic = false;
        _fistRb.gravityScale = 0;

        _phyActions += HaExt;
        _taim = aim;
        _fistExt.OnContact = (other) =>
        {
            if (!other.gameObject.CompareTag(Tag.Hookable)) return;
            _phyActions -= HaExt;
            _fistExt.OnContact = null;
            _fistExt.OnCollision = null;
            _fistRb.isKinematic = true;
            _fistRb.velocity = Vector3.zero;
            bodyJoint.enabled = true;
            bodyJoint.connectedBody = _fistRb;
            bodyJoint.distance = rope.ropeLength;
            QueueEvent(Trigger.Hooked);
        };
        _fistExt.OnCollision = (col) =>
        {
            if (col.rigidbody == null || !col.rigidbody.CompareTag(Tag.Movable)) // NOTE: Movable when collide against immovable should change tag
            {
                _phyActions -= HaExt;
                _phyActions += Repel;
                bodyJoint.enabled = true;
                bodyJoint.connectedBody = _fistRb;
                bodyJoint.distance = rope.ropeLength;
                _fistRb.isKinematic = true;
                _fistExt.OnContact = null;
                _fistExt.OnCollision = null;
                QueueEvent(Trigger.Contact);
            }
        };
    }

    void OnRepelStop(object input = null)
    {
        _phyActions -= Repel;
        playerController.OnGrounded += OnGrndAftRepel;
    }

    void Repel()
    {
        bodyJoint.distance = Mathf.Min(maxLength, bodyJoint.distance + hardExtRate * Time.fixedDeltaTime);
        // NOTE: DO NOT COMMENT THIS OUT FORCES PHYSICS TO UPDATE
        bodyJoint.transform.position += 0.01f * Vector3.up;
        bodyJoint.transform.position -= 0.01f * Vector3.up;

        rope.ropeLength = bodyJoint.distance;
        rope.linePoints = Math.Clamp(Mathf.CeilToInt(rope.ropeLength / segLength), minSegCount, maxSegCount);
        Vector2 repdir = (rope.StartPoint.position - rope.EndPoint.position).normalized;
        if (bodyJoint.distance > maxLength - Utils.Const.EPS || Physics2D.Raycast(playerController.transform.position, repdir, colDetDist, Abyss.Settings.LayerMask.OBSTACLE_LMASK)) QueueEvent(Trigger.Ret, true);
    }

    void OnHaToRet(object input = null)
    {
        bodyJoint.distance = Mathf.Max(0, bodyJoint.distance - fixedRetractRate * Time.fixedDeltaTime);
        rope.ropeLength = bodyJoint.distance;
        rope.linePoints = Math.Clamp(Mathf.CeilToInt(rope.ropeLength / segLength), minSegCount, maxSegCount);
    }

    void HaExt()
    {
        rope.ropeLength = Mathf.Min(maxLength, (rope.EndPoint.position - rope.StartPoint.position).magnitude + hardExtRate * Time.fixedDeltaTime);
        rope.linePoints = Math.Clamp(Mathf.CeilToInt(rope.ropeLength / segLength), minSegCount, maxSegCount);
        // NOTE: newEnd stores temp calc res to possibly be used by aim in the same turn: see sequence of invocation in fixed update
        _hasNewEnd = true;
        _newEnd = rope.StartPoint.position + (Vector3)_taim.normalized * rope.ropeLength;
        _fistRb.MovePosition(_newEnd);
        if (rope.ropeLength > maxLength - Utils.Const.EPS)
        {
            _phyActions -= HaExt;
            QueueEvent(Trigger.Ret, true);
        }
    }

    void OnHaLoAim(object input = null)
    {
        Vector3 caim = (_hasNewEnd ? _newEnd : _fistRb.transform.position) - rope.StartPoint.position;
        _hasNewEnd = false;
        Vector3 naim = Quaternion.Euler(0, 0, Vector2.SignedAngle(caim, aim) * aimResp * Time.fixedDeltaTime) * caim;
        _taim = naim;
        _fistRb.MovePosition(rope.StartPoint.position + naim);
    }
    #endregion

    #region Helpers
    void AddTransition(State start, Trigger trigger, State next, Action<object> action = null, bool bidir = false) => _fsm.AddTransition(new((int)start, (int)trigger, (int)next, action), bidir);
    void InitLaunch()
    {
        rope.enabled = true;
        ropeRenderer.enabled = true;
        fist.SetActive(true);
        fist.transform.position = fistPH.transform.position;
        fistPH.SetActive(false);
    }
    public void QueueEvent(Trigger trigger, object arg = null) => _triggerQueue.Enqueue((trigger, arg));
    public void SetAim() => aim = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - rope.StartPoint.position).normalized;

    void AppImp()
    {
        _fistRb.AddForce(aim * softExtImpulse, ForceMode2D.Impulse);
        _phyActions -= AppImp;
    }

    // TODO: Add in state Ha_CoRe_To only on ground touch then becomes Ha_CoRe_Lo
    void OnGrndAftRepel()
    {
        _fistRb.isKinematic = false;
        _fistJoint.enabled = true;
        _fistJoint.distance = rope.ropeLength;
        bodyJoint.enabled = false;
        QueueEvent(Trigger.Grnded);
    }
    #endregion
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)aim);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(playerController.transform.position, playerController.transform.position + (rope.StartPoint.position - rope.EndPoint.position).normalized * colDetDist);
    }
#endif
}
