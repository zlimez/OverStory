using TMPro;
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


        text.text = name;
        Renderer renderer = transform.GetComponent<Renderer>();
        if (renderer != null)
        {
            Bounds bounds = renderer.bounds;
            Vector3 adjustedPosition = new Vector3(bounds.center.x, bounds.max.y + 0.9f, bounds.center.z);

            RectTransform panelRectTransform = infoPanel.GetComponent<RectTransform>();
            panelRectTransform.position = adjustedPosition;
        }
        else
        {
            Vector3 targetPosition = transform.position;
            Vector3 adjustedPosition = new Vector3(targetPosition.x, targetPosition.y + 0.9f, targetPosition.z);
            RectTransform panelRectTransform = infoPanel.GetComponent<RectTransform>();
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
