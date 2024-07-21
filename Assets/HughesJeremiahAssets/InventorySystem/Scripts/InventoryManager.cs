using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance; // Singleton instance of the InventoryManager
    public ItemDatabase itemDatabase;
    public List<ItemContainer> containers = new List<ItemContainer>();

    public InventoryUI inventoryUI; // Reference to the InventoryUI script

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of InventoryManager found!");
            return;
        }
        instance = this;
    }

    private void Start()
    {
        foreach (var container in containers)
        {
            container.InitializeContainer();
        }
    }

    public void AddItemToInventory(int itemID, int count)
    {
        Debug.Log("AddItemToInventory called with itemID: " + itemID + ", count: " + count);
        if (itemDatabase == null)
        {
            Debug.LogError("ItemDatabase is not assigned.");
            return;
        }

        InventoryItem item = itemDatabase.GetItemByID(itemID);
        if (item != null)
        {
            int remainingCount = count;
            foreach (var container in containers)
            {
                if (remainingCount <= 0)
                {
                    break;
                }
                remainingCount = container.AddItem(item, remainingCount);
            }

            if (remainingCount > 0)
            {
                Debug.LogWarning($"Not enough space to add {remainingCount} of {item.itemName} to the inventory.");
            }
            else
            {
                Debug.Log($"Added {count} of {item.itemName} to the inventory.");
            }
            inventoryUI.UpdateUI(); // Update the UI
        }
        else
        {
            Debug.LogWarning($"Item with ID {itemID} not found in the database.");
        }
    }
}
