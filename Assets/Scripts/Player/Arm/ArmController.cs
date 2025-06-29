using System;
using GogoGaga.OptimizedRopesAndCables;
using UnityEngine;
using AI.FSM;
using Abyss.Settings;
using Abyss.Player;
using System.Collections.Generic;
using Abyss.EventSystem;
using UnityEngine.Rendering;

public class ArmController : MonoBehaviour
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

    public Rope rope; // NOTE: Startpoint is fist Endpoint is not necessarily 
    public LineRenderer ropeRenderer;
    public DistanceJoint2D bodyJoint; // NOTE: Placed on the player
    public GameObject fist, fistEnd, fistPH;
    public PlayerController playerCtr;
    public Rigidbody2D playerRb;
    public Vector2 aim = Vector2.right;
    public DroneBT drone;
    Rigidbody2D _fistRb;
    DistanceJoint2D _fistJoint;
    PolygonCollider2D _fistCol;
    ColNotifier _fistExt;
    public Vector2 Taim { get; private set; }

    [Header("延展／回收")]
    [SerializeField][Tooltip("軟式發射衝力")] float softExtImp = 45f;
    [SerializeField][Tooltip("硬式延展速率")] float hardExtRate = 5f;
    [SerializeField][Tooltip("軟式延展速率")] float softExtRate = 5f;
    [Tooltip("一端固定時回收速率")] public float fixedRetractRate = 40f;
    [SerializeField][Tooltip("無固定時回收速率")] float freeRetractRate = 20f;
    [SerializeField][Tooltip("固定回收初始衝力")] float fiRetImp = 100f;
    [SerializeField][Tooltip("瞄準敏感度")] float aimResp = 2f;

    [Header("拳頭")]
    [SerializeField][Tooltip("拳頭質量")] float mass = 1f;
    [SerializeField][Tooltip("拳頭重力加成")] float gravScale = 2f;

    [Header("繩子")]
    [SerializeField][Tooltip("最低線段數")] int minSegCount = 10;
    [SerializeField][Tooltip("最高線段數")] int maxSegCount = 50;
    [SerializeField][Tooltip("線段長")] float segLength = 0.2f, maxLength = 10f;

    [Header("反撐")]
    [SerializeField][Tooltip("反撐時偵測玩家與他物碰撞的距離")] float repColDetDist = 1f;
    [SerializeField][Tooltip("數值愈低玩家反撐且不受重力影響的角度愈廣 [0,1]")] ClampedFloatParameter noGravRng = new(0.75f, 0, 1);

    [Header("脫離")]
    [SerializeField][Tooltip("速度")] float speed = 15f;
    [SerializeField][Tooltip("脫離時偵測拳頭與他物碰撞的距離")] float discColDetDist = 1f;

    Action _phyActions;
    readonly Queue<(Trigger, object)> _triggerQueue = new();
    bool _isHard = false, _hasNewEnd = false, _fiRetBeg = false;
    Vector2 _newEnd;

    readonly FSM _fsm = new((int)State.So_CoOg_Lo);

    public State CurrState => (State)_fsm.CurrState;
    public bool ArmActive => _fsm.CurrState != (int)State.So_CoOg_Lo && _fsm.CurrState != (int)State.Ha_CoOg_Lo;
    public bool Swinging => CurrState == State.So_CoRe_Fi;
    public bool Repeling => CurrState == State.Ha_CoEx_To || CurrState == State.Ha_CoRe_To;
    public bool VertRepeling = false;
    public bool IsSoft => CurrState.ToString().StartsWith("So");
    public Vector2 Anchor => _fistRb.transform.position;
    public Vector2 AnchorDir => Anchor - (Vector2)bodyJoint.transform.position;
    public float Dist2Fist => AnchorDir.magnitude;
    public float JointLen => rope.ropeLength;
    public bool PlayerFree => Dist2Fist < JointLen - Utils.Const.EPS;

    void Awake()
    {
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
        _fistRb.mass = mass;
        _fistJoint.connectedBody = GetComponent<Rigidbody2D>();
        _fistCol = fistEnd.GetComponent<PolygonCollider2D>();

        fist.SetActive(false);
        fistPH.SetActive(true);
        fistEnd.SetActive(false);

        AddTransition(State.So_CoOg_Lo, Trigger.HardSoft, State.Ha_CoOg_Lo, OnHaSoTog, true);
        AddTransition(State.So_CoOg_Lo, Trigger.ExtRel, State.So_CoEx_Lo, OnSoEx);
        AddTransition(State.So_CoOg_Lo, Trigger.Disc, State.So_DiMo, OnDisc);

        AddTransition(State.So_CoEx_Lo, Trigger.Ret, State.So_CoRe_Lo, OnSoRet);
        AddTransition(State.So_CoEx_Lo, Trigger.Hooked, State.So_CoRe_Fi);
        AddTransition(State.So_CoEx_Lo, Trigger.Contact, State.So_CoRe_Lo);
        // AddTransition(State.So_CoEx_Lo, Trigger.HardSoft, State.Ha_CoRe_Lo);

        AddTransition(State.So_CoRe_Lo, Trigger.Kept, State.So_CoOg_Lo, OnKept);
        AddTransition(State.So_CoRe_Lo, Trigger.Ret, State.So_CoRe_Lo, OnSoRet);
        // AddTransition(State.So_CoRe_Lo, Trigger.HardSoft, State.Ha_CoRe_Lo, null, true);

        AddTransition(State.Ha_CoOg_Lo, Trigger.ExtRel, State.Ha_CoEx_Lo, OnHaExLo);
        AddTransition(State.Ha_CoOg_Lo, Trigger.Disc, State.Ha_DiMo, OnDisc);

        AddTransition(State.So_CoRe_Fi, Trigger.ExtRel, State.So_CoRe_Lo, OnSoRel);
        AddTransition(State.So_CoRe_Fi, Trigger.Ret, State.So_CoRe_Fi, OnFiRet); // NOTE: Swinging mechanic
        AddTransition(State.So_CoRe_Fi, Trigger.Kept, State.So_CoOg_Fi, OnKept);
        // AddTransition(State.So_CoRe_Fi, Trigger.HardSoft, State.Ha_CoRe_Fi, null, true);

        AddTransition(State.Ha_CoEx_Lo, Trigger.Aim, State.Ha_CoEx_Lo, OnHaLoAim);
        AddTransition(State.Ha_CoEx_Lo, Trigger.Ret, State.Ha_CoRe_Lo, OnHaRet);
        AddTransition(State.Ha_CoEx_Lo, Trigger.Contact, State.Ha_CoEx_To);
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

        AddTransition(State.Ha_CoOg_Fi, Trigger.ExtRel, State.Ha_CoOg_Lo, OnRel);
        AddTransition(State.Ha_CoOg_Fi, Trigger.HardSoft, State.So_CoOg_Fi, OnHaSoTog, true);

        AddTransition(State.So_CoOg_Fi, Trigger.ExtRel, State.So_CoOg_Lo, OnRel);
    }

    void FixedUpdate()
    {
        if (playerCtr.PressingRet) _fsm.ProcessEvent((int)Trigger.Ret, true);
        else if (playerCtr.PressingLen) _fsm.ProcessEvent((int)Trigger.Ret, false);
        while (_triggerQueue.Count > 0)
        {
            var (trigger, arg) = _triggerQueue.Dequeue();
            Debug.Log($"Processing {trigger}");
            _fsm.ProcessEvent((int)trigger, arg);
        }
        _phyActions?.Invoke();
        if (playerCtr.PressingAim)
        {
            SetAim();
            _fsm.ProcessEvent((int)Trigger.Aim);
        }
    }

    #region Transition Actions
    void OnDisc(object input = null)
    {
        fist.SetActive(true);
        fist.transform.position = fistPH.transform.position;
        fistEnd.transform.position = rope.transform.position;
        fistPH.SetActive(false);

        _fistJoint.enabled = false;
        bodyJoint.enabled = false;
        _fistRb.isKinematic = true;
        Taim = aim;

        rope.SetEndPoint(fistEnd.transform);
        rope.ropeLength = 0;
        _phyActions += Move;
    }

    void Move()
    {
        if (Physics2D.Raycast(fist.transform.position, Taim, discColDetDist, Abyss.Settings.LayerMask.OBSTACLE_LMASK))
        {
            _phyActions -= Move;
            QueueEvent(Trigger.Contact);
            return;
        }
        fist.transform.position += speed * Time.fixedDeltaTime * (Vector3)Taim;
    }

    void OnHaDiRet(object isRet)
    {
        rope.enabled = true;
        ropeRenderer.enabled = true;
        fistEnd.SetActive(true);
        if ((bool)isRet)
        {
            rope.ropeLength = Mathf.Max(0, rope.ropeLength - fixedRetractRate * Time.fixedDeltaTime);
            if (rope.ropeLength <= Utils.Const.EPS)
            {
                QueueEvent(Trigger.Kept);
                return;
            }
        }
        else if (rope.ropeLength < maxLength - Utils.Const.EPS) rope.ropeLength = Mathf.Min(maxLength, rope.ropeLength + hardExtRate * Time.fixedDeltaTime);
        rope.linePoints = Math.Clamp(Mathf.CeilToInt(rope.ropeLength / segLength), minSegCount, maxSegCount);
        rope.EndPoint.position = rope.StartPoint.position - rope.ropeLength * (Vector3)Taim;
        Vector2 dir = rope.StartPoint.position - rope.EndPoint.position;
        Vector2 pdir = Vector2.Perpendicular(dir.normalized);
        Vector2[] colVtx = new Vector2[4];
        colVtx[0] = dir - pdir * rope.ropeWidth / 2;
        colVtx[1] = dir + pdir * rope.ropeWidth / 2;
        colVtx[2] = pdir * rope.ropeWidth / 2;
        colVtx[3] = -pdir * rope.ropeWidth / 2;
        for (int i = 0; i < 4; i++) colVtx[i] = new(colVtx[i].x / fistEnd.transform.lossyScale.x, colVtx[i].y / fistEnd.transform.lossyScale.y);
        _fistCol.SetPath(0, colVtx);
    }

    void OnDiKept(object input = null)
    {
        ropeRenderer.enabled = false;
        rope.enabled = false;
    }

    void OnDrone(object input = null)
    {
        fistEnd.SetActive(false);
        EventManager.InvokeEvent(PlayEvents.GetArm, fist.transform);
    }
    void OnAttached(object input = null) { fist.SetActive(false); }

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
        InitEx();
        _fistRb.isKinematic = false;
        _fistRb.gravityScale = gravScale;
        _phyActions += AppImp;
        _phyActions += Ext;
        _fistExt.OnContact = (other) =>
        {
            if (!other.gameObject.CompareTag(Tag.Hookable)) return;
            _fiRetBeg = true;
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
        rope.ropeLength = Mathf.Min(maxLength, (rope.StartPoint.position - rope.EndPoint.position).magnitude);
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
        if (Mathf.Abs(rope.ropeLength) <= Utils.Const.EPS) QueueEvent(Trigger.Kept);
    }

    void OnFiRet(object isRet)
    {
        if ((bool)isRet)
        {
            rope.ropeLength = Mathf.Max(0, rope.ropeLength - fixedRetractRate * Time.fixedDeltaTime);
            if (_fiRetBeg)
            {
                _fiRetBeg = false;
                playerRb.AddForce(AnchorDir.normalized * fiRetImp, ForceMode2D.Impulse);
            }
        }
        else if (rope.ropeLength < maxLength - Utils.Const.EPS) rope.ropeLength = Mathf.Min(maxLength, rope.ropeLength + softExtRate * Time.fixedDeltaTime);
        rope.linePoints = Math.Clamp(Mathf.CeilToInt(rope.ropeLength / segLength), minSegCount, maxSegCount);
        bodyJoint.distance = rope.ropeLength;
        // NOTE: DO NOT COMMENT THIS OUT FORCES PHYSICS TO UPDATE
        transform.position += 0.01f * Vector3.up;
        transform.position -= 0.01f * Vector3.up;
    }

    void OnHaExLo(object input = null)
    {
        InitEx();
        _fistRb.isKinematic = false;
        _fistRb.gravityScale = 0;

        _phyActions += HaExt;
        Taim = aim;
        _fistExt.OnCollision = (col) =>
        {
            if (col.rigidbody == null || !col.rigidbody.CompareTag(Tag.Movable)) // NOTE: Movable when collide against immovable should change tag
            {
                _phyActions -= HaExt;
                _phyActions += Repel;
                VertRepeling = Vector2.Dot(Vector2.down, aim) > noGravRng.value;
                Taim = aim;
                playerCtr.RepStart = true;
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
        playerCtr.OnGrounded += OnGrndAftRepel;
    }

    void Repel()
    {
        rope.ropeLength = (rope.StartPoint.position - rope.EndPoint.position).magnitude;
        rope.linePoints = Math.Clamp(Mathf.CeilToInt(rope.ropeLength / segLength), minSegCount, maxSegCount);
        Vector2 repdir = (rope.EndPoint.position - rope.StartPoint.position).normalized;
        if (rope.ropeLength > maxLength - Utils.Const.EPS || Physics2D.Raycast(playerCtr.transform.position, repdir, repColDetDist, Abyss.Settings.LayerMask.OBSTACLE_LMASK)) QueueEvent(Trigger.Ret, true);
    }

    void OnHaToRet(object input = null)
    {
        rope.ropeLength = (rope.StartPoint.position - rope.EndPoint.position).magnitude;
        rope.linePoints = Math.Clamp(Mathf.CeilToInt(rope.ropeLength / segLength), minSegCount, maxSegCount);
    }

    void HaExt()
    {
        rope.ropeLength = Mathf.Min(maxLength, rope.ropeLength + hardExtRate * Time.fixedDeltaTime);
        rope.linePoints = Math.Clamp(Mathf.CeilToInt(rope.ropeLength / segLength), minSegCount, maxSegCount);
        // NOTE: newEnd stores temp calc res to possibly be used by aim in the same turn: see sequence of invocation in fixed update
        _hasNewEnd = true;
        _newEnd = rope.EndPoint.position + (Vector3)Taim * rope.ropeLength;
        _fistRb.MovePosition(_newEnd);
        if (rope.ropeLength > maxLength - Utils.Const.EPS)
        {
            _phyActions -= HaExt;
            QueueEvent(Trigger.Ret, true);
        }
    }

    void OnHaLoAim(object input = null)
    {
        Vector3 caim = (_hasNewEnd ? _newEnd : _fistRb.transform.position) - rope.EndPoint.position;
        _hasNewEnd = false;
        Vector3 naim = Quaternion.Euler(0, 0, Vector2.SignedAngle(caim, aim) * aimResp * Time.fixedDeltaTime) * caim, nnaim = naim.normalized;
        if (!Physics2D.BoxCast(rope.EndPoint.position, new(0.5f, rope.ropeWidth), Vector2.SignedAngle(Vector2.right, naim), nnaim, naim.magnitude, Abyss.Settings.LayerMask.OBSTACLE_LMASK))
        {
            Taim = nnaim;
            _fistRb.MovePosition(rope.EndPoint.position + naim);
        }
    }
    #endregion

    #region Helpers
    void AddTransition(State start, Trigger trigger, State next, Action<object> action = null, bool bidir = false, Func<object, bool> cond = null) => _fsm.AddTransition(new((int)start, (int)trigger, (int)next, action, cond), bidir);
    void InitEx()
    {
        rope.enabled = true;
        ropeRenderer.enabled = true;
        rope.ropeLength = 0;
        rope.SetEndPoint(transform);
        fist.SetActive(true);
        fist.transform.position = fistPH.transform.position;
        fistPH.SetActive(false);
    }
    public void QueueEvent(Trigger trigger, object arg = null) => _triggerQueue.Enqueue((trigger, arg));
    public void SetAim() => aim = ((Vector2)(Camera.main.ScreenToWorldPoint(Input.mousePosition) - rope.EndPoint.position)).normalized;

    void AppImp()
    {
        _fistRb.AddForce(aim * softExtImp, ForceMode2D.Impulse);
        _phyActions -= AppImp;
    }

    // TODO: Add in state Ha_CoRe_To only on ground touch then becomes Ha_CoRe_Lo
    void OnGrndAftRepel()
    {
        _fistRb.isKinematic = false;
        _fistJoint.enabled = true;
        _fistJoint.distance = rope.ropeLength;
        QueueEvent(Trigger.Grnded);
    }
    #endregion
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)aim);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(playerCtr.transform.position, playerCtr.transform.position + (rope.EndPoint.position - rope.StartPoint.position).normalized * repColDetDist);
    }
#endif
}
