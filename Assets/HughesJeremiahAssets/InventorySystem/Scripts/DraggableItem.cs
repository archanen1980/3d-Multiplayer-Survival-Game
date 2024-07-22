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

        EquipmentSlot newEquipmentSlot = null;
        InventorySlot newInventorySlot = null;

        if (eventData.pointerEnter != null)
        {
            newEquipmentSlot = eventData.pointerEnter.GetComponentInParent<EquipmentSlot>();
            newInventorySlot = eventData.pointerEnter.GetComponentInParent<InventorySlot>();
        }

        if (newEquipmentSlot != null && DraggedItem.Instance.GetItem() is EquipmentItem)
        {
            EquipmentItem item = DraggedItem.Instance.GetItem() as EquipmentItem;
            if (newEquipmentSlot.CanEquipItem(item))
            {
                newEquipmentSlot.EquipItem(item);
                if (originalInventorySlot != null)
                {
                    originalInventorySlot.ClearSlot();
                }
                else if (originalEquipmentSlot != null)
                {
                    originalEquipmentSlot.ClearSlot();
                }
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
                Debug.LogWarning("Cannot unequip item to the container it creates slots for.");
            }
            else
            {
                if (originalInventorySlot != null)
                {
                    newInventorySlot.AddItem(item, originalInventorySlot.GetItemCount());
                    originalInventorySlot.ClearSlot();
                }
                else if (originalEquipmentSlot != null)
                {
                    newInventorySlot.AddItem(item, 1);
                    originalEquipmentSlot.ClearSlot();
                }
            }
        }
        else if (InventoryManager.instance.useDragAndDropToDelete && !EventSystem.current.IsPointerOverGameObject())
        {
            // Handle deleting items
            if (originalInventorySlot != null)
            {
                originalInventorySlot.ClearSlot();
            }
            else if (originalEquipmentSlot != null)
            {
                originalEquipmentSlot.ClearSlot();
            }
        }

        DraggedItem.Instance.ClearDraggedItem();
        InventoryManager.instance.RefreshUI();
    }
}
