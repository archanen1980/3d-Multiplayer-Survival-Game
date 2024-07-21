using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public Image icon; // UI icon for the item
    public TextMeshProUGUI itemAmount; // UI text for item count
    private InventoryItem item; // The item in this slot
    private int itemCount; // The count of the item in this slot

    private void Start()
    {
        ClearSlot();
    }

    public void AddItem(InventoryItem newItem, int newCount)
    {
        item = newItem;
        itemCount = newCount;
        icon.sprite = item.icon;
        icon.enabled = true;

        UpdateItemAmountText();

        // Ensure the DraggableItem component is attached to the icon
        if (icon.gameObject.GetComponent<DraggableItem>() == null)
        {
            icon.gameObject.AddComponent<DraggableItem>();
        }
    }

    public void ClearSlot()
    {
        item = null;
        itemCount = 0;
        icon.sprite = null;
        icon.enabled = false;
        itemAmount.text = null;
        itemAmount.enabled = false;
    }

    private void UpdateItemAmountText()
    {
        if (item != null && item.isStackable && itemCount > 1)
        {
            itemAmount.text = itemCount.ToString();
            itemAmount.enabled = true;
        }
        else
        {
            itemAmount.text = "";
            itemAmount.enabled = false;
        }
    }

    public InventoryItem GetItem() => item;
    public int GetItemCount() => itemCount;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ContextMenu.Instance.ShowContextMenu(this);
        }
    }
}
