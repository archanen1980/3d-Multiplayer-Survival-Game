using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class ContextMenu : MonoBehaviour
{
    public static ContextMenu Instance;
    private GameObject contextMenu;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button splitStackButton;
    private InventorySlot currentInventorySlot;
    private EquipmentSlot currentEquipmentSlot;

    // References for split stack panel
    [SerializeField] private GameObject splitStackPanel;
    [SerializeField] private Slider splitSlider;
    [SerializeField] private TextMeshProUGUI splitAmountText;
    [SerializeField] private Button confirmSplitButton;

    private CanvasGroup splitStackPanelCanvasGroup;
    private RectTransform contextMenuRectTransform;
    private RectTransform splitStackPanelRectTransform;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            contextMenu = this.gameObject;
            if (contextMenu == null)
            {
                Debug.LogError("ContextMenu object not found. Ensure it is a child of this GameObject.");
                return;
            }

            if (deleteButton == null || splitStackButton == null || splitStackPanel == null || splitSlider == null || splitAmountText == null || confirmSplitButton == null)
            {
                Debug.LogError("Buttons or split stack components are not assigned. Ensure they are assigned in the inspector.");
                return;
            }

            splitStackPanelCanvasGroup = splitStackPanel.GetComponent<CanvasGroup>();
            contextMenuRectTransform = contextMenu.GetComponent<RectTransform>();
            splitStackPanelRectTransform = splitStackPanel.GetComponent<RectTransform>();

            contextMenu.SetActive(false);
            splitStackPanel.SetActive(false); // Hide split stack panel initially
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (contextMenu.activeSelf && Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
        {
            HideContextMenu();
        }

        if (splitStackPanel.activeSelf && Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
        {
            splitStackPanel.SetActive(false);
        }
    }

    public void ShowContextMenu(InventorySlot slot)
    {
        currentInventorySlot = slot;
        currentEquipmentSlot = null;
        ShowContextMenuCommon(slot.GetItem(), slot.GetItemCount());
    }

    public void ShowContextMenu(EquipmentSlot slot)
    {
        currentInventorySlot = null;
        currentEquipmentSlot = slot;
        ShowContextMenuCommon(slot.GetEquippedItem(), 1);
    }

    private void ShowContextMenuCommon(InventoryItem item, int itemCount)
    {
        if (contextMenu == null || deleteButton == null || splitStackButton == null)
        {
            Debug.LogError("ContextMenu or buttons are not assigned.");
            return;
        }

        contextMenu.SetActive(true);
        contextMenu.transform.position = Input.mousePosition;

        if (item != null && item.isStackable && itemCount > 1)
        {
            splitStackButton.gameObject.SetActive(true);
        }
        else
        {
            splitStackButton.gameObject.SetActive(false);
        }

        deleteButton.onClick.RemoveAllListeners();
        deleteButton.onClick.AddListener(RemoveItem);

        splitStackButton.onClick.RemoveAllListeners();
        splitStackButton.onClick.AddListener(ShowSplitStackPanel);

        splitSlider.onValueChanged.AddListener(UpdateSplitAmountText);
        confirmSplitButton.onClick.AddListener(ConfirmSplit);
    }

    public void HideContextMenu()
    {
        contextMenu.SetActive(false);
        splitStackPanel.SetActive(false); // Hide split stack panel when context menu is hidden
    }

    private void ShowSplitStackPanel()
    {
        if (currentInventorySlot == null)
        {
            Debug.LogError("Current slot is null.");
            return;
        }

        // Position the split stack panel above the context menu
        Vector3 newPosition = contextMenuRectTransform.position;
        newPosition.y += contextMenuRectTransform.rect.height; // Adjust the Y offset

        splitStackPanel.SetActive(true);
        splitStackPanel.transform.position = newPosition;

        // Initialize the slider
        splitSlider.minValue = 1;
        splitSlider.maxValue = currentInventorySlot.GetItemCount() - 1; // Cannot split all items
        splitSlider.value = 1; // Default to splitting one item

        UpdateSplitAmountText(splitSlider.value); // Initialize the split amount text
    }

    private void UpdateSplitAmountText(float value)
    {
        splitAmountText.text = value.ToString();
    }

    private void ConfirmSplit()
    {
        int splitAmount = (int)splitSlider.value;
        SplitStack(splitAmount);
    }

    private void RemoveItem()
    {
        if (currentInventorySlot != null)
        {
            currentInventorySlot.ClearSlot();
        }
        else if (currentEquipmentSlot != null)
        {
            currentEquipmentSlot.ClearSlot();
        }

        HideContextMenu();
        InventoryManager.instance.RefreshUI();
    }

    private void SplitStack(int splitAmount)
    {
        if (currentInventorySlot == null)
        {
            Debug.LogError("Current slot is null.");
            return;
        }

        InventoryItem item = currentInventorySlot.GetItem();
        int itemCount = currentInventorySlot.GetItemCount();

        if (item == null || !item.isStackable || itemCount <= 1)
        {
            Debug.LogError("Item cannot be split.");
            return;
        }

        // Calculate the remaining amount in the original stack after the split
        int remainingAmount = itemCount - splitAmount;

        // Update the original slot with the remaining amount
        currentInventorySlot.AddItem(item, remainingAmount);

        // Create a new stack with the split amount
        InventoryItem newStack = Instantiate(item);
        newStack.count = splitAmount;

        ItemContainer container = currentInventorySlot.GetComponentInParent<ItemContainer>();
        if (container == null)
        {
            Debug.LogError("ItemContainer not found.");
            return;
        }

        bool placed = false;

        foreach (var slot in container.slots)
        {
            if (slot.GetItem() == null)
            {
                slot.AddItem(newStack, splitAmount);
                placed = true;
                break;
            }
        }

        if (!placed)
        {
            Debug.LogWarning("No available slot to place the new stack.");
        }

        HideContextMenu();
        InventoryManager.instance.RefreshUI();
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (RaycastResult result in results)
        {
            if (result.gameObject == contextMenu || result.gameObject.transform.IsChildOf(contextMenu.transform) ||
                result.gameObject == splitStackPanel || result.gameObject.transform.IsChildOf(splitStackPanel.transform))
            {
                return true;
            }
        }
        return false;
    }
}
