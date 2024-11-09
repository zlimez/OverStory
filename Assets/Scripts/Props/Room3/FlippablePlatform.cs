using Abyss.Player;
using UnityEngine;

public class FlippablePlatform : MonoBehaviour
{
    [SerializeField] Transform cog;
    [SerializeField] AnimationCurve retardAccelCurve;
    [SerializeField] float angularInertia, angularRetardation;
    [SerializeField][Tooltip("Makes the platform act like seesaw ")] float maxSpringAngularAccel;
    [SerializeField][Tooltip("When angular velocity and z-angle are below these thresholds platform will return to default stationery state (Radians)")] float stillAngularVelocityThreshold, stillZAngleThreshold;
    [SerializeField] float playerImpulseDamper = 0.1f, playerVelocityDamper = 0.1f;
    // [SerializeField] float enterMinImpulse = 8f;
    bool _inContact = false;
    GameObject _player;
    float _angularVel = 0;

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
}
