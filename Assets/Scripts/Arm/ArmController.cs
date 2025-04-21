using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmController : MonoBehaviour
{
    public GameObject armSegmentPrefab;
    public Transform startPoint;
    private int initialSegmentCount = 10;
    public float segmentLength = 0.1f;

    private List<GameObject> segments = new List<GameObject>();
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;

        CreateRope();
    }

    void CreateRope()
    {
        Rigidbody2D previousRB = startPoint.GetComponent<Rigidbody2D>();

        for (int i = 0; i < initialSegmentCount; i++)
        {
            Vector2 pos = startPoint.position - new Vector3(0, segmentLength * (i + 1), 0);
            GameObject newSeg = Instantiate(armSegmentPrefab, pos, Quaternion.identity, this.transform);
            segments.Add(newSeg);

            // SpringJoint2D joint = newSeg.GetComponent<SpringJoint2D>();
            DistanceJoint2D joint = newSeg.GetComponent<DistanceJoint2D>();
            joint.connectedBody = previousRB;
            joint.distance = segmentLength;
            // joint.enableCollision = false; 

            previousRB = newSeg.GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        lineRenderer.positionCount = segments.Count + 1;
        lineRenderer.SetPosition(0, startPoint.position);
        for (int i = 0; i < segments.Count; i++)
        {
            lineRenderer.SetPosition(i + 1, segments[i].transform.position);
        }
    }

    public void AddSegment()
    {
        GameObject last = segments[segments.Count - 1];
        Rigidbody2D lastRB = last.GetComponent<Rigidbody2D>();
        Vector2 pos = last.transform.position - new Vector3(0, segmentLength, 0);
        GameObject newSeg = Instantiate(armSegmentPrefab, pos, Quaternion.identity);

        SpringJoint2D joint = newSeg.GetComponent<SpringJoint2D>();
        // DistanceJoint2D joint = newSeg.GetComponent<DistanceJoint2D>();
        joint.connectedBody = lastRB;
        joint.distance = segmentLength;
        joint.enableCollision = true; 

        segments.Add(newSeg);
    }
}
