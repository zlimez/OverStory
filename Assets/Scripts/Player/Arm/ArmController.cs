using System.Collections;
using System;
using GogoGaga.OptimizedRopesAndCables;
using UnityEngine;
using AI.FSM;
using UnityEngine.Rendering;
using Abyss.Settings;
using System.Collections.Generic;
using Abyss.Player;

public class ArmController : MonoBehaviour
{
    public enum State : int
    {
        So_CoOg_Lo, Ha_CoOg_Lo, So_CoEx_Lo, Ha_CoEx_Lo,
        So_CoRe_Lo, Ha_CoRe_Lo,
        So_CoRe_Fi, Ha_CoRe_Fi, Ha_CoEx_To,
        So_CoOg_Fi, Ha_CoOg_Fi
    }

    public enum Trigger : int
    {
        ExtRel, Disc, HardSoft, Ret, Aim,
        Contact, DronePick, Kept, Hooked
    }

    public Rope rope;
    public LineRenderer ropeRenderer;
    public DistanceJoint2D bodyJoint; // Placed on the player
    public GameObject fistK, fistD, fistPH; // TODO: When fired disable default fist spawn fist instance as the moving part only when kept switch back
    public ColNotifier bodyExt;
    public PlayerController playerController;
    public Vector2 aim = Vector2.right;
    Rigidbody2D _fistRb;
    DistanceJoint2D _fistJoint;
    ColNotifier _fistExt;

    [Header("Extension Settings")]
    [SerializeField] float hardExtRate = 5f;
    [SerializeField] float retractRate = 5f, maxLength = 10f;
    [SerializeField] ClampedFloatParameter aimResp = new(0.0f, 0.1f, 1f);
    [Header("Fist Settings")]
    [SerializeField] float softExtImpulse = 10f;
    [SerializeField] float hardContImpulse = 10f, fistMass = 1f, fistJointExDist = 0.3f;
    [Header("Rope Settings")]
    [SerializeField] int minSegCount = 10;
    [SerializeField] int maxSegCount = 50;
    [SerializeField] float segLength = 0.2f;
    Queue<Trigger> _eventQueue = new();
    IEnumerator _extRout;
    bool _willAppImp = false;
    // GameObject _activeFist;

    readonly FSM _fsm = new((int)State.So_CoOg_Lo);

    public State CurrState => (State)_fsm.CurrState;
    public bool ArmActive => _fsm.CurrState != (int)State.So_CoOg_Lo && _fsm.CurrState != (int)State.Ha_CoOg_Lo;

