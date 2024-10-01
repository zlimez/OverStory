using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddItemButton : MonoBehaviour
{
    [Tooltip("Press B to submit")] public Item item;
    public int quantity;
    public bool isNormal = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            PutItem();
        }
    }

    public void PutItem()
    {
        if (item != null && quantity > 0)
        {
            Inventory.Instance.AddTo(item, quantity);
        }
    }
}
