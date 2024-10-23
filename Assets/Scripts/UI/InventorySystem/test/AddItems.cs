using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddItems : MonoBehaviour
{
    [Tooltip("Press B to submit")]
    public List<Item> items;

    public int quantity = 1;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            PutItem();
        }
    }

    public void PutItem()
    {
        foreach (Item item in items) GameManager.Instance.Inventory.Add(item, quantity);
    }
}