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

        EquipmentSlot newEquipmentSlot = null;
        InventorySlot newInventorySlot = null;

        if (eventData.pointerEnter != null)
        {
            newEquipmentSlot = eventData.pointerEnter.GetComponentInParent<EquipmentSlot>();
            newInventorySlot = eventData.pointerEnter.GetComponentInParent<InventorySlot>();
        }

        // Check if dropped on the original slot
        if ((newInventorySlot != null && newInventorySlot == originalInventorySlot) ||
            (newEquipmentSlot != null && newEquipmentSlot == originalEquipmentSlot))
        {
            // Cancel the drag and return to the original slot
            rectTransform.SetParent(originalParent);
            rectTransform.localPosition = Vector3.zero;
            DraggedItem.Instance.ClearDraggedItem();
            DraggedItem.Instance.gameObject.SetActive(false);
            return;
        }

        rectTransform.SetParent(originalParent);
        rectTransform.localPosition = Vector3.zero;

        if (newEquipmentSlot != null && DraggedItem.Instance.GetItem() is EquipmentItem)
        {
            EquipmentItem item = DraggedItem.Instance.GetItem() as EquipmentItem;
            if (newEquipmentSlot.CanEquipItem(item))
            {
                newEquipmentSlot.EquipItem(item);
                ClearOriginalSlot();
            }
            else
            {
                Debug.LogWarning("Cannot equip item to this slot.");
            }
        }
        else if (newInventorySlot != null && DraggedItem.Instance.GetItem() is InventoryItem)
        {
            InventoryItem item = DraggedItem.Instance.GetItem();
            if (originalEquipmentSlot != null && originalEquipmentSlot.associatedContainer == newInventorySlot.GetComponentInParent<ItemContainer>())
            {
                if (originalEquipmentSlot.associatedContainer.minSlotCount > 0 &&
                    originalEquipmentSlot.associatedContainer.GetItemCount() < originalEquipmentSlot.associatedContainer.minSlotCount &&
                    originalEquipmentSlot.CanFitInContainer(item, 1))
                {
                    newInventorySlot.AddItem(item, 1);
                    originalEquipmentSlot.ClearSlot();
                }
                else
                {
                    Debug.LogWarning("Cannot unequip item to the container it creates slots for.");
                }
            }
            else
            {
                if (originalInventorySlot != null)
                {
                    if (newInventorySlot.GetItem() != null && newInventorySlot.GetItem().itemID == item.itemID && item.isStackable)
                    {
                        int newCount = newInventorySlot.GetItemCount() + DraggedItem.Instance.GetCount();
                        newInventorySlot.AddItem(item, newCount);
                    }
                    else
                    {
                        newInventorySlot.AddItem(item, originalInventorySlot.GetItemCount());
                    }
                    originalInventorySlot.ClearSlot();
                }
                else if (originalEquipmentSlot != null)
                {
                    if (newInventorySlot.GetItem() != null && newInventorySlot.GetItem().itemID == item.itemID && item.isStackable)
                    {
                        int newCount = newInventorySlot.GetItemCount() + DraggedItem.Instance.GetCount();
                        newInventorySlot.AddItem(item, newCount);
                    }
                    else
                    {
                        newInventorySlot.AddItem(item, 1);
                    }
                    originalEquipmentSlot.ClearSlot();
                }
            }
        }
        else if (InventoryManager.instance.useDragAndDropToDelete && !EventSystem.current.IsPointerOverGameObject())
        {
            // Handle deleting items
            ClearOriginalSlot();
        }

        DraggedItem.Instance.ClearDraggedItem();
        InventoryManager.instance.RefreshUI();
        DraggedItem.Instance.gameObject.SetActive(false);
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
}
