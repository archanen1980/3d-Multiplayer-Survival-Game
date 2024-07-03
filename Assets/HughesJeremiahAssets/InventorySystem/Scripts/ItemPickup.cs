using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Inventory inventory;
    public InventoryUI inventoryUI;
    public InventoryItem itemToPickup; // The item to be picked up
    public int itemID; // The ID of the item to be picked up
    public int itemCount = 1; // Number of items to pick up

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {

            PickupItem();
            inventoryUI.UpdateUI();
        }
    }

    public void PickupItem()
    {
        Debug.Log("SimulateItemPickup called");
        if (inventory != null)
        {
            bool itemAdded = inventory.AddItem(itemID, itemCount);
            InventoryItem item = inventory.itemDatabase.GetItemByID(itemID);
            if (itemAdded)
            {
                Debug.Log("Item picked up: " + item.itemName);
                inventory.SaveInventory();
            }
            else
            {
                Debug.Log("Failed to pick up item: " + item.itemName);
            }
        }
        else
        {
            Debug.LogError("Inventory is not set in ItemPickupSimulator");
        }
    }
}