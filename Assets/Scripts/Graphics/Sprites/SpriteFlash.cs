using System.Collections.Generic;
using AnyPortrait;
using Tuples;
using UnityEngine;

public class SpriteFlash : MonoBehaviour
{
    [SerializeField] apPortrait portrait;
    [SerializeField] Color flashColor;
    [SerializeField] float flashDuration;
    List<Pair<Material, Color>> _materials = new();

    void Start()
    {
        GetMaterialsFromChildren(portrait.transform);
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
                if (child.TryGetComponent<MeshRenderer>(out var renderer))
                    foreach (var material in renderer.materials)
                        _materials.Add(new(material, material.color));
            }
        }
    }

    public void Flash()
    {
        foreach (var material in _materials)
            material.Head.color = flashColor;
    }

    public void StopFlash()
    {
        foreach (var material in _materials)
            material.Head.color = material.Tail;
    }
}
