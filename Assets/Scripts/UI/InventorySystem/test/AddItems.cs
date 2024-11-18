using Tuples;
using UnityEngine;

public class AddItems : MonoBehaviour
{
    [Tooltip("Press B to submit")]
    public Pair<Item, int>[] itemAndCounts;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            foreach (var ip in itemAndCounts) GameManager.Instance.Inventory.Add(ip.Head, ip.Tail);
    }
}