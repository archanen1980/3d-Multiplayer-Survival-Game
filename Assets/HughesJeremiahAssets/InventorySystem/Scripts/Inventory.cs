using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public ItemDatabase itemDatabase;
    public List<InventoryItem> items = new List<InventoryItem>();
    public int maxInventorySize = 16;
    public bool developerMode = true; // Toggle developer mode here

    private void Start()
    {
        Debug.Log("Inventory Start Called");
        InitializeInventory();
        if (!developerMode)
        {
            LoadInventory();
        }
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
        if (developerMode)
        {
            Debug.Log("Developer mode enabled, not saving to PlayFab.");
            return;
        }

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
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "Inventory", json }
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    private void OnDataSend(UpdateUserDataResult result)
    {
        Debug.Log("Inventory data successfully saved to PlayFab!");
    }

    public void LoadInventory()
    {
        if (developerMode)
        {
            Debug.Log("Developer mode enabled, not loading from PlayFab.");
            return;
        }

        Debug.Log("Loading inventory");
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataReceived, OnError);
    }

    private void OnDataReceived(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey("Inventory"))
        {
            string json = result.Data["Inventory"].Value;
            InventoryData data = JsonUtility.FromJson<InventoryData>(json);
            Debug.Log("Inventory data successfully loaded from PlayFab!");

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
            Debug.LogWarning("No inventory data found in PlayFab.");
        }
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError("Error: " + error.GenerateErrorReport());
    }

    public InventoryItem GetItemByID(int id)
    {
        return itemDatabase.GetItemByID(id);
    }

    // Developer Mode Commands
    private void Update()
    {
        DeveloperModeUsage();
    }

    private void DeveloperModeUsage()
    {
        if (developerMode)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                AddItem(1, 1); // Example: Add 1 items with ID 1
                Debug.Log("Added 10 items with ID 1");
            }
            // Add more cheat commands as needed
        }
    }
}
