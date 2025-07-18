using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    /// <summary>
    /// Shake Amount.
    /// </summary>
    public Vector3 Amount = new Vector3(1.3f, 1.3f, 0);

    /// <summary>
    /// How long the shake will last.
    /// </summary>
    public float Duration = 1;

    /// <summary>
    /// The Speed of the shake
    /// </summary>
    public float Speed = 10;

    /// <summary>
    /// The amount of the shake over its lifetime
    /// </summary>
    public AnimationCurve Curve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    /// <summary>
    /// Set it to true: The camera position is set in reference to the old position of the camera
    /// Set it to false: The camera position is set in absolute values or is fixed to an object
    /// </summary>
    public bool DeltaMovement = true;

    /// <summary>
    /// Protected values. Id recommend ignoring them if you dont know what you are doing lmao.
    /// </summary>
    protected Camera Camera;
    protected float time = 0;
    protected Vector3 lastPos;
    protected Vector3 nextPos;
    protected float lastFoV;
    protected float nextFoV;
    protected bool destroyAfterPlay;


    /// <summary>
    /// Get the Camera component.
    /// </summary>
    private void Awake()
    {
        Camera = GetComponent<Camera>();
    }

    public static void ShakeOnce(float duration = 1f, float speed = 10f, Vector3? amount = null, Camera camera = null, bool deltaMovement = true, AnimationCurve curve = null)
    {
        //set data
        var instance = ((camera != null) ? camera : Camera.main).gameObject.AddComponent<CameraShake>();
        instance.Duration = duration;
        instance.Speed = speed;
        if (amount != null)
            instance.Amount = (Vector3)amount;
        if (curve != null)
            instance.Curve = curve;
        instance.DeltaMovement = deltaMovement;

        //one time
        instance.destroyAfterPlay = true;
        instance.Shake();
    }

    /// <summary>
    /// Do the shake
    /// </summary>
    public void Shake()
    {
        ResetCam();
        time = Duration;
    }

    private void LateUpdate()
    {
        if (time > 0)
        {
            //start subtracting the time
            time -= Time.deltaTime;
            if (time > 0)
            {
                //perlin noise because perlin noise is epic (and also because its kinda fucking important but who cares)
                nextPos = (Mathf.PerlinNoise(time * Speed, time * Speed * 2) - 0.5f) * Amount.x * transform.right * Curve.Evaluate(1f - time / Duration) +
                          (Mathf.PerlinNoise(time * Speed * 2, time * Speed) - 0.5f) * Amount.y * transform.up * Curve.Evaluate(1f - time / Duration);
                nextFoV = (Mathf.PerlinNoise(time * Speed * 2, time * Speed * 2) - 0.5f) * Amount.z * Curve.Evaluate(1f - time / Duration);

                Camera.fieldOfView += (nextFoV - lastFoV);
                Camera.transform.Translate(DeltaMovement ? (nextPos - lastPos) : nextPos);

                lastPos = nextPos;
                lastFoV = nextFoV;
            }
            else
            {
                //Destroy the component after the last frame
                ResetCam();
                if (destroyAfterPlay)
                    Destroy(this);
            }
        }
    }
    //Clear all values and reset everything to normal.
    private void ResetCam()
    {
        Camera.transform.Translate(DeltaMovement ? -lastPos : Vector3.zero);
        Camera.fieldOfView -= lastFoV;
        //clear le values
        lastPos = nextPos = Vector3.zero;
        lastFoV = nextFoV = 0f;
    }
}