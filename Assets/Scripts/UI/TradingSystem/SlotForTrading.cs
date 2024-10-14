using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotForTrading : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Countable<Item> itemStack;
    public Image itemIcon;
    public TextMeshProUGUI itemCount;


    public GameObject tooltipPanel; // Reference to the tooltip panel
    public TextMeshProUGUI tooltipText;

    public Canvas canvas;
    private Transform originalParent;
    public GameObject SlotDragPrefab;
    private GameObject draggingSlot;

    [System.Serializable]
    public class DragEvent : UnityEvent<Transform, Transform, Item> { }
    public DragEvent onItemDragged;

    public Button slotButton;

    public void InitializeSlot(Countable<Item> newItem)
    {
        itemStack = newItem;
        if (itemStack != null)
        {
            itemIcon.sprite = itemStack.Data.icon;
            itemCount.text = itemStack.Count.ToString();
        }
    }
    void Start()
    {
        CloseTooltip();
        if (onItemDragged == null)
            onItemDragged = new DragEvent();
    }

    public void ShowTooltip()
    {
        tooltipPanel.SetActive(true);
        tooltipText.text = itemStack.Data.description;
    }

    public void CloseTooltip()
    {
        tooltipPanel.SetActive(false);
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;

        draggingSlot = Instantiate(SlotDragPrefab, canvas.transform, true);
        draggingSlot.transform.position = transform.position;
        draggingSlot.transform.localScale = Vector3.one;


        Transform nestedObject = draggingSlot.transform.Find("ItemIcon");
        if (nestedObject != null)
        {
            Image nestedImage = nestedObject.GetComponent<Image>();
            nestedImage.sprite = itemStack.Data.icon;
        }


    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggingSlot != null)
        {
            draggingSlot.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Transform destinationParent = originalParent; 

        // onItemDragged.Invoke(originalParent, destinationParent, itemStack.Data);

        Destroy(draggingSlot);
        
    }


}
