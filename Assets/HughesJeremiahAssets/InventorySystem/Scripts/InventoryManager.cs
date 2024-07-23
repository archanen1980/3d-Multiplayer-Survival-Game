using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public ItemDatabase itemDatabase;
    public List<ItemContainer> containers = new List<ItemContainer>();

    public InventoryUI inventoryUI;
    public EquipmentUI equipmentUI;
    public bool useContextMenuToDelete = true; // Toggle for using the context menu to delete
    public bool useDragAndDropToDelete = true; // Toggle for using drag and drop to delete
    public bool useCurrency = true; // Toggle for using currency
    public int currentCurrency = 0; // Current amount of currency

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of InventoryManager found!");
            return;
        }
        instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.CheatCurrency.performed += OnCheatCurrency;
    }

    private void OnEnable()
    {
        playerInputActions.Player.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Player.Disable();
    }

    private void Start()
    {
        foreach (var container in containers)
        {
            container.InitializeContainer();
        }

        if (useCurrency)
        {
            equipmentUI.UpdateCurrencyUI(currentCurrency);
        }
        else
        {
            equipmentUI.HideCurrencyUI();
        }
    }

    public void AddItemToInventory(int itemID, int count)
    {
        Debug.Log("AddItemToInventory called with itemID: " + itemID + ", count: " + count);
        if (itemDatabase == null)
        {
            Debug.LogError("ItemDatabase is not assigned.");
            return;
        }

        InventoryItem item = itemDatabase.GetItemByID(itemID);
        if (item != null)
        {
            int remainingCount = count;
            foreach (var container in containers)
            {
                if (remainingCount <= 0)
                {
                    break;
                }
                remainingCount = container.AddItem(item, remainingCount);
            }

            if (remainingCount > 0)
            {
                Debug.LogWarning($"Not enough space to add {remainingCount} of {item.itemName} to the inventory.");
            }
            else
            {
                Debug.Log($"Added {count} of {item.itemName} to the inventory.");
            }
            inventoryUI.UpdateUI();
        }
        else
        {
            Debug.LogWarning($"Item with ID {itemID} not found in the database.");
        }
    }

    public void AddCurrency(int amount)
    {
        if (useCurrency)
        {
            currentCurrency += amount;
            if (equipmentUI.gameObject.activeSelf) // Only update UI if equipment UI is active
            {
                equipmentUI.UpdateCurrencyUI(currentCurrency);
            }
        }
    }

    public void SpendCurrency(int amount)
    {
        if (useCurrency && currentCurrency >= amount)
        {
            currentCurrency -= amount;
            if (equipmentUI.gameObject.activeSelf) // Only update UI if equipment UI is active
            {
                equipmentUI.UpdateCurrencyUI(currentCurrency);
            }
        }
    }

    public void RefreshUI()
    {
        inventoryUI.UpdateUI();
        equipmentUI.UpdateUI();
        if (useCurrency && equipmentUI.gameObject.activeSelf) // Only update currency UI if equipment UI is active
        {
            equipmentUI.UpdateCurrencyUI(currentCurrency);
        }
    }

    private void OnCheatCurrency(InputAction.CallbackContext context)
    {
        AddCurrency(1000); // Add 1000 currency for cheating
        Debug.Log("Cheat activated: Added 1000 currency.");
        if (equipmentUI.gameObject.activeSelf) // Only update UI if equipment UI is active
        {
            equipmentUI.UpdateCurrencyUI(currentCurrency); // Update the Equipment UI currency display
        }
    }
}
