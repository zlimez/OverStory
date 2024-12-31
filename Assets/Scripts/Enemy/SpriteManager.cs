
using System.Collections.Generic;
using Tuples;
using UnityEngine;

/** <summary> Makes required transform operations on root object such that sprite reflects movement </summary> **/
public class SpriteManager : MonoBehaviour
{
    public Vector3 forward;
    readonly List<Pair<Material, Color>> _materials = new();
    void Awake() => GetMaterialsFromChildren(transform);

    public void Face(Vector3 target)
    {
        if ((target.x < transform.position.x && forward.x > 0) || (target.x > transform.position.x && forward.x < 0))
            Flip();
    }

    public void Flip()
    {
        forward.x *= -1;
        transform.localScale = new Vector3(forward.x * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    public void FaceDir(Vector2 dir)
    {
        if ((dir.x < 0 && forward.x > 0) || (dir.x > 0 && forward.x < 0))
            Flip();
    }

    void GetMaterialsFromChildren(Transform parent)
    {
        Queue<Transform> queue = new();
        queue.Enqueue(parent);
        while (queue.Count > 0)
        {
            Transform current = queue.Dequeue();
            foreach (Transform child in current)
            {
                queue.Enqueue(child);
                if (child.TryGetComponent<Renderer>(out var renderer))
                    foreach (var material in renderer.materials)
                        _materials.Add(new(material, material.color));
            }
        }
    }

    public void Appear()
    {
        foreach (var material in _materials)
            material.Head.color = new Color(material.Head.color.r, material.Head.color.g, material.Head.color.b, 1);
    }

    public void Disappear()
    {
        foreach (var material in _materials)
            material.Head.color = new Color(material.Head.color.r, material.Head.color.g, material.Head.color.b, 0);
    }
}
