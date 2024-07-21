using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance; // Singleton instance of the InventoryManager
    public ItemDatabase itemDatabase;
    public ItemContainer backpack;
    public ItemContainer pants;
    public ItemContainer jacket;
    public ItemContainer pouch;

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
        if (backpack != null) backpack.InitializeContainer();
        if (pants != null) pants.InitializeContainer();
        if (jacket != null) jacket.InitializeContainer();
        if (pouch != null) pouch.InitializeContainer();
    }

    public void AddItemToBackpack(int itemID, int count)
    {
        Debug.Log("AddItemToBackpack called with itemID: " + itemID + ", count: " + count);
        if (itemDatabase == null)
        {
            Debug.LogError("ItemDatabase is not assigned.");
            return;
        }

        if (backpack == null)
        {
            Debug.LogError("Backpack is not assigned.");
            return;
        }

        InventoryItem item = itemDatabase.GetItemByID(itemID);
        if (item != null)
        {
            backpack.AddItem(item, count);
            Debug.Log($"Added {count} of {item.itemName} to the backpack.");
            inventoryUI.UpdateUI(); // Update the UI
        }
        else
        {
            Debug.LogWarning($"Item with ID {itemID} not found in the database.");
        }
    }

    // Similar methods for other containers (pants, jacket, pouch)
}
