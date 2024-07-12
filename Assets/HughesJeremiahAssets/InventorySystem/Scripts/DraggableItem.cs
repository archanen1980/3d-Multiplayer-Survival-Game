using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private CanvasGroup canvasGroup;
    private InventorySlot parentSlot;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        canvasGroup.alpha = 0.6f; // Make the item semi-transparent
        canvasGroup.blocksRaycasts = false; // Allow the item to be dragged through other UI elements

        // Set the dragged item data
        parentSlot = GetComponentInParent<InventorySlot>();
        DraggedItem.Instance.SetDraggedItem(parentSlot.GetItem(), parentSlot.GetItemCount());
    }

    public void OnDrag(PointerEventData eventData)
    {
        // The DraggedItem singleton will handle the visual update
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        canvasGroup.alpha = 1f; // Make the item fully opaque again
        canvasGroup.blocksRaycasts = true; // Block raycasts again

        // Check if the item was dropped in a valid slot
        if (eventData.pointerEnter != null)
        {
            InventorySlot newSlot = eventData.pointerEnter.GetComponentInParent<InventorySlot>();
            if (newSlot != null && newSlot != parentSlot)
            {
                newSlot.AddItem(parentSlot.GetItem(), parentSlot.GetItemCount());
                parentSlot.ClearSlot();
            }
        }

        // Clear the dragged item data
        if (DraggedItem.Instance != null)
        {
            DraggedItem.Instance.ClearDraggedItem();
        }

        transform.SetParent(parentSlot.transform); // Move the item back to its original parent
    }
}
