using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private InventorySlot originalSlot;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        originalSlot = GetComponentInParent<InventorySlot>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f; // Make the item semi-transparent
        canvasGroup.blocksRaycasts = false; // Allow the item to be dragged through other UI elements
        originalParent = transform.parent; // Remember the original parent
        transform.SetParent(transform.root); // Move the item to the root to avoid clipping
        DraggedItem.Instance.SetDraggedItem(originalSlot.GetItem(), originalSlot.GetItemCount());
        DraggedItem.Instance.transform.position = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        DraggedItem.Instance.transform.position = Input.mousePosition; // Move the item along with the mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f; // Make the item fully opaque again
        canvasGroup.blocksRaycasts = true; // Block raycasts again
        transform.SetParent(originalParent); // Move the item back to its original parent
        transform.localPosition = Vector3.zero; // Reset the item's position

        InventorySlot newSlot = eventData.pointerEnter?.GetComponentInParent<InventorySlot>();
        if (newSlot != null && newSlot != originalSlot)
        {
            ItemContainer originalContainer = originalSlot.GetComponentInParent<ItemContainer>();
            ItemContainer newContainer = newSlot.GetComponentInParent<ItemContainer>();

            if (newContainer != null && originalContainer != newContainer)
            {
                int fromIndex = originalSlot.transform.GetSiblingIndex();
                int toIndex = newSlot.transform.GetSiblingIndex();
                originalContainer.TransferItem(fromIndex, toIndex, newContainer);
            }
            else if (newContainer == originalContainer)
            {
                int fromIndex = originalSlot.transform.GetSiblingIndex();
                int toIndex = newSlot.transform.GetSiblingIndex();
                originalContainer.MoveItem(fromIndex, toIndex);
            }
            originalSlot.ClearSlot();
        }
        DraggedItem.Instance.ClearDraggedItem();
        InventoryManager.instance.inventoryUI.RefreshUI(); // Refresh the UI
    }
}
