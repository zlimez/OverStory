using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WindmillRotator : MonoBehaviour
{
    [SerializeField] private float peakRotationSpeed;
    public float PeakRotationSpeed { get { return peakRotationSpeed; } }
    [SerializeField] private float rotationAcceleration;
    // + for anticlockwise - for clockwise
    public float CurrRotationVel { get; private set; }
    private bool isStopped = true;
    public bool IsRotating => Mathf.Abs(CurrRotationVel) > 0;
    public bool IsClockwise { get; private set; }
    public bool IsRotatingClockwise => CurrRotationVel < 0;
    private bool isPowerOffSeq = false;
    private IEnumerator powerDownRoutine;
    public event Action OnRotatingClockwise;
    public event Action OnRotatingAnticlockwise;
    public event Action OnStopped;

    public void PowerOn()
    {
        this.enabled = true;
        if (powerDownRoutine != null) StopCoroutine(powerDownRoutine);
        powerDownRoutine = PowerDown();
        isPowerOffSeq = false;
        isStopped = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPowerOffSeq) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            IsClockwise = false;
            isStopped = false;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {

            IsClockwise = true;
            isStopped = false;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            isStopped = true;
        }

        if (isStopped)
        {
            if (CurrRotationVel == 0) return;
            CurrRotationVel = Mathf.Sign(CurrRotationVel) * Mathf.Max(0, Mathf.Abs(CurrRotationVel) - rotationAcceleration * Time.deltaTime);
            if (CurrRotationVel == 0) OnStopped?.Invoke();
        }
        else
        {
            if (IsClockwise)
            {
                bool WasClockwise = IsRotatingClockwise && CurrRotationVel != 0;
                CurrRotationVel -= rotationAcceleration * Time.deltaTime;
                if (Mathf.Abs(CurrRotationVel) <= 0.5)
                {
                    Debug.Log("Small rotation velocity " + CurrRotationVel);
                }
                if (!WasClockwise && IsRotatingClockwise)
                {
                    OnRotatingClockwise?.Invoke();
                    Debug.Log("Starting anticlockwise rotation");
                }
            }
            else
            {
                bool WasRotatingAnticlockwise = !IsRotatingClockwise && CurrRotationVel != 0;
                CurrRotationVel += rotationAcceleration * Time.deltaTime;
                if (Mathf.Abs(CurrRotationVel) <= 0.5)
                {
                    Debug.Log("Small rotation velocity " + CurrRotationVel);
                }
                bool IsRotatingAntiClockwise = !IsRotatingClockwise && CurrRotationVel != 0;
                if (!WasRotatingAnticlockwise && !IsRotatingClockwise)
                {
                    OnRotatingAnticlockwise?.Invoke();
                    Debug.Log("Starting anticlockwise rotation");
                }
            }
        }

        CurrRotationVel = Mathf.Clamp(CurrRotationVel, -peakRotationSpeed, peakRotationSpeed);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.rotation.eulerAngles.z + CurrRotationVel * Time.deltaTime);
    }

    public void Reset()
    {
        isStopped = true;
        CurrRotationVel = 0;
        isPowerOffSeq = false;
        if (powerDownRoutine != null) StopCoroutine(powerDownRoutine);
    }

    public void StartPowerDown()
    {
        isPowerOffSeq = true;
        isStopped = true;
        StartCoroutine(powerDownRoutine);
    }

    // When disengaged
    IEnumerator PowerDown()
    {
        while (Mathf.Abs(CurrRotationVel) > 0)
        {
            CurrRotationVel = Mathf.Sign(CurrRotationVel) * Mathf.Max(0, Mathf.Abs(CurrRotationVel) - rotationAcceleration * Time.deltaTime);
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.rotation.eulerAngles.z + CurrRotationVel * Time.deltaTime);
            yield return null;
        }
        powerDownRoutine = null;
        this.enabled = false;
    }
}
