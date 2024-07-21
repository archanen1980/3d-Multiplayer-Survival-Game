using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Image icon; // UI icon for the item
    public TextMeshProUGUI itemAmount; // UI text for item count
    private InventoryItem item; // The item in this slot
    private int itemCount; // The count of the item in this slot

    private void Start()
    {
        ClearSlot();
    }

    public bool AddItem(InventoryItem newItem, int newCount)
    {
        if (item == null)
        {
            item = newItem;
            itemCount = newCount;
            icon.sprite = item.icon;
            icon.enabled = true;

            UpdateItemAmountText();

            // Make the icon draggable
            if (icon.gameObject.GetComponent<DraggableItem>() == null)
            {
                icon.gameObject.AddComponent<DraggableItem>();
            }

            return true;
        }
        else if (item.itemID == newItem.itemID && item.isStackable)
        {
            itemCount = newCount;
            item.count = itemCount; // Ensure the item count is updated correctly
            UpdateItemAmountText();
            return true;
        }

        return false;
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
}
