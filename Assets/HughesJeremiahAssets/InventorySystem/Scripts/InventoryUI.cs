using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent; // The parent object of all the inventory slots
    public GameObject inventoryUI; // The entire UI

    private Inventory inventory;
    private InventorySlot[] slots;
    private PlayerInputActions playerInputActions;
    private FirstPersonController playerController; // Reference to the player controller
    private bool isInventoryOpen = false;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();

        // Dynamically locate the inventoryUI GameObject
        itemsParent = GameObject.FindGameObjectWithTag("InventoryUI").transform;
        inventoryUI = GameObject.FindGameObjectWithTag("InventoryUI");
        playerController = FindObjectOfType<FirstPersonController>();

        if (itemsParent == null)
        {
            Debug.LogError("itemsParent GameObject not found in the scene. Please make sure it is named correctly.");
        }

        if (inventoryUI == null)
        {
            Debug.LogError("InventoryUI GameObject not found in the scene. Please make sure it is named correctly.");
        }

        if (playerController == null)
        {
            Debug.LogError("PlayerController not found in the scene. Please make sure it is present.");
        }
    }

    private void OnEnable()
    {
        playerInputActions.Player.Enable();
        playerInputActions.Player.ToggleInventory.performed += OnToggleInventory;
    }

    private void OnDisable()
    {
        playerInputActions.Player.ToggleInventory.performed -= OnToggleInventory;
        playerInputActions.Player.Disable();
    }

    void Start()
    {
        inventory = GetComponent<Inventory>();
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
        inventoryUI.SetActive(false);

        InitializeUI();
    }

    void OnToggleInventory(InputAction.CallbackContext context)
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryUI.SetActive(isInventoryOpen);
        UpdateUI();

        if (isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            Cursor.visible = true; // Show the cursor
            playerController.SetMouseLookEnabled(false); // Disable mouse look
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
            Cursor.visible = false; // Hide the cursor
            playerController.SetMouseLookEnabled(true); // Enable mouse look
        }

    }

    private void InitializeUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.maxInventorySize)
            {
                slots[i].ClearSlot();
            }
            else
            {
                slots[i].gameObject.SetActive(false); // Hide unused slots
            }
        }
    }

    public void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count && inventory.items[i] != null)
            {
                // Assuming the InventoryItem class has a count property for stackable items
                int itemCount = inventory.items[i].isStackable ? inventory.items[i].count : 1;
                slots[i].AddItem(inventory.items[i], itemCount);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}
