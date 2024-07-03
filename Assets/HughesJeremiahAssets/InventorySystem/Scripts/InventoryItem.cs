using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInventoryItem", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    public int itemID;
    public string itemName = "New Item";
    public Sprite icon = null;
    public bool isStackable = false;
    public int maxStackSize = 1;

    // Property to hold the current count of items
    [HideInInspector] public int count = 1;
}
