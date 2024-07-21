using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public int slotCount; // Number of slots in the container
    public List<InventoryItem> items = new List<InventoryItem>();

    public void InitializeContainer()
    {
        items = new List<InventoryItem>(new InventoryItem[slotCount]);
    }

    public int AddItem(InventoryItem newItem, int count)
    {
        int remainingCount = count;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                InventoryItem newItemInstance = Instantiate(newItem);
                newItemInstance.count = remainingCount;
                items[i] = newItemInstance;
                return 0; // All items were added
            }
            else if (items[i].itemID == newItem.itemID && items[i].isStackable)
            {
                int spaceLeft = newItem.maxStackSize - items[i].count;
                int itemsToAdd = Mathf.Min(spaceLeft, remainingCount);
                items[i].count += itemsToAdd;
                remainingCount -= itemsToAdd;

                if (remainingCount <= 0)
                {
                    return 0; // All items were added
                }
            }
        }

        Debug.LogWarning("No available slot to add the item.");
        return remainingCount; // Return the remaining count if there was not enough space
    }

    public void MoveItem(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex >= items.Count || toIndex < 0 || toIndex >= items.Count)
        {
            Debug.LogWarning("Invalid slot index.");
            return;
        }

        InventoryItem itemToMove = items[fromIndex];
        items[fromIndex] = items[toIndex];
        items[toIndex] = itemToMove;
    }

    public void TransferItem(int fromIndex, int toIndex, ItemContainer targetContainer)
    {
        if (fromIndex < 0 || fromIndex >= items.Count || toIndex < 0 || toIndex >= targetContainer.items.Count)
        {
            Debug.LogWarning("Invalid slot index.");
            return;
        }

        InventoryItem itemToTransfer = items[fromIndex];
        if (itemToTransfer == null)
        {
            return;
        }

        InventoryItem targetItem = targetContainer.items[toIndex];
        if (targetItem == null)
        {
            targetContainer.items[toIndex] = itemToTransfer;
            items[fromIndex] = null;
        }
        else if (targetItem.itemID == itemToTransfer.itemID && targetItem.isStackable)
        {
            int spaceLeft = targetItem.maxStackSize - targetItem.count;
            int itemsToAdd = Mathf.Min(spaceLeft, itemToTransfer.count);
            targetItem.count += itemsToAdd;
            itemToTransfer.count -= itemsToAdd;

            if (itemToTransfer.count <= 0)
            {
                items[fromIndex] = null;
            }
        }
    }
}
