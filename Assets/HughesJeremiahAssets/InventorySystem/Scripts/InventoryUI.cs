using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab; // Prefab for an inventory slot

    // Parent objects for different containers
    public GameObject backpackContainerParent;
    public GameObject jacketContainerParent;
    public GameObject pantsContainerParent;
    public GameObject pouchContainerParent;

    public Transform backpackSlotsParent;
    public Transform jacketSlotsParent;
    public Transform pantsSlotsParent;
    public Transform pouchSlotsParent;

    public GameObject inventoryUI; // The Inventory UI

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
        CreateSlots(inventoryManager.backpack, backpackSlotsParent);
        CreateSlots(inventoryManager.pants, pantsSlotsParent);
        CreateSlots(inventoryManager.jacket, jacketSlotsParent);
        CreateSlots(inventoryManager.pouch, pouchSlotsParent);
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
    }

    public void UpdateUI()
    {
        // Update container visibility based on slot count
        backpackContainerParent.SetActive(inventoryManager.backpack.slotCount > 0);
        pantsContainerParent.SetActive(inventoryManager.pants.slotCount > 0);
        jacketContainerParent.SetActive(inventoryManager.jacket.slotCount > 0);
        pouchContainerParent.SetActive(inventoryManager.pouch.slotCount > 0);

        UpdateContainerSlots(inventoryManager.backpack, backpackSlotsParent);
        UpdateContainerSlots(inventoryManager.pants, pantsSlotsParent);
        UpdateContainerSlots(inventoryManager.jacket, jacketSlotsParent);
        UpdateContainerSlots(inventoryManager.pouch, pouchSlotsParent);
    }

    private void UpdateContainerSlots(ItemContainer container, Transform parent)
    {
        InventorySlot[] slots = parent.GetComponentsInChildren<InventorySlot>();
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < container.items.Count && container.items[i] != null)
            {
                slots[i].AddItem(container.items[i], container.items[i].count);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }

    private void OnToggleInventory(InputAction.CallbackContext context)
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryUI.SetActive(isInventoryOpen);

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
