using Unity.Collections;
using UnityEngine;

namespace VerletPhysics
{
    public enum C_ColliderType { Circle, Polygon }

    struct C_Collider
    {
        public (Vector2 BtmLeft, Vector2 TopRight) AABB;
        public C_ColliderType Type;
        public Vector2 Center;
        public float Radius;
        public int VertexCount;
    }

    public struct NativePoly
    {
        public int VertexStart;
        public int VertexCount;
        public NativeArray<Vector2> Vertices;
    }

    public static class CollisionDetection
    {
        public static bool PointInAABB(Vector2 point, Vector2 rectMin, Vector2 rectMax)
        {
            return point.x >= rectMin.x && point.x <= rectMax.x &&
                   point.y >= rectMin.y && point.y <= rectMax.y;
        }

        public static bool CircleCollision(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB, out Vector2 normal, out float depth)
        {
            normal = centerA - centerB; // Reverse the direction of the normal
            var sqd = normal.sqrMagnitude;
            var radSum = radiusA + radiusB;

            if (sqd >= radSum * radSum)
            {
                depth = 0;
                return false;
            }

            var distance = Mathf.Sqrt(sqd);
            depth = radSum - distance;
            if (distance > 0) normal /= distance; // Normalize the normal
            return true;
        }

        public static bool SatCheck(Vector2 center, float radius, NativePoly polyB, out Vector2 normal, out float depth)
        {
            normal = Vector2.zero;
            depth = float.MaxValue;

            for (var i = 0; i < polyB.VertexCount; i++)
            {
                Vector2 a = polyB.Vertices[polyB.VertexStart + i], b = polyB.Vertices[polyB.VertexStart + (i + 1) % polyB.VertexCount];
                Vector2 edge = b - a, axis = new Vector2(-edge.y, edge.x).normalized;

                ProjectPoly(polyB, axis, out var minB, out var maxB);
                var circleProjection = Vector2.Dot(center, axis);
                float circleMin = circleProjection - radius, circleMax = circleProjection + radius;

                if (maxB < circleMin || circleMax < minB) return false;

                var overlap = Mathf.Min(maxB - circleMin, circleMax - minB);
                if (!(overlap < depth)) continue;
                depth = overlap;
                normal = axis * (Vector2.Dot(center - a, axis) < 0 ? -1 : 1);
            }

            Vector2 polygonCenter = GetPolyCenter(polyB), circleAxis = (center - polygonCenter).normalized;

            ProjectPoly(polyB, circleAxis, out var polyMin, out var polyMax);
            var circleProj = Vector2.Dot(center, circleAxis);
            float cMin = circleProj - radius, cMax = circleProj + radius;

            if (polyMax < cMin || cMax < polyMin) return false;

            var circleOverlap = Mathf.Min(polyMax - cMin, cMax - polyMin);
            if (!(circleOverlap < depth)) return true;
            depth = circleOverlap;
            normal = circleAxis;

            return true;
        }

        private static void ProjectPoly(NativePoly poly, Vector2 axis, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;
            for (var i = 0; i < poly.VertexCount; i++)
            {
                var v = poly.Vertices[poly.VertexStart + i];
                var projection = Vector2.Dot(v, axis);
                min = Mathf.Min(min, projection);
                max = Mathf.Max(max, projection);
            }
        }

        private static Vector2 GetPolyCenter(NativePoly poly)
        {
            var center = Vector2.zero;
            for (var i = 0; i < poly.VertexCount; i++) center += poly.Vertices[poly.VertexStart + i];
            return center / poly.VertexCount;
        }
    }
}
