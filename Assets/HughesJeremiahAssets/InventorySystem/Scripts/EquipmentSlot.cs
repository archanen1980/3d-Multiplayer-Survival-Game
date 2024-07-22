using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IPointerClickHandler
{
    public ItemType slotType;  // The type of item this slot can hold
    public Image icon;
    private EquipmentItem currentItem;

    private void Awake()
    {
        // Dynamically find the icon GameObject in the children
        icon = transform.GetChild(0).GetComponent<Image>(); // Adjust the index as needed
        if (icon == null)
        {
            Debug.LogError("Icon Image not found in EquipmentSlot's children.");
        }
    }

    private void Start()
    {
        ClearSlot();
    }

    public void EquipItem(EquipmentItem item)
    {
        if (item.itemType == slotType)
        {
            currentItem = item;
            icon.sprite = item.icon;
            icon.enabled = true;
        }
        else
        {
            Debug.LogWarning($"Item {item.itemName} cannot be equipped in {slotType} slot.");
        }
    }

    public EquipmentItem GetEquippedItem()
    {
        return currentItem;
    }

    public void ClearSlot()
    {
        currentItem = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public bool CanEquipItem(EquipmentItem item)
    {
        return item.itemType == slotType;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && currentItem != null)
        {
            // Start dragging the item
            DraggedItem.Instance.SetDraggedItem(currentItem, 1);
            ClearSlot();
        }
    }
}
