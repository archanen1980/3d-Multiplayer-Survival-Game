using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public InventoryItem[] items;

    public InventoryItem GetItemByID(int id)
    {
        foreach (InventoryItem item in items)
        {
            if (item.itemID == id)
            {
                return item;
            }
        }
        return null;
    }
}
