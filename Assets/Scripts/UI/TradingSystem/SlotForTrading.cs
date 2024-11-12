using Abyss.EventSystem;
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


    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;
    private EventTrigger hoverEventTrigger;

    public Canvas canvas;
    private Vector2 originalPosition;
    public GameObject SlotDragPrefab;
    private GameObject draggingSlot;

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
    }

    public void ShowTooltip()
    {
        if (tooltipPanel != null && tooltipText != null && itemStack != null)
        {
            tooltipPanel.SetActive(true);
            // need to change
            tooltipText.text = itemStack.Data.itemName + ": " + itemStack.Data.description;
            //
        }
    }

    public void CloseTooltip()
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        hoverEventTrigger = GetComponent<EventTrigger>();
        if (hoverEventTrigger != null)
        {
            hoverEventTrigger.enabled = false;
        }
        CloseTooltip();

        originalPosition = eventData.position;

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
        if (hoverEventTrigger != null)
        {
            hoverEventTrigger.enabled = true;
        }

        Vector2 destinationPosition = eventData.position;

        DragEventArgs dragArgs = new DragEventArgs(originalPosition, destinationPosition, itemStack.Data);
        EventManager.InvokeEvent(UIEvents.DraggedItem, dragArgs);

        Destroy(draggingSlot);

    }


}

public class DragEventArgs
{
    public Vector2 OriginalPosition { get; }
    public Vector2 DestinationPosition { get; }
    public Item Item { get; }

    public DragEventArgs(Vector2 originalPosition, Vector2 destinationPosition, Item item)
    {
        OriginalPosition = originalPosition;
        DestinationPosition = destinationPosition;
        Item = item;
    }
}

