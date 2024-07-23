using UnityEngine;

public enum ItemType
{
    DefaultItem, Head, Torso, Legs, Feet, Glasses, Face, Back, Belt, MainHand, OffHand
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    public int itemID; // Unique ID for the item
    public string itemName = "New Item"; // Name of the item
    [TextArea(5, 10)]
    public string itemDescription = "Item Description"; // Description of the item
    public Sprite icon = null; // Icon to represent the item
    public bool isStackable = false; // Indicates if the item can be stacked
    public int maxStackSize = 1; // Maximum stack size for stackable items
    public ItemType itemType; // Type of the item

    // Property to hold the current count of items
    [HideInInspector]
    public int count = 1;
}
