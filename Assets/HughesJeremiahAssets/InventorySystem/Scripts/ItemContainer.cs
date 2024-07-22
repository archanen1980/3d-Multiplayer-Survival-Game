using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public int CurrentSlotCount; // Current number of slots in the container
    public int minSlotCount; // Minimum number of slots in the container
    public List<InventorySlot> slots = new List<InventorySlot>();
    public Transform slotParent; // Parent object for the slots
    public GameObject slotPrefab; // Prefab for creating new slots

    public void InitializeContainer()
    {
        slots = new List<InventorySlot>(GetComponentsInChildren<InventorySlot>());

        if (slots.Count != CurrentSlotCount)
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

    public void SetSlots(int additionalSlots)
    {
        // Calculate the new slot count based on the minimum slots plus additional slots
        int newSlotCount = Mathf.Max(minSlotCount + additionalSlots, 0);
        Debug.Log($"Setting slot count to {newSlotCount}");

        // Adjust the number of slots
        while (slots.Count < newSlotCount)
        {
            InventorySlot newSlot = Instantiate(slotPrefab, slotParent).GetComponent<InventorySlot>(); // Ensure the new slots are added to the slot parent
            newSlot.ClearSlot();
            slots.Add(newSlot);
        }

        while (slots.Count > newSlotCount)
        {
            if (slots.Count == 0)
            {
                break;
            }

            InventorySlot slotToRemove = slots[slots.Count - 1];
            Destroy(slotToRemove.gameObject);
            slots.RemoveAt(slots.Count - 1);
        }

        // Update the slot count
        CurrentSlotCount = newSlotCount;

        // Ensure UI is updated
        InventoryManager.instance.RefreshUI();
    }
}