    void Awake()
    {
        rope = GetComponent<Rope>();
        ropeRenderer = GetComponent<LineRenderer>();
        rope.enabled = false;
        ropeRenderer.enabled = false;

        bodyJoint.maxDistanceOnly = true;
        bodyJoint.autoConfigureDistance = false;
        bodyJoint.enabled = false;

        _fistRb = fistK.GetComponent<Rigidbody2D>();
        _fistJoint = fistK.GetComponent<DistanceJoint2D>();
        _fistExt = fistK.GetComponent<ColNotifier>();
        _fistJoint.maxDistanceOnly = true;
        _fistJoint.autoConfigureDistance = false;
        _fistJoint.enabled = false;
        _fistRb.mass = fistMass;
        _fistJoint.connectedBody = GetComponent<Rigidbody2D>();

        fistK.SetActive(false);
        // fistD.SetActive(false);
        fistPH.SetActive(true);

        AddTransition(State.So_CoOg_Lo, Trigger.HardSoft, State.Ha_CoOg_Lo, OnHaSoTog, true);
        AddTransition(State.So_CoOg_Lo, Trigger.ExtRel, State.So_CoEx_Lo, OnSoEx);

        AddTransition(State.So_CoEx_Lo, Trigger.Ret, State.So_CoRe_Lo);
        AddTransition(State.So_CoEx_Lo, Trigger.Hooked, State.So_CoRe_Fi);
        AddTransition(State.So_CoEx_Lo, Trigger.Contact, State.So_CoRe_Lo);
        // AddTransition(State.So_CoEx_Lo, Trigger.HardSoft, State.Ha_CoRe_Lo);

        AddTransition(State.So_CoRe_Lo, Trigger.Kept, State.So_CoOg_Lo, OnKept);
        AddTransition(State.So_CoRe_Lo, Trigger.Ret, State.So_CoRe_Lo, OnSoRet);
        // AddTransition(State.So_CoRe_Lo, Trigger.HardSoft, State.Ha_CoRe_Lo, null, true);

        AddTransition(State.Ha_CoOg_Lo, Trigger.ExtRel, State.Ha_CoEx_Lo, HaExLo);

        AddTransition(State.So_CoRe_Fi, Trigger.ExtRel, State.So_CoRe_Lo, OnSoRel);
        AddTransition(State.So_CoRe_Fi, Trigger.Ret, State.So_CoRe_Fi, OnFiRet); // NOTE: Swinging mechanic
        AddTransition(State.So_CoRe_Fi, Trigger.Kept, State.So_CoOg_Fi, OnKept);
        // AddTransition(State.So_CoRe_Fi, Trigger.HardSoft, State.Ha_CoRe_Fi, null, true);

        AddTransition(State.Ha_CoRe_Fi, Trigger.ExtRel, State.Ha_CoRe_Lo, OnRel);
        AddTransition(State.Ha_CoRe_Fi, Trigger.Ret, State.Ha_CoRe_Fi, OnFiRet);
        AddTransition(State.Ha_CoRe_Fi, Trigger.Kept, State.Ha_CoOg_Fi, OnKept);
        // AddTransition(State.Ha_CoRe_Fi, Trigger.Aim, State.Ha_CoRe_Fi);

        AddTransition(State.Ha_CoEx_Lo, Trigger.Aim, State.Ha_CoEx_Lo, HaLoAim);
        AddTransition(State.Ha_CoEx_Lo, Trigger.Ret, State.Ha_CoRe_Lo, OnHaRet);
        AddTransition(State.Ha_CoEx_Lo, Trigger.Hooked, State.Ha_CoRe_Fi);
        AddTransition(State.Ha_CoEx_Lo, Trigger.Contact, State.Ha_CoEx_To);
        // AddTransition(State.Ha_CoEx_Lo, Trigger.HardSoft, State.So_CoRe_Lo);

        AddTransition(State.Ha_CoEx_To, Trigger.Ret, State.Ha_CoRe_Lo);

        AddTransition(State.Ha_CoRe_Lo, Trigger.Kept, State.Ha_CoOg_Lo, OnKept);
        AddTransition(State.Ha_CoRe_Lo, Trigger.Ret, State.Ha_CoRe_Lo, OnHaRet);
        AddTransition(State.Ha_CoRe_Lo, Trigger.Aim, State.Ha_CoRe_Lo, HaLoAim);

        AddTransition(State.Ha_CoOg_Fi, Trigger.ExtRel, State.Ha_CoOg_Lo, OnRel);
        AddTransition(State.Ha_CoOg_Fi, Trigger.HardSoft, State.So_CoOg_Fi, OnHaSoTog, true);

        AddTransition(State.So_CoOg_Fi, Trigger.ExtRel, State.So_CoOg_Lo, OnRel);
    }

    void FixedUpdate()
    {
        if (!_fistRb.isKinematic && _willAppImp)
        {
            _willAppImp = false;
            _fistRb.AddForce(aim * softExtImpulse, ForceMode2D.Impulse);
        }

        if (playerController.PressingRet) OnSoRet();
    }

    #region Transition Actions
    void OnKept(object input = null)
    {
        fistK.SetActive(false);
        fistPH.SetActive(true);
        _fistJoint.enabled = false;
    }

    void OnRel(object input = null) => bodyJoint.enabled = false;
    void OnSoRel(object input = null)
    {
        bodyJoint.enabled = false;
        _fistJoint.enabled = true;
    }

