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
    [SerializeField] private Button equipButton;
    [SerializeField] private Button unequipButton;
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

            if (deleteButton == null || splitStackButton == null || equipButton == null || unequipButton == null ||
                splitStackPanel == null || splitSlider == null || splitAmountText == null || confirmSplitButton == null)
            {
                Debug.LogError("Buttons or split stack components are not assigned. Ensure they are assigned in the inspector.");
                return;
            }

            splitStackPanelCanvasGroup = splitStackPanel.GetComponent<CanvasGroup>();
            contextMenuRectTransform = contextMenu.GetComponent<RectTransform>();
            splitStackPanelRectTransform = splitStackPanel.GetComponent<RectTransform>();

            contextMenu.SetActive(false);
            splitStackPanel.SetActive(false);
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
        if (contextMenu == null || deleteButton == null || splitStackButton == null || equipButton == null || unequipButton == null)
        {
            Debug.LogError("ContextMenu or buttons are not assigned.");
            return;
        }

        contextMenu.SetActive(true);
        contextMenu.transform.position = Input.mousePosition;

        splitStackButton.gameObject.SetActive(item != null && item.isStackable && itemCount > 1);

        deleteButton.onClick.RemoveAllListeners();
        deleteButton.onClick.AddListener(RemoveItem);

        splitStackButton.onClick.RemoveAllListeners();
        splitStackButton.onClick.AddListener(ShowSplitStackPanel);

        splitSlider.onValueChanged.AddListener(UpdateSplitAmountText);
        confirmSplitButton.onClick.AddListener(ConfirmSplit);

        equipButton.gameObject.SetActive(currentInventorySlot != null && item is EquipmentItem);
        equipButton.onClick.RemoveAllListeners();
        equipButton.onClick.AddListener(EquipItem);

        unequipButton.gameObject.SetActive(currentEquipmentSlot != null && currentEquipmentSlot.GetEquippedItem() != null);
        unequipButton.onClick.RemoveAllListeners();
        unequipButton.onClick.AddListener(UnequipItem);
    }

    public void HideContextMenu()
    {
        contextMenu.SetActive(false);
        splitStackPanel.SetActive(false);
    }

    private void ShowSplitStackPanel()
    {
        if (currentInventorySlot == null)
        {
            Debug.LogError("Current slot is null.");
            return;
        }

        Vector3 newPosition = contextMenuRectTransform.position;
        newPosition.y += contextMenuRectTransform.rect.height;

        splitStackPanel.SetActive(true);
        splitStackPanel.transform.position = newPosition;

        splitSlider.minValue = 1;
        splitSlider.maxValue = currentInventorySlot.GetItemCount() - 1;
        splitSlider.value = 1;

        UpdateSplitAmountText(splitSlider.value);
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

        int remainingAmount = itemCount - splitAmount;

        currentInventorySlot.AddItem(item, remainingAmount);

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

    private void EquipItem()
    {
        if (currentInventorySlot == null)
        {
            Debug.LogError("No inventory slot selected for equipping.");
            return;
        }

        InventoryItem item = currentInventorySlot.GetItem();
        if (item is EquipmentItem equipmentItem)
        {
            EquipmentSlot suitableSlot = FindSuitableEquipmentSlot(equipmentItem);
            if (suitableSlot != null)
            {
                EquipmentItem currentEquippedItem = suitableSlot.GetEquippedItem();

                suitableSlot.EquipItem(equipmentItem);
                currentInventorySlot.ClearSlot();

                if (currentEquippedItem != null)
                {
                    currentInventorySlot.AddItem(currentEquippedItem, 1);
                }
            }
            else
            {
                Debug.LogWarning("No suitable equipment slot found.");
            }
        }

        HideContextMenu();
        InventoryManager.instance.RefreshUI();
    }

    private void UnequipItem()
    {
        if (currentEquipmentSlot == null)
        {
            Debug.LogError("No equipment slot selected for unequipping.");
            return;
        }

        EquipmentItem item = currentEquipmentSlot.GetEquippedItem();
        if (item != null)
        {
            InventorySlot emptySlot = FindEmptyInventorySlot();
            if (emptySlot != null)
            {
                emptySlot.AddItem(item, 1);
                currentEquipmentSlot.ClearSlot();
            }
            else
            {
                Debug.LogWarning("No empty inventory slot available.");
            }
        }

        HideContextMenu();
        InventoryManager.instance.RefreshUI();
    }

    private EquipmentSlot FindSuitableEquipmentSlot(EquipmentItem item)
    {
        foreach (var slot in EquipmentUI.Instance.equipmentSlots)
        {
            if (slot.slotType == item.itemType)
            {
                return slot;
            }
        }
        return null;
    }

    private InventorySlot FindEmptyInventorySlot()
    {
        foreach (var container in InventoryManager.instance.containers)
        {
            foreach (var slot in container.slots)
            {
                if (slot.GetItem() == null)
                {
                    return slot;
                }
            }
        }
        return null;
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
