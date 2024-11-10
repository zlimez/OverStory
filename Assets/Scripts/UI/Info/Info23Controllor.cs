using System.Collections;
using System.Collections.Generic;
using Abyss.EventSystem;
using TMPro;
using Tuples;
using UnityEngine;
using UnityEngine.UI;

public class Info23Controllor : MonoBehaviour
{
    [SerializeField] GameObject infoPanel;
    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI text;

    public void InitializePanel(string name, Transform transform)
    {
        text.text = name;
        Bounds bounds = transform.GetComponent<Renderer>().bounds;
        Vector3 adjustedPosition = new Vector3(bounds.center.x, bounds.max.y + 0.9f, bounds.center.z);

        RectTransform panelRectTransform = infoPanel.GetComponent<RectTransform>();
        panelRectTransform.position = adjustedPosition;
    }

    public void OpenPanel()
    {
        infoPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        infoPanel.SetActive(false);
    }

}
