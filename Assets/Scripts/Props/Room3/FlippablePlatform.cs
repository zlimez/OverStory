using Abyss;
using Abyss.Environment.Enemy;
using Abyss.EventSystem;
using Abyss.Player;
using UnityEngine;

public class FlippablePlatform : MonoBehaviour
{
    static readonly int _enemyLayerMask = 1 << (int)AbyssSettings.Layers.Enemy;
    [Header("Dynamic Rotation")]
    [SerializeField] Transform cog;
    [SerializeField] AnimationCurve retardAccelCurve;
    [SerializeField] float angularInertia, angularRetardation;
    [SerializeField][Tooltip("Makes the platform act like seesaw ")] float maxSpringAngularAccel;
    [SerializeField][Tooltip("When angular velocity and z-angle are below these thresholds platform will return to default stationery state (Radians)")] float stillAngularVelocityThreshold, stillZAngleThreshold;
    [SerializeField] float playerImpulseDamper = 0.1f, playerVelocityDamper = 0.1f;

    [Header("Hog Interaction")]
    [SerializeField] float raycastRadius = 2f;
    [SerializeField] DynamicEvent hogStunEvent;
    [SerializeField] Transform hogUnderLoc;

    bool _inContact = false;
    GameObject _player;
    float _angularVel = 0;

    bool _hogInContact = false, _isTipping = false;
    float _hogStayTimer = 0;
    GameObject _hog;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _player = other.gameObject;
            Vector2 cog2Cp = _player.GetComponent<PlayerController>().Foot.position - cog.position;
            float angularImpulse = _player.GetComponent<Rigidbody2D>().mass * Vector2.Dot(new Vector2(0, other.relativeVelocity.y), Vector2.Perpendicular(cog2Cp)) * playerVelocityDamper;
            // Debug.Log("Angular impulse: " + angularImpulse);
            _angularVel += angularImpulse / angularInertia;
            _inContact = true;
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (_player != null && _player.GetComponent<PlayerController>().IsJumping)
            {
                Vector2 cog2Cp = _player.GetComponent<PlayerController>().Foot.position - cog.position;
                float angularImpulse = _player.GetComponent<PlayerController>().InitJumpImpulse * playerImpulseDamper * Vector2.Dot(Vector2.down, Vector2.Perpendicular(cog2Cp));
                _angularVel += angularImpulse / angularInertia;
            }
            _inContact = false;
            _player = null;
        }
    }

    void FixedUpdate()
    {
        float regZ = transform.eulerAngles.z > 180 ? 360 - transform.eulerAngles.z : transform.eulerAngles.z;
        float angularAccel = maxSpringAngularAccel * retardAccelCurve.Evaluate(Mathf.Clamp01(regZ / 90)) * (transform.eulerAngles.z > 180 ? 1 : -1);
        if (_inContact)
        {
            Vector2 cog2Cp = _player.GetComponent<PlayerController>().Foot.position - cog.position;
            Rigidbody2D rb = _player.GetComponent<Rigidbody2D>();
            angularAccel += rb.mass * Mathf.Abs(Physics2D.gravity.y) * Vector2.Dot(Vector2.down, Vector2.Perpendicular(cog2Cp)) / angularInertia;
            // NOTE: Hog can only brought to contact with the platform by player
            if (!_hogInContact)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(cog.position, raycastRadius, _enemyLayerMask);
                foreach (Collider2D collider in colliders)
                {
                    if (collider.TryGetComponent<HogBT>(out var hogBT))
                    {
                        _hogInContact = true;
                        EventManager.StartListening(new GameEvent(hogStunEvent.EventName), Flip);
                        break;
                    }
                }
            }
        }

        _angularVel += angularAccel * Time.fixedDeltaTime;
        _angularVel += -Mathf.Sign(_angularVel) * angularRetardation * Time.fixedDeltaTime;
    }

    void Update()
    {
        float regZ = transform.eulerAngles.z > 180 ? 360 - transform.eulerAngles.z : transform.eulerAngles.z;
        if (Mathf.Abs(_angularVel) < stillAngularVelocityThreshold && regZ * Mathf.Deg2Rad < stillZAngleThreshold)
        {
            _angularVel = 0;
            transform.rotation = Quaternion.identity;
        }
        else
            transform.Rotate(Vector3.forward, _angularVel * Mathf.Rad2Deg * Time.deltaTime);
    }

    void Flip(object inpput = null)
    {
        _hog.GetComponent<HogBT>().StopBT();
        _hog.GetComponent<EnemyManager>().Defeat();
    }
}
