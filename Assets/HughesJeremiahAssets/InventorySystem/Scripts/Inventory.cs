using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Inventory : MonoBehaviour
{
    public ItemDatabase itemDatabase;
    public List<InventoryItem> items = new List<InventoryItem>();
    public int maxInventorySize = 16;

    private void Start()
    {
        Debug.Log("Inventory Start Called");
        InitializeInventory();
        LoadInventory();
    }

    private void InitializeInventory()
    {
        Debug.Log("Initializing inventory");
        for (int i = 0; i < maxInventorySize; i++)
        {
            items.Add(null); // Initialize with empty slots
        }
    }

    public bool AddItem(int itemID, int count)
    {
        InventoryItem newItem = itemDatabase.GetItemByID(itemID);
        if (newItem == null) return false;

        Debug.Log("AddItem called with itemID: " + newItem.itemID);
        if (newItem.isStackable)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null && items[i].itemID == newItem.itemID && items[i].count < items[i].maxStackSize)
                {
                    items[i].count += count;
                    if (items[i].count > items[i].maxStackSize)
                    {
                        items[i].count = items[i].maxStackSize;
                    }
                    return true;
                }
            }
        }

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                InventoryItem newItemInstance = Instantiate(newItem);
                newItemInstance.count = count;
                items[i] = newItemInstance;
                return true;
            }
        }

        Debug.Log("Inventory is full!");
        return false;
    }

    public void RemoveItem(int itemID, int count)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null && items[i].itemID == itemID)
            {
                items[i].count -= count;
                if (items[i].count <= 0)
                {
                    items[i] = null;
                }
                return;
            }
        }
    }

    [System.Serializable]
    private class InventoryData
    {
        public List<int> itemIDs = new List<int>();
        public List<int> itemCounts = new List<int>();
    }

    public void SaveInventory()
    {
        Debug.Log("Saving inventory");
        InventoryData data = new InventoryData();
        foreach (InventoryItem item in items)
        {
            if (item != null)
            {
                data.itemIDs.Add(item.itemID);
                data.itemCounts.Add(item.count);
            }
            else
            {
                data.itemIDs.Add(-1);
                data.itemCounts.Add(0);
            }
        }

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/inventory.json", json);
        Debug.Log("Inventory saved to: " + Application.persistentDataPath + "/inventory.json");
    }

    public void LoadInventory()
    {
        Debug.Log("Loading inventory");
        string path = Application.persistentDataPath + "/inventory.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            InventoryData data = JsonUtility.FromJson<InventoryData>(json);
            Debug.Log("Inventory loaded from: " + path);

            // Ensure the inventory list can accommodate the number of items being loaded
            if (data.itemIDs.Count > items.Count)
            {
                int additionalSlots = data.itemIDs.Count - items.Count;
                for (int i = 0; i < additionalSlots; i++)
                {
                    items.Add(null);
                }
            }

            for (int i = 0; i < data.itemIDs.Count; i++)
            {
                if (data.itemIDs[i] != -1)
                {
                    InventoryItem item = itemDatabase.GetItemByID(data.itemIDs[i]);
                    if (item != null)
                    {
                        items[i] = item;
                        items[i].count = data.itemCounts[i];
                        Debug.Log($"Loaded item: {item.itemName} with count: {items[i].count}");
                    }
                }
                else
                {
                    items[i] = null;
                }
            }
        }
        else
        {
            Debug.LogWarning("No inventory file found at: " + path);
        }
    }

    public InventoryItem GetItemByID(int id)
    {
        // You need to implement a way to find and return an InventoryItem by its ID.
        // This might involve searching through a list of predefined items or creating a lookup method.
        // For now, let's assume you have a method or a way to get the item by ID.
        // For example:
        return Resources.Load<InventoryItem>($"Items/Item_{id}");
    }

    // Additional methods for managing inventory (i.e. using items)

}

