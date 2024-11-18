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
        RectTransform panelRectTransform = infoPanel.GetComponent<RectTransform>();
        Vector3 parentScale = infoPanel.transform.parent.lossyScale;
        panelRectTransform.localScale = new Vector3(
            panelRectTransform.localScale.x / parentScale.x,
            panelRectTransform.localScale.y / parentScale.y,
            panelRectTransform.localScale.z / parentScale.z);

        text.text = name;
        Renderer renderer = transform.GetComponent<Renderer>();
        if (renderer != null)
        {
            Bounds bounds = renderer.bounds;
            Vector3 adjustedPosition = new Vector3(bounds.center.x, bounds.max.y + 0.9f, bounds.center.z);

            panelRectTransform.position = adjustedPosition;
        }
        else
        {
            Vector3 targetPosition = transform.position;
            Vector3 adjustedPosition = new Vector3(targetPosition.x, targetPosition.y + 0.9f, targetPosition.z);
            panelRectTransform.position = adjustedPosition;
            panelRectTransform.anchoredPosition = new Vector2(0, panelRectTransform.anchoredPosition.y);
        }
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
