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
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        originalParent = transform.parent;
        DraggedItem.Instance.SetDraggedItem(originalSlot.GetItem(), originalSlot.GetItemCount());
        rectTransform.SetParent(transform.root);
        rectTransform.position = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        rectTransform.SetParent(originalParent);
        rectTransform.localPosition = Vector3.zero;

        InventorySlot newSlot = null;
        if (eventData.pointerEnter != null)
        {
            newSlot = eventData.pointerEnter.GetComponentInParent<InventorySlot>();
        }

        if (newSlot != null && newSlot != originalSlot)
        {
            if (newSlot.GetItem() != null && newSlot.GetItem().itemID == originalSlot.GetItem().itemID && newSlot.GetItemCount() < newSlot.GetItem().maxStackSize)
            {
                // Combine stacks
                int combinedCount = newSlot.GetItemCount() + originalSlot.GetItemCount();
                int maxStackSize = newSlot.GetItem().maxStackSize;

                if (combinedCount <= maxStackSize)
                {
                    Debug.Log("Combining stacks.");
                    newSlot.AddItem(originalSlot.GetItem(), combinedCount);
                    originalSlot.ClearSlot();
                }
                else
                {
                    Debug.Log("Combining stacks with overflow.");
                    newSlot.AddItem(originalSlot.GetItem(), maxStackSize);
                    originalSlot.AddItem(originalSlot.GetItem(), combinedCount - maxStackSize);
                }
            }
            else
            {
                // Swap items
                InventoryItem tempItem = newSlot.GetItem();
                int tempCount = newSlot.GetItemCount();

                if (tempItem != null && tempCount > 0)
                {
                    newSlot.AddItem(originalSlot.GetItem(), originalSlot.GetItemCount());
                    originalSlot.AddItem(tempItem, tempCount);
                }
                else
                {
                    newSlot.AddItem(originalSlot.GetItem(), originalSlot.GetItemCount());
                    originalSlot.ClearSlot();
                }
            }
        }
        else if (InventoryManager.instance.useDragAndDropToDelete && !EventSystem.current.IsPointerOverGameObject())
        {
            // Item dropped outside inventory and not over any UI, delete it
            originalSlot.ClearSlot();
        }

        DraggedItem.Instance.ClearDraggedItem();
        InventoryManager.instance.RefreshUI();
    }
}
