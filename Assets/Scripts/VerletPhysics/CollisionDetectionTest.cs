using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace VerletPhysics
{
    public class CollisionDetectionTest : MonoBehaviour
    {
        public float radius = 5f;
        public Vector2 center = Vector2.zero;
        NativeArray<C_Collider> n_colliders;
        NativeArray<Vector2> n_col_vertices;

        private void Start()
        {
            List<Collider2D> cols = new();
            Collider2D[] allCols = FindObjectsOfType<Collider2D>();
            foreach (var col in allCols)
                if ((1 << col.gameObject.layer & Abyss.Settings.LayerMask.OBSTACLE_LMASK) > 0) cols.Add(col);
            n_colliders = new NativeArray<C_Collider>(cols.Count, Allocator.Persistent);
            List<Vector2> colVerts = new();
            for (var i = 0; i < cols.Count; i++)
            {
                var col = cols[i];
                Debug.Log($"Collider {i}: {col.name}");
                if (!(col is PolygonCollider2D || col is CircleCollider2D || col is BoxCollider2D))
                {
                    Debug.LogWarning($"Unsupported collider type: {col.GetType()}");
                    continue;
                }
                C_Collider ccol = new()
                {
                    Type = col is CircleCollider2D ? C_ColliderType.Circle : C_ColliderType.Polygon,
                    Center = col.bounds.center,
                    AABB = (col.bounds.min, col.bounds.max)
                };

                if (col is CircleCollider2D circleCol)
                    ccol.Radius = circleCol.radius * Mathf.Max(circleCol.transform.lossyScale.x, circleCol.transform.lossyScale.y);
                else if (col is PolygonCollider2D polyCol)
                {
                    ccol.VertexCount = polyCol.points.Length;
                    foreach (var pt in polyCol.points)
                        colVerts.Add(polyCol.transform.TransformPoint(pt));
                }
                else
                {
                    var boxCol = col as BoxCollider2D;
                    ccol.VertexCount = 4;
                    var extents = boxCol.size * 0.5f;
                    var p1 = boxCol.transform.TransformPoint(boxCol.offset + new Vector2(-extents.x, -extents.y));
                    var p2 = boxCol.transform.TransformPoint(boxCol.offset + new Vector2(extents.x, -extents.y));
                    var p3 = boxCol.transform.TransformPoint(boxCol.offset + new Vector2(extents.x, extents.y));
                    var p4 = boxCol.transform.TransformPoint(boxCol.offset + new Vector2(-extents.x, extents.y));
                    colVerts.Add(p1);
                    colVerts.Add(p2);
                    colVerts.Add(p3);
                    colVerts.Add(p4);
                    Debug.Log($"BoxCollider {col.name} with extents: {extents} {p1}, {p2}, {p3}, {p4} vi from {colVerts.Count - 4} to {colVerts.Count - 1}");
                }
                // Debug.Log($"Collider {i} {col.name} Type: {ccol.Type}, Center: {ccol.Center}, Radius: {ccol.Radius}, VertexCount: {ccol.VertexCount}");
                n_colliders[i] = ccol;
            }
            n_col_vertices = new NativeArray<Vector2>(colVerts.ToArray(), Allocator.Persistent);
        }

        private void Update() => center = Camera.main!.ScreenToWorldPoint(Input.mousePosition);

        private void FixedUpdate()
        {
            var vi = 0;
            foreach (var col in n_colliders)
            {
                Vector2 rectMin = col.AABB.Item1, rectMax = col.AABB.Item2;
                rectMin -= Vector2.one * radius;
                rectMax += Vector2.one * radius;
                if (!CollisionDetection.PointInAABB(center, rectMin, rectMax))
                {
                    if (col.Type == C_ColliderType.Polygon) vi += col.VertexCount;
                    continue;
                }

                float depth = 0; var dn = Vector2.zero;
                var colType = col.Type == C_ColliderType.Circle ? "Circle" : "Polygon";
                if (col.Type == C_ColliderType.Polygon)
                {
                    var hasCol = CollisionDetection.SatCheck(center, radius, new NativePoly() { VertexStart = vi, VertexCount = col.VertexCount, Vertices = n_col_vertices }, out dn, out depth);
                    vi += col.VertexCount;
                    if (!hasCol) continue;
                }
                else if (col.Type == C_ColliderType.Circle && !CollisionDetection.CircleCollision(center, radius, col.Center, col.Radius, out dn, out depth)) continue;
                Debug.Log($"Collision with {colType} collider Depth: {depth}, Normal: {dn}");
            }
        }

        private void OnDestroy()
        {
            if (n_colliders.IsCreated) n_colliders.Dispose();
            if (n_col_vertices.IsCreated) n_col_vertices.Dispose();
        }

        private void OnDrawGizmos()
        {
            // Draw circle around center
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(center, radius);
            // Draw colliders
            var vi = 0;
            Gizmos.color = Color.red;
            foreach (var col in n_colliders)
            {
                var vertices = new Vector2[col.VertexCount];
                if (col.Type == C_ColliderType.Circle)
                    Gizmos.DrawWireSphere(col.Center, col.Radius);
                else
                {
                    for (var i = 0; i < col.VertexCount; i++)
                        vertices[i] = n_col_vertices[vi + i];
                    Gizmos.DrawLine(vertices[0], vertices[^1]);
                    for (var i = 0; i < vertices.Length - 1; i++)
                        Gizmos.DrawLine(vertices[i], vertices[i + 1]);
                    vi += col.VertexCount;
                }
            }
        }
    }
}
