using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemDatabase", menuName = "Inventory/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public List<InventoryItem> itemList = new List<InventoryItem>();

    private Dictionary<int, InventoryItem> itemDictionary = new Dictionary<int, InventoryItem>();

    private void OnEnable()
    {
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        itemDictionary.Clear();
        foreach (var item in itemList)
        {
            if (!itemDictionary.ContainsKey(item.itemID))
            {
                itemDictionary.Add(item.itemID, item);
            }
            else
            {
                Debug.LogWarning($"Item ID {item.itemID} is already in the database. Skipping {item.itemName}.");
            }
        }
    }

    public InventoryItem GetItemByID(int id)
    {
        itemDictionary.TryGetValue(id, out var item);
        return item;
    }

    public void AddItem(InventoryItem item)
    {
        if (!itemDictionary.ContainsKey(item.itemID))
        {
            itemList.Add(item);
            itemDictionary.Add(item.itemID, item);
        }
        else
        {
            Debug.LogWarning($"Item ID {item.itemID} is already in the database. Skipping {item.itemName}.");
        }
    }
}
