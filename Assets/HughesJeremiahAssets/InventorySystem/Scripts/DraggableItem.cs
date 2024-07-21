using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private InventorySlot originalSlot;
    private RectTransform rectTransform;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        originalSlot = GetComponentInParent<InventorySlot>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f; // Make the item semi-transparent
        canvasGroup.blocksRaycasts = false; // Allow the item to be dragged through other UI elements
        originalParent = transform.parent; // Remember the original parent
        DraggedItem.Instance.SetDraggedItem(originalSlot.GetItem(), originalSlot.GetItemCount());
        rectTransform.SetParent(transform.root); // Move the rectTransform to the root to avoid clipping
        rectTransform.position = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = Input.mousePosition; // Move the item along with the mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f; // Make the item fully opaque again
        canvasGroup.blocksRaycasts = true; // Block raycasts again
        rectTransform.SetParent(originalParent); // Move the item back to its original parent
        rectTransform.localPosition = Vector3.zero; // Reset the item's position

        InventorySlot newSlot = null;
        if (eventData.pointerEnter != null)
        {
            newSlot = eventData.pointerEnter.GetComponentInParent<InventorySlot>();
        }

        if (newSlot != null && newSlot != originalSlot)
        {
            newSlot.AddItem(originalSlot.GetItem(), originalSlot.GetItemCount());
            originalSlot.ClearSlot();
        }

        DraggedItem.Instance.ClearDraggedItem();
        InventoryManager.instance.RefreshUI(); // Refresh the UI
    }
}
