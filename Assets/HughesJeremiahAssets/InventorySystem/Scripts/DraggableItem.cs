using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private InventorySlot originalInventorySlot;
    private EquipmentSlot originalEquipmentSlot;
    private RectTransform rectTransform;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        originalParent = transform.parent;

        originalInventorySlot = GetComponentInParent<InventorySlot>();
        originalEquipmentSlot = GetComponentInParent<EquipmentSlot>();

        if (originalInventorySlot != null)
        {
            DraggedItem.Instance.SetDraggedItem(originalInventorySlot.GetItem(), originalInventorySlot.GetItemCount());
        }
        else if (originalEquipmentSlot != null)
        {
            DraggedItem.Instance.SetDraggedItem(originalEquipmentSlot.GetEquippedItem(), 1);
        }

        rectTransform.SetParent(transform.root);
        DraggedItem.Instance.gameObject.SetActive(true);
        DraggedItem.Instance.icon.transform.position = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        DraggedItem.Instance.icon.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        InventorySlot newInventorySlot = null;
        EquipmentSlot newEquipmentSlot = null;

        if (eventData.pointerEnter != null)
        {
            newInventorySlot = eventData.pointerEnter.GetComponentInParent<InventorySlot>();
            newEquipmentSlot = eventData.pointerEnter.GetComponentInParent<EquipmentSlot>();
        }

        if (newInventorySlot != null)
        {
            HandleInventorySlotInteraction(newInventorySlot);
        }
        else if (newEquipmentSlot != null)
        {
            HandleEquipmentSlotInteraction(newEquipmentSlot);
        }

        DraggedItem.Instance.ClearDraggedItem();
        rectTransform.SetParent(originalParent); // Reset parent to original
        rectTransform.localPosition = Vector3.zero; // Reset position
        DraggedItem.Instance.gameObject.SetActive(false);
        InventoryManager.instance.RefreshUI();
    }

    private void HandleInventorySlotInteraction(InventorySlot newSlot)
    {
        if (newSlot == null)
        {
            Debug.LogError("newSlot is null in HandleInventorySlotInteraction.");
            return;
        }

        InventoryItem newItem = DraggedItem.Instance.GetItem();
        if (newItem == null)
        {
            Debug.LogError("Dragged item is null or not an InventoryItem.");
            return;
        }

        InventoryItem currentItem = newSlot.GetItem();

        if (currentItem != null && currentItem.itemID == newItem.itemID)
        {
            if (newItem.isStackable)
            {
                // Stack items
                int combinedCount = newSlot.GetItemCount() + DraggedItem.Instance.GetCount();
                int maxStackSize = newItem.maxStackSize;

                if (combinedCount <= maxStackSize)
                {
                    newSlot.AddItem(newItem, combinedCount);
                    ClearOriginalSlot(); // Call without parameters
                }
                else
                {
                    newSlot.AddItem(newItem, maxStackSize);
                    if (originalInventorySlot != null)
                    {
                        originalInventorySlot.AddItem(newItem, combinedCount - maxStackSize);
                    }
                    else
                    {
                        Debug.LogError("originalInventorySlot is null while stacking items.");
                    }
                }
            }
            else
            {
                // Swap items
                SwapItems(newSlot, originalInventorySlot);
            }
        }
        else
        {
            // Place in empty slot or swap with a different item
            if (newSlot.GetItem() == null)
            {
                newSlot.AddItem(newItem, DraggedItem.Instance.GetCount());
                ClearOriginalSlot(); // Call without parameters
            }
            else
            {
                SwapItems(newSlot, originalInventorySlot);
            }
        }
    }

    private void HandleEquipmentSlotInteraction(EquipmentSlot newSlot)
    {
        if (newSlot == null)
        {
            Debug.LogError("newSlot is null in HandleEquipmentSlotInteraction.");
            return;
        }

        EquipmentItem newItem = DraggedItem.Instance.GetItem() as EquipmentItem;
        if (newItem == null)
        {
            Debug.LogError("Dragged item is null or not an EquipmentItem.");
            return;
        }

        // Check if the new item can be equipped in the slot
        if (newItem.itemType != newSlot.slotType)
        {
            Debug.LogWarning($"Item {newItem.itemName} of type {newItem.itemType} cannot be equipped in slot of type {newSlot.slotType}.");
            return;
        }

        EquipmentItem currentItem = newSlot.GetEquippedItem();

        if (currentItem != null)
        {
            // Prevent swapping if the item types are not compatible
            InventoryItem inventoryItem = DraggedItem.Instance.GetItem();
            if (inventoryItem is EquipmentItem inventoryEquipmentItem)
            {
                if (inventoryEquipmentItem.itemType != newSlot.slotType)
                {
                    Debug.LogWarning($"Cannot swap item {currentItem.itemName} with {newItem.itemName}. Incompatible item types.");
                    return;
                }
            }

            // Swap items
            SwapItems(newSlot, originalEquipmentSlot);
        }
        else
        {
            // Equip in empty slot
            newSlot.EquipItem(newItem);
            ClearOriginalSlot();
        }
    }

    private void ClearOriginalSlot()
    {
        if (originalInventorySlot != null)
        {
            originalInventorySlot.ClearSlot();
        }
        else if (originalEquipmentSlot != null)
        {
            originalEquipmentSlot.ClearSlot();
        }
    }

    private void SwapItems(InventorySlot slot1, InventorySlot slot2)
    {
        if (slot1 == null || slot2 == null)
        {
            //Debug.LogError("Attempted to swap items with a null slot.");
            return;
        }

        InventoryItem item1 = slot1.GetItem();
        int count1 = slot1.GetItemCount();

        InventoryItem item2 = slot2.GetItem();
        int count2 = slot2.GetItemCount();

        slot1.ClearSlot();
        if (item2 != null) slot1.AddItem(item2, count2);

        slot2.ClearSlot();
        if (item1 != null) slot2.AddItem(item1, count1);
    }

    private void SwapItems(EquipmentSlot slot1, EquipmentSlot slot2)
    {
        if (slot1 == null || slot2 == null)
        {
            //Debug.LogError("Attempted to swap items with a null slot.");
            return;
        }

        EquipmentItem tempItem = slot1.GetEquippedItem();

        slot1.EquipItem(slot2.GetEquippedItem());
        slot2.EquipItem(tempItem);
    }
}
