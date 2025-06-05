using UnityEngine;
using System;

namespace Utils
{
    public class Const
    {
        public static readonly float EPS = 0.001f;
    }

    public class Color
    {
        public static UnityEngine.Color CubicLerpColor(UnityEngine.Color from, UnityEngine.Color to, float t)
        {
            float clampedT = Mathf.Clamp(t, 0, 1);
            return UnityEngine.Color.LerpUnclamped(from, to, 1 + Mathf.Pow(clampedT - 1, 3));
        }
        public static UnityEngine.Color HexToRGB(string hex, float alpha)
        {
            if (hex.Length != 6)
            {
                Debug.LogError("Invalid hex color: " + hex);
                return UnityEngine.Color.black;
            }

            int r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            int g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            int b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            float rf = r / 255.0f;
            float gf = g / 255.0f;
            float bf = b / 255.0f;

            return new UnityEngine.Color(rf, gf, bf, alpha);
        }
    }


    public class Rotation
    {
        public static Quaternion fullXRotation = Quaternion.Euler(360, 0, 0);
        public static Quaternion fullYRotation = Quaternion.Euler(0, 360, 0);
        public static Quaternion fullZRotation = Quaternion.Euler(0, 0, 360);
        public static Quaternion CubicLerpRotation(Quaternion from, Quaternion to, float t)
        {
            float clampedT = Mathf.Clamp(t, 0, 1);
            return Quaternion.Slerp(from, to, 1 + Mathf.Pow(clampedT - 1, 3));
        }
    }

    public class Curves
    {
        public static float GetGradient(AnimationCurve curve, float t, float deltaT = 0.01f)
        {
            float valueAtT = curve.Evaluate(t);
            float valueAtTPlusDelta = curve.Evaluate(t + deltaT);
            return (valueAtTPlusDelta - valueAtT) / deltaT;
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
            return Mathf.Pow(clampedT, 2) * (end - start) + start;
        }

        public static float EaseOutSquare(float start, float end, float t) => -(end - start) * t * (t - 2) + start;
    }

    namespace Tuples
    {
        [Serializable]
        public struct Pair<U, T>
        {
            public Pair(U head, T tail)
            {
                Head = head;
                Tail = tail;
            }
            public U Head;
            public T Tail;
        }

        [Serializable]
        public struct Triplet<U, T, Z>
        {
            public U Item1;
            public T Item2;
            public Z Item3;
            public Triplet(U item1, T item2, Z item3)
            {
                Item1 = item1;
                Item2 = item2;
                Item3 = item3;
            }
        }

        [Serializable]
        public class RefPair<U, T>
        {
            public RefPair() { }
            public RefPair(U head, T tail)
            {
                Head = head;
                Tail = tail;
            }
            public U Head = default;
            public T Tail = default;
        }

        [Serializable]
        public class RefTriplet<U, T, Z>
        {
            public U Item1 = default;
            public T Item2 = default;
            public Z Item3 = default;

            public RefTriplet() { }

            public RefTriplet(U item1, T item2, Z item3)
            {
                Item1 = item1;
                Item2 = item2;
                Item3 = item3;
            }
        }
    }
}
