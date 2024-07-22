using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab; // Prefab for an inventory slot

    // Parent objects for different containers
    public List<Transform> slotParents = new List<Transform>();

    public GameObject inventoryUI; // The Inventory UI
    public EquipmentUI equipmentUI; // Reference to the Equipment UI script

    private InventoryManager inventoryManager;
    private PlayerInputActions playerInputActions;
    private bool isInventoryOpen = false;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.ToggleInventory.performed += OnToggleInventory;
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
        inventoryManager = InventoryManager.instance;
        inventoryUI.SetActive(false); // Start with the inventory UI closed
        CreateAllSlots(); // Create all slots initially
        UpdateUI(); // Initial UI update
    }

    private void CreateAllSlots()
    {
        for (int i = 0; i < inventoryManager.containers.Count; i++)
        {
            CreateSlots(inventoryManager.containers[i], slotParents[i]);
        }
    }

    private void CreateSlots(ItemContainer container, Transform parent)
    {
        // Clear any existing slots
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }

        // Create slots based on the container's slot count
        for (int i = 0; i < container.slotCount; i++)
        {
            Instantiate(slotPrefab, parent);
        }

        container.InitializeContainer(); // Initialize container with the created slots
    }

    public void UpdateUI()
    {
        for (int i = 0; i < inventoryManager.containers.Count; i++)
        {
            inventoryManager.containers[i].gameObject.SetActive(inventoryManager.containers[i].slotCount > 0);
            UpdateContainerSlots(inventoryManager.containers[i], slotParents[i]);
        }
    }

    private void UpdateContainerSlots(ItemContainer container, Transform parent)
    {
        InventorySlot[] slots = parent.GetComponentsInChildren<InventorySlot>();
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < container.slots.Count && container.slots[i].GetItem() != null)
            {
                slots[i].AddItem(container.slots[i].GetItem(), container.slots[i].GetItemCount());
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }

    public void RefreshUI()
    {
        UpdateUI();
        equipmentUI.UpdateUI(); // Also update the Equipment UI
    }

    private void OnToggleInventory(InputAction.CallbackContext context)
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryUI.SetActive(isInventoryOpen);
        equipmentUI.ToggleEquipmentUI(isInventoryOpen); // Toggle equipment UI visibility

        if (isInventoryOpen)
        {
            UpdateUI(); // Update the UI when inventory is toggled on
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            Cursor.visible = true; // Show the cursor
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
            Cursor.visible = false; // Hide the cursor
        }
    }
}
