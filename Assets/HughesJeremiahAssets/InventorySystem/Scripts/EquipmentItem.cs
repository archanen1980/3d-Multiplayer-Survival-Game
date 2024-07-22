using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipment", menuName = "Inventory/Equipment")]
public class EquipmentItem : InventoryItem
{
    public int armorModifier;
    public int damageModifier;
    public int slotsToAdd; // New field for the amount of slots this equipment provides
    public float movementMultiplier; // New field for movement speed multiplier
}
