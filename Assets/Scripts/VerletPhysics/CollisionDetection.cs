using Unity.Collections;
using UnityEngine;

namespace VerletPhysics
{
    public enum C_ColliderType { Circle, Polygon }

    struct C_Collider
    {
        public (Vector2, Vector2) AABB;
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

    public class CollisionDetection
    {
        public static bool PointInAABB(Vector2 point, Vector2 rectMin, Vector2 rectMax)
        {
            return point.x >= rectMin.x && point.x <= rectMax.x &&
                   point.y >= rectMin.y && point.y <= rectMax.y;
        }

        public static bool CircleCollision(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB, out Vector2 normal, out float depth)
        {
            normal = centerA - centerB; // Reverse the direction of the normal
            float sqd = normal.sqrMagnitude;
            float rsum = radiusA + radiusB;

            if (sqd >= rsum * rsum)
            {
                depth = 0;
                return false;
            }

            float distance = Mathf.Sqrt(sqd);
            depth = rsum - distance;
            if (distance > 0) normal /= distance; // Normalize the normal
            return true;
        }

        public static bool SATCheck(Vector2 center, float radius, NativePoly polyB, out Vector2 normal, out float depth)
        {
            normal = Vector2.zero;
            depth = float.MaxValue;

            for (int i = 0; i < polyB.VertexCount; i++)
            {
                Vector2 a = polyB.Vertices[polyB.VertexStart + i];
                Vector2 b = polyB.Vertices[polyB.VertexStart + (i + 1) % polyB.VertexCount];
                Vector2 edge = b - a;
                Vector2 axis = new Vector2(-edge.y, edge.x).normalized;

                ProjectPoly(polyB, axis, out float minB, out float maxB);
                float circleProjection = Vector2.Dot(center, axis);
                float circleMin = circleProjection - radius;
                float circleMax = circleProjection + radius;

                if (maxB < circleMin || circleMax < minB) return false;

                float overlap = Mathf.Min(maxB - circleMin, circleMax - minB);
                if (overlap < depth)
                {
                    depth = overlap;
                    normal = axis * (Vector2.Dot(center - a, axis) < 0 ? -1 : 1);
                }
            }

            Vector2 polygonCenter = GetPolyCenter(polyB);
            Vector2 circleAxis = (center - polygonCenter).normalized;

            ProjectPoly(polyB, circleAxis, out float polyMin, out float polyMax);
            float circleProj = Vector2.Dot(center, circleAxis);
            float cMin = circleProj - radius;
            float cMax = circleProj + radius;

            if (polyMax < cMin || cMax < polyMin) return false;

            float circleOverlap = Mathf.Min(polyMax - cMin, cMax - polyMin);
            if (circleOverlap < depth)
            {
                depth = circleOverlap;
                normal = circleAxis;
            }

            return true;
        }

        private static void ProjectPoly(NativePoly poly, Vector2 axis, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;
            for (int i = 0; i < poly.VertexCount; i++)
            {
                Vector2 v = poly.Vertices[poly.VertexStart + i];
                float projection = Vector2.Dot(v, axis);
                min = Mathf.Min(min, projection);
                max = Mathf.Max(max, projection);
            }
        }

        public static Vector2 GetPolyCenter(NativePoly poly)
        {
            Vector2 center = Vector2.zero;
            for (int i = 0; i < poly.VertexCount; i++) center += poly.Vertices[poly.VertexStart + i];
            return center / poly.VertexCount;
        }
    }
}
