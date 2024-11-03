using System.Collections;
using System.Collections.Generic;
using AnyPortrait;
using Tuples;
using UnityEngine;

public class SpriteFlash : MonoBehaviour
{
    [SerializeField] apPortrait portrait;
    [SerializeField] Color flashColor;
    [SerializeField] float flashDuration;
    [SerializeField] int reps;
    List<Pair<Material, Color>> _materials = new();
    IEnumerator _flashRoutine;

    void Start() => GetMaterialsFromChildren(portrait.transform);

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

    public void StartFlash()
    {
        if (_flashRoutine != null)
            StopCoroutine(_flashRoutine);
        _flashRoutine = Flash();
        StartCoroutine(_flashRoutine);
    }

    IEnumerator Flash()
    {
        for (int i = 0; i < reps; i++)
        {
            foreach (var material in _materials)
                material.Head.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            foreach (var material in _materials)
                material.Head.color = material.Tail;
            yield return new WaitForSeconds(flashDuration);
        }
    }
}
