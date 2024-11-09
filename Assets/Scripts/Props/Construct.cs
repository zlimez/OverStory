using System.Collections;
using Tuples;
using UnityEngine;

public class Construct : MonoBehaviour
{
    public ConstructionSystem ConstructionSys;
    [SerializeField] int durability = 3;
    [SerializeField][Tooltip("Bounds used to spawn disappear animation")] Transform topRight, bottomLeft;
    [SerializeField] Pair<int, int> particlesNumRange;
    [SerializeField] Pair<float, float> particlesScaleRange, delayRange;
    [SerializeField] GameObject particlePrefab;
    int partsSpawned = 0;

    public void TakeDmg()
    {
        durability--;
        if (durability == 0)
            SpawnParticles();
    }

    void SpawnParticles()
    {
        partsSpawned = Random.Range(particlesNumRange.Head, particlesNumRange.Tail);
        for (int i = 0; i < partsSpawned; i++)
            StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(Random.Range(delayRange.Head, delayRange.Tail));
        Vector3 pos = new(Random.Range(bottomLeft.position.x, topRight.position.x), Random.Range(bottomLeft.position.y, topRight.position.y), 0);
        GameObject particle = Instantiate(particlePrefab, pos, Quaternion.Euler(0, 0, Random.Range(0, 360)));
        float uniScale = Random.Range(particlesScaleRange.Head, particlesScaleRange.Tail);
        particle.transform.localScale = new Vector3(uniScale, uniScale, uniScale);
        particle.transform.SetParent(transform);
        particle.GetComponent<DestroyOnAnimEnd>().OnAnimEnd += OnPartDestroyed;
    }

    void OnPartDestroyed()
    {
        partsSpawned--;
        ConstructionSys.IsBuilt = false;
        if (partsSpawned == 0)
            Destroy(gameObject);
    }
}
