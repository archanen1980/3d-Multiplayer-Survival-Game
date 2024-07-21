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

    public void AddItemToBackpack(int itemID, int count)
    {
        Debug.Log("AddItemToBackpack called with itemID: " + itemID + ", count: " + count);
        if (itemDatabase == null)
        {
            Debug.LogError("ItemDatabase is not assigned.");
            return;
        }

        if (containers.Count == 0 || containers[0] == null)
        {
            Debug.LogError("Backpack is not assigned.");
            return;
        }

        InventoryItem item = itemDatabase.GetItemByID(itemID);
        if (item != null)
        {
            containers[0].AddItem(item, count); // Add to the first container in the list
            Debug.Log($"Added {count} of {item.itemName} to the backpack.");
            inventoryUI.UpdateUI(); // Update the UI
        }
        else
        {
            Debug.LogWarning($"Item with ID {itemID} not found in the database.");
        }
    }

    // Similar methods for other containers (pants, jacket, belt)
}
