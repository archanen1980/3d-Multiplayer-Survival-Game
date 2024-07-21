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
}
