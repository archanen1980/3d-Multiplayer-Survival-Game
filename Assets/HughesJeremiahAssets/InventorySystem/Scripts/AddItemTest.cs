using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic; // If using TextMeshPro for UI elements

public class AddItemTest : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public ItemDatabase itemDatabase;
    public TMP_Dropdown itemDropdown; // Dropdown to select item
    public TMP_InputField itemCountInput; // Input field to set item count
    public Button addButton; // Reference to the add button

    private void Start()
    {
        if (inventoryManager == null)
        {
            inventoryManager = GameObject.FindFirstObjectByType<InventoryManager>();
        }
        if (itemDatabase == null)
        {
            itemDatabase = GameObject.FindFirstObjectByType<ItemDatabase>();
        }

        // Populate the dropdown with item names
        PopulateItemDropdown();

        if (addButton != null)
        {
            addButton.onClick.RemoveListener(AddItemToInventory); // Ensure no duplicate listeners
            addButton.onClick.AddListener(AddItemToInventory);
        }
    }

    private void PopulateItemDropdown()
    {
        if (itemDropdown != null)
        {
            itemDropdown.ClearOptions();
            List<string> options = new List<string>();
            foreach (var item in itemDatabase.itemList)
            {
                options.Add(item.itemName);
            }
            itemDropdown.AddOptions(options);
        }
        else
        {
            Debug.LogError("Item dropdown reference is not set.");
        }
    }

    public void AddItemToInventory()
    {
        Debug.Log("AddItemToInventory called.");

        int selectedItemIndex = itemDropdown.value;
        InventoryItem selectedItem = itemDatabase.itemList[selectedItemIndex];
        int itemCount;

        if (!int.TryParse(itemCountInput.text, out itemCount))
        {
            itemCount = 1; // Default to 1 if parsing fails
        }

        Debug.Log($"Adding {itemCount} of {selectedItem.itemName} to the inventory.");

        if (selectedItem != null)
        {
            inventoryManager.AddItemToInventory(selectedItem.itemID, itemCount);
            Debug.Log($"Added {itemCount} of {selectedItem.itemName} to the inventory.");
        }
        else
        {
            Debug.LogWarning("Selected item not found in the database.");
        }

        // Reset item count input field to default value (e.g., 1)
        itemCountInput.text = "1";
    }
}
