using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipment", menuName = "Inventory/Equipment")]
public class EquipmentItem : InventoryItem
{
    public int armorModifier;
    public int damageModifier;
}
