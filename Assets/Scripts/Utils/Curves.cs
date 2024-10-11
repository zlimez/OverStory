using UnityEditor.PackageManager;
using UnityEngine;

namespace Utils
{
    public class Curves
    {
        public static float GetGradient(AnimationCurve curve, float t, float deltaT = 0.01f)
        {
            float valueAtT = curve.Evaluate(t);
            float valueAtTPlusDelta = curve.Evaluate(t + deltaT);

            // Calculate the gradient (difference in y-values over difference in time)
            float gradient = (valueAtTPlusDelta - valueAtT) / deltaT;

            return gradient;
        }


        public static Vector3 CubicLerpVector(Vector3 from, Vector3 to, float t)
        {
            float clampedT = Mathf.Clamp(t, 0, 1);
            return Vector3.Lerp(from, to, 1 + Mathf.Pow(clampedT - 1, 3));
        }

        public static Vector3 SinLerpVector(Vector3 center, Vector3 amplitude, float t)
        {
            float clampedT = Mathf.Clamp(t, 0, 1);
            return Mathf.Sin(clampedT * 2 * Mathf.PI) * amplitude + center;
        }

        public static Vector3 CosLerpVector(Vector3 center, Vector3 amplitude, float t)
        {
            float clampedT = Mathf.Clamp(t, 0, 1);
            return Mathf.Cos(clampedT * 2 * Mathf.PI) * amplitude + center;
        }

        public static float SquareLerpFloat(float start, float end, float t)
        {
            float clampedT = Mathf.Clamp(t, 0, 1);
            return Mathf.Pow(t, 2) * (end - start) + start;
        }

        public static float EaseOutSquare(float start, float end, float t)
        {
            return -(end - start) * t * (t - 2) + start;
        }
    }
}
