using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public int slotCount; // Number of slots in the container
    public List<InventorySlot> slots = new List<InventorySlot>();

    public void InitializeContainer()
    {
        slots = new List<InventorySlot>(GetComponentsInChildren<InventorySlot>());

        if (slots.Count != slotCount)
        {
            Debug.LogWarning("Slot count mismatch. Check the number of InventorySlot components in the container.");
        }

        foreach (var slot in slots)
        {
            slot.ClearSlot();
        }
    }

    public int AddItem(InventoryItem newItem, int count)
    {
        int remainingCount = count;

        // First, try to add to existing stacks
        foreach (var slot in slots)
        {
            if (slot.GetItem() != null && slot.GetItem().itemID == newItem.itemID && newItem.isStackable)
            {
                int spaceLeft = newItem.maxStackSize - slot.GetItemCount();
                if (spaceLeft > 0)
                {
                    int itemsToAdd = Mathf.Min(spaceLeft, remainingCount);
                    slot.AddItem(newItem, slot.GetItemCount() + itemsToAdd);
                    remainingCount -= itemsToAdd;

                    if (remainingCount <= 0)
                    {
                        return 0; // All items were added
                    }
                }
            }
        }

        // If there are still items remaining, add them to empty slots
        foreach (var slot in slots)
        {
            if (slot.GetItem() == null)
            {
                slot.AddItem(newItem, remainingCount);
                return 0; // All items were added
            }
        }

        Debug.LogWarning("No available slot to add the item.");
        return remainingCount; // Return the remaining count if there was not enough space
    }
}
