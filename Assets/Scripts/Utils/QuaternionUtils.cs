using UnityEngine;

public class QuaternionUtils
{
    public static Quaternion fullXRotation = Quaternion.Euler(360, 0, 0);
    public static Quaternion fullYRotation = Quaternion.Euler(0, 360, 0);
    public static Quaternion fullZRotation = Quaternion.Euler(0, 0, 360);
    // using cubic function
    public static Quaternion CubicLerpRotation(Quaternion from, Quaternion to, float t) {
        float clampedT = Mathf.Clamp(t, 0, 1);
        return Quaternion.Slerp(from, to, 1 + Mathf.Pow(clampedT - 1, 3));
    }
}
