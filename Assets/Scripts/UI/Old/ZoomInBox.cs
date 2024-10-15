using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoomInBox : MonoBehaviour
{
    public GameObject zoomInBox;
    public Image itemImage;
    public Text itemName;
    public Text description;
    public Sprite transparentImage;

    public void Show(Item item)
    {
        itemImage.sprite = item.itemImage;
        itemImage.preserveAspect = true;
        description.text = item.description;
        itemName.text = item.itemName;

        zoomInBox.SetActive(true);
    }

    public void Hide()
    {
        itemImage.sprite = transparentImage;
        itemImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        description.text = "";
        itemName.text = "";
    }
}
