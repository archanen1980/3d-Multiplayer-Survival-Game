using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class EquipmentSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public ItemType slotType;  // The type of item this slot can hold
    public Image icon;
    private EquipmentItem currentItem;

    [SerializeField] public ItemContainer associatedContainer; // Reference to the associated ItemContainer
    private int additionalSlots = 0; // Track additional slots provided by the equipped item

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

            if (associatedContainer != null)
            {
                additionalSlots = item.slotsToAdd;
                associatedContainer.SetSlots(additionalSlots);
            }
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
        if (currentItem != null && associatedContainer != null)
        {
            List<InventorySlot> excessSlots = new List<InventorySlot>();

            // Collect excess slots
            for (int i = associatedContainer.minSlotCount; i < associatedContainer.slots.Count; i++)
            {
                InventorySlot slot = associatedContainer.slots[i];
                if (slot.GetItem() != null)
                {
                    excessSlots.Add(slot);
                }
            }

            // Move items from excess slots to minimum slots
            foreach (var slot in excessSlots)
            {
                InventoryItem item = slot.GetItem();
                int itemCount = slot.GetItemCount();
                int remainingItems = MoveItemToMinSlotsWithinContainer(associatedContainer, item, itemCount);

                if (remainingItems > 0)
                {
                    slot.AddItem(item, remainingItems); // Keep remaining items in the slot if they couldn't be moved
                }
                else
                {
                    slot.ClearSlot(); // Clear the slot if all items were moved
                }
            }

            // Move remaining items in excess slots to other containers
            foreach (var slot in excessSlots)
            {
                if (slot.GetItem() != null)
                {
                    InventoryItem item = slot.GetItem();
                    int itemCount = slot.GetItemCount();
                    int remainingItems = MoveItemToAvailableContainers(item, itemCount);

                    if (remainingItems > 0)
                    {
                        slot.AddItem(item, remainingItems); // Keep remaining items in the slot if they couldn't be moved
                    }
                    else
                    {
                        slot.ClearSlot(); // Clear the slot if all items were moved
                    }
                }
            }

            additionalSlots = 0;
            associatedContainer.SetSlots(additionalSlots);
        }

        currentItem = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    private int MoveItemToAvailableContainers(InventoryItem item, int count)
    {
        int remainingCount = count;

        foreach (var container in InventoryManager.instance.containers)
        {
            if (container != associatedContainer) // Avoid moving items back to the associated container
            {
                remainingCount = container.AddItem(item, remainingCount);

                if (remainingCount <= 0)
                {
                    return 0; // All items were moved successfully
                }
            }
        }

        return remainingCount; // Return remaining items if not all could be moved
    }

    public bool CanEquipItem(EquipmentItem item)
    {
        return item.itemType == slotType;
    }

    private int MoveItemToMinSlotsWithinContainer(ItemContainer container, InventoryItem item, int count)
    {
        int remainingCount = count;

        for (int i = 0; i < container.minSlotCount; i++)
        {
            InventorySlot slot = container.slots[i];

            if (slot.GetItem() == null || (slot.GetItem().itemID == item.itemID && item.isStackable))
            {
                int spaceLeft = item.maxStackSize - slot.GetItemCount();
                if (spaceLeft > 0)
                {
                    int itemsToAdd = Mathf.Min(spaceLeft, remainingCount);
                    slot.AddItem(item, slot.GetItemCount() + itemsToAdd);
                    remainingCount -= itemsToAdd;

                    if (remainingCount <= 0)
                    {
                        return 0; // All items were moved successfully
                    }
                }
            }
        }

        return remainingCount; // Return remaining items if not all could be moved
    }

    public bool CanFitInContainer(InventoryItem item, int count)
    {
        int remainingCount = count;

        // Check available space in the container
        foreach (var slot in associatedContainer.slots)
        {
            if (slot.GetItem() == null || (slot.GetItem().itemID == item.itemID && item.isStackable))
            {
                int spaceLeft = item.maxStackSize - slot.GetItemCount();
                if (spaceLeft > 0)
                {
                    int itemsToAdd = Mathf.Min(spaceLeft, remainingCount);
                    remainingCount -= itemsToAdd;

                    if (remainingCount <= 0)
                    {
                        return true; // Enough space found
                    }
                }
            }
        }

        return false; // Not enough space available
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem != null)
        {
            ItemTooltip.Instance.ShowTooltip(currentItem, icon.rectTransform);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltip.Instance.HideTooltip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && InventoryManager.instance.useContextMenuToDelete && currentItem != null)
        {
            ContextMenu.Instance.ShowContextMenu(this);
        }
    }
}