    void OnHaSoTog(object input = null)
    {
        bodyJoint.maxDistanceOnly = !bodyJoint.maxDistanceOnly;
        _fistJoint.maxDistanceOnly = !_fistJoint.maxDistanceOnly;
    }

    void OnSoEx(object input = null)
    {
        InitLaunch(false);
        _fistRb.isKinematic = false;
        _willAppImp = true;
        _extRout = Ext();
        StartCoroutine(_extRout);
        _fistExt.OnContact = (other) =>
        {
            if (!other.gameObject.CompareTag(Tag.Hookable)) return;
            StopCoroutine(_extRout);
            _extRout = null;
            _fistExt.OnContact = null;
            _fistRb.isKinematic = true;
            bodyJoint.enabled = true;
            bodyJoint.connectedBody = _fistRb;
            bodyJoint.distance = rope.ropeLength;
            _fistJoint.distance = rope.ropeLength;
            ProcessEvent(Trigger.Hooked);
        };

        _fistExt.OnCollision = (col) =>
        {
            StopCoroutine(_extRout);
            _extRout = null;
            _fistExt.OnCollision = null;
            _fistJoint.distance = rope.ropeLength;
            _fistJoint.enabled = true;
            ProcessEvent(Trigger.Contact);
        };
    }

    IEnumerator Ext()
    {
        while (true)
        {
            float ropeLen = (rope.EndPoint.position - rope.StartPoint.position).magnitude;
            rope.ropeLength = Mathf.Min(maxLength, ropeLen);
            // _fistJoint.distance = rope.ropeLength + fistJointExDist;
            rope.linePoints = Math.Clamp(Mathf.CeilToInt(rope.ropeLength / segLength), minSegCount, maxSegCount);
            if (ropeLen >= maxLength - Mathf.Epsilon)
            {
                _fistJoint.distance = rope.ropeLength;
                _fistJoint.enabled = true;
                ProcessEvent(Trigger.Ret);
                _fistExt.OnContact = null;
                yield break;
            }
            yield return null;
        }
    }

    void OnSoRet(object input = null)
    {
        if (_extRout != null)
        {
            StopCoroutine(_extRout);
            _extRout = null;
        }

        // TODO: rope ground intersection considered
        rope.ropeLength = Mathf.Max(0, rope.ropeLength - retractRate * Time.deltaTime);
        rope.linePoints = Math.Clamp(Mathf.CeilToInt(rope.ropeLength / segLength), minSegCount, maxSegCount);
        _fistJoint.distance = rope.ropeLength;
        if (Mathf.Abs(rope.ropeLength) <= Mathf.Epsilon) ProcessEvent(Trigger.Kept);
    }

    void OnFiRet(object input = null)
    {
        bodyJoint.distance = rope.ropeLength = Mathf.Max(0, rope.ropeLength - retractRate * Time.deltaTime);
        rope.linePoints = Math.Clamp(Mathf.CeilToInt(rope.ropeLength / segLength), minSegCount, maxSegCount);
    }

    void HaExLo(object input = null)
    {
        InitLaunch(true);

        _extRout = HaExt();
        StartCoroutine(_extRout);
        _fistExt.OnContact = (other) =>
        {
            if (!other.gameObject.CompareTag(Tag.Hookable)) return;
            StopCoroutine(_extRout);
            _extRout = null;
            bodyJoint.enabled = true;
            bodyJoint.connectedBody = _fistRb;
            bodyJoint.distance = rope.ropeLength;
            _fistExt.OnContact = null;
            ProcessEvent(Trigger.Hooked);
        };
        _fistExt.OnCollision = (col) =>
        {
            if (col.otherRigidbody != null && col.otherRigidbody.CompareTag(Tag.Movable)) // NOTE: Movable when collide against immovable should change tag
                col.otherRigidbody.AddForce((rope.EndPoint.position - rope.StartPoint.position).normalized * hardContImpulse, ForceMode2D.Impulse);
            else
            {
                StopCoroutine(_extRout);
                _extRout = Repel();
                StartCoroutine(_extRout);
                bodyJoint.enabled = true;
                bodyJoint.connectedBody = _fistRb;
                bodyJoint.distance = rope.ropeLength;
                bodyExt.OnCollision = (bcol) =>
                {
                    StopCoroutine(_extRout);
                    _extRout = null;
                    bodyExt.OnCollision = null;
                    ProcessEvent(Trigger.Ret);
                };
                _fistExt.OnCollision = null;
                ProcessEvent(Trigger.Contact);
            }
        };
    }

