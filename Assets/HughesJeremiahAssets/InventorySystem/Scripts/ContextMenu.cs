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
    private InventorySlot currentSlot;

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
        if (contextMenu == null || deleteButton == null || splitStackButton == null)
        {
            Debug.LogError("ContextMenu or buttons are not assigned.");
            return;
        }

        currentSlot = slot;
        contextMenu.SetActive(true);
        contextMenu.transform.position = Input.mousePosition;

        InventoryItem item = slot.GetItem();
        if (item != null && item.isStackable && slot.GetItemCount() > 1)
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
        if (currentSlot == null)
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
        splitSlider.maxValue = currentSlot.GetItemCount() - 1; // Cannot split all items
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
        if (currentSlot == null)
        {
            Debug.LogError("Current slot is null.");
            return;
        }

        currentSlot.ClearSlot();
        HideContextMenu();
        InventoryManager.instance.RefreshUI();
    }

    private void SplitStack(int splitAmount)
    {
        if (currentSlot == null)
        {
            Debug.LogError("Current slot is null.");
            return;
        }

        InventoryItem item = currentSlot.GetItem();
        int itemCount = currentSlot.GetItemCount();

        if (item == null || !item.isStackable || itemCount <= 1)
        {
            Debug.LogError("Item cannot be split.");
            return;
        }

        int remainingAmount = itemCount - splitAmount;
        currentSlot.AddItem(item, remainingAmount);

        InventoryItem newStack = Instantiate(item);
        newStack.count = splitAmount;

        ItemContainer container = currentSlot.GetComponentInParent<ItemContainer>();
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
