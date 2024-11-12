using Tuples;
using UnityEngine;

/// <summary>
/// Controls which construction post is activated, the highest tier the player can build, upgrade tree in a sense
/// </summary>
public class ConstructionControl : MonoBehaviour
{
    public Pair<ConstructionItem, GameObject> Tier1ItemPost;
    public Pair<ConstructionItem, GameObject> Tier2ItemPost;
    public Pair<ConstructionItem, GameObject> Tier3ItemPost;

    void OnTriggerEnter2D(Collider2D collider)
    {
        // Tier3ItemPost.Tail.SetActive(false);
        if (collider.CompareTag("Player"))
        {
            // if (Tier3ItemPost.Head != null && GameManager.Instance.Inventory.MaterialCollection.Contains(Tier3ItemPost.Head))
            //     Tier3ItemPost.Tail.SetActive(true);
            if (GameManager.Instance.Inventory.MaterialCollection.Contains(Tier2ItemPost.Head))
            {
                Tier1ItemPost.Tail.SetActive(false);
                Tier2ItemPost.Tail.SetActive(true);
            }
            else if (GameManager.Instance.Inventory.MaterialCollection.Contains(Tier1ItemPost.Head))
            {
                Tier1ItemPost.Tail.SetActive(true);
                Tier2ItemPost.Tail.SetActive(false);
            }
        }
    }
}