    IEnumerator Repel()
    {
        float etime = 0;
        while (rope.ropeLength < maxLength - Mathf.Epsilon)
        {
            etime += Time.deltaTime;
            bodyJoint.distance += hardExtRate * Time.deltaTime;
            rope.ropeLength = Mathf.Min(maxLength, (rope.EndPoint.position - rope.StartPoint.position).magnitude);
            rope.linePoints = Math.Clamp(Mathf.CeilToInt(rope.ropeLength / segLength), minSegCount, maxSegCount);
        }
        ProcessEvent(Trigger.Ret);
        yield return null;
    }

    IEnumerator HaExt()
    {
        float etime = 0;
        while (rope.ropeLength < maxLength - Mathf.Epsilon)
        {
            etime += Time.deltaTime;
            rope.ropeLength = Mathf.Min(maxLength, rope.ropeLength + hardExtRate * Time.deltaTime);
            rope.linePoints = Math.Clamp(Mathf.CeilToInt(rope.ropeLength / segLength), minSegCount, maxSegCount);
            rope.EndPoint.position = rope.StartPoint.position + (rope.EndPoint.position - rope.StartPoint.position).normalized * rope.ropeLength;
        }
        ProcessEvent(Trigger.Ret);
        _fistExt.OnContact = null;
        yield return null;
    }

    void OnHaRet(object input = null)
    {
        rope.ropeLength = Mathf.Max(0, rope.ropeLength - retractRate * Time.deltaTime);
        rope.linePoints = Math.Clamp(Mathf.CeilToInt(rope.ropeLength / segLength), minSegCount, maxSegCount);
        rope.EndPoint.position = rope.StartPoint.position + (rope.EndPoint.position - rope.StartPoint.position).normalized * rope.ropeLength;
        if (Mathf.Abs(rope.ropeLength) <= Mathf.Epsilon) ProcessEvent(Trigger.Kept);
    }

    void HaLoAim(object input = null)
    {
        Vector3 caim = rope.EndPoint.position - rope.StartPoint.position;
        Vector3 naim = Quaternion.Euler(0, 0, Vector2.SignedAngle(caim, aim) * aimResp.value * Time.deltaTime) * caim;
        if (!Physics2D.Raycast(rope.StartPoint.position, naim, Abyss.Settings.LayerMask.OBSTACLE_LMASK)) rope.EndPoint.position = rope.StartPoint.position + naim;
    }
    #endregion

    #region Helpers
    void AddTransition(State start, Trigger trigger, State next, Action<object> action = null, bool bidir = false) => _fsm.AddTransition(new((int)start, (int)trigger, (int)next, action), bidir);
    void InitLaunch(bool isKine)
    {
        rope.enabled = true;
        ropeRenderer.enabled = true;
        fistK.SetActive(true);
        // _activeFist = isKine ? fistK : fistD;
        // _activeFist.SetActive(true);
        // _activeFist.transform.position = fistPH.transform.position;
        fistK.transform.position = fistPH.transform.position;
        fistPH.SetActive(false);
        // _fistJoint = fistK.GetComponent<DistanceJoint2D>();
        // _fistRb = fistK.GetComponent<Rigidbody2D>();
        // _fistExt = fistK.GetComponent<ColNotifier>();
    }
    public void ProcessEvent(Trigger trigger) => _fsm.ProcessEvent((int)trigger);
    public void SetAim() => aim = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - rope.StartPoint.position).normalized;
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)aim);
    }
#endif
    #endregion
}
