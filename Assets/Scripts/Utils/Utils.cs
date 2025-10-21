using UnityEngine;
using System;

namespace Utils
{
    public static class Const
    {
        public const float EPS = 0.001f;
    }

    public static class ColorFuncs
    {
        public static Color CubicLerpColor(Color from, Color to, float t)
        {
            var clampedT = Mathf.Clamp(t, 0, 1);
            return Color.LerpUnclamped(from, to, 1 + Mathf.Pow(clampedT - 1, 3));
        }

        public static Color HexToRGB(string hex, float alpha)
        {
            if (hex.Length != 6)
            {
                Debug.LogError("Invalid hex color: " + hex);
                return Color.black;
            }

            var r = int.Parse(hex[..2], System.Globalization.NumberStyles.HexNumber);
            var g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            var b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            float rf = r / 255.0f, gf = g / 255.0f, bf = b / 255.0f;

            return new Color(rf, gf, bf, alpha);
        }
    }


    public class Rotation
    {
        public static Quaternion FullXRotation = Quaternion.Euler(360, 0, 0);
        public static Quaternion FullYRotation = Quaternion.Euler(0, 360, 0);
        public static Quaternion FullZRotation = Quaternion.Euler(0, 0, 360);
        public static Quaternion CubicLerpRotation(Quaternion from, Quaternion to, float t)
        {
            var clampedT = Mathf.Clamp(t, 0, 1);
            return Quaternion.Slerp(from, to, 1 + Mathf.Pow(clampedT - 1, 3));
        }
    }

    public static class Curves
    {
        public static float GetGradient(AnimationCurve curve, float t, float deltaT = 0.01f)
        {
            var valueAtT = curve.Evaluate(t);
            var valueAtTPlusDelta = curve.Evaluate(t + deltaT);
            return (valueAtTPlusDelta - valueAtT) / deltaT;
        }


        public static Vector3 CubicLerpVector(Vector3 from, Vector3 to, float t)
        {
            var clampedT = Mathf.Clamp(t, 0, 1);
            return Vector3.Lerp(from, to, 1 + Mathf.Pow(clampedT - 1, 3));
        }

        public static Vector3 SinLerpVector(Vector3 center, Vector3 amplitude, float t)
        {
            var clampedT = Mathf.Clamp(t, 0, 1);
            return Mathf.Sin(clampedT * 2 * Mathf.PI) * amplitude + center;
        }

        public static Vector3 CosLerpVector(Vector3 center, Vector3 amplitude, float t)
        {
            var clampedT = Mathf.Clamp(t, 0, 1);
            return Mathf.Cos(clampedT * 2 * Mathf.PI) * amplitude + center;
        }

        public static float SquareLerpFloat(float start, float end, float t)
        {
            var clampedT = Mathf.Clamp(t, 0, 1);
            return Mathf.Pow(clampedT, 2) * (end - start) + start;
        }

        public static float EaseOutSquare(float start, float end, float t) => -(end - start) * t * (t - 2) + start;
    }

    namespace Tuples
    {
        [Serializable]
        public struct Pair<T1, T2>
        {
            public Pair(T1 head, T2 tail)
            {
                Head = head;
                Tail = tail;
            }
            public T1 Head;
            public T2 Tail;
        }

        [Serializable]
        public struct Triplet<T1, T2, T3>
        {
            public T1 Item1;
            public T2 Item2;
            public T3 Item3;
            public Triplet(T1 item1, T2 item2, T3 item3)
            {
                Item1 = item1;
                Item2 = item2;
                Item3 = item3;
            }
        }

        [Serializable]
        public class RefPair<T1, T2>
        {
            public RefPair() { }
            public RefPair(T1 head, T2 tail)
            {
                Head = head;
                Tail = tail;
            }
            public T1 Head;
            public T2 Tail;
        }

        [Serializable]
        public class RefTriplet<T1, T2, T3>
        {
            public T1 Item1;
            public T2 Item2;
            public T3 Item3;

            public RefTriplet() { }

            public RefTriplet(T1 item1, T2 item2, T3 item3)
            {
                Item1 = item1;
                Item2 = item2;
                Item3 = item3;
            }
        }
    }
}
