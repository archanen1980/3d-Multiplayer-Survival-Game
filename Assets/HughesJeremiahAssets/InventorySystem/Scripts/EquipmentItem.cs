using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipment", menuName = "Inventory/Equipment")]
public class EquipmentItem : InventoryItem
{
    public int armorModifier;
    public int damageModifier;
    public int slotsToAdd; // New field for the amount of slots this equipment provides
    public float movementMultiplier; // New field for movement speed multiplier

    // Durability properties
    public int maxDurability = 100; // Max durability of the item
    [HideInInspector]
    public int currentDurability;

    private void OnEnable()
    {
        // Initialize current durability to max durability when the item is created
        currentDurability = maxDurability;
    }
}
