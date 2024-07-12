using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Image icon; // Reference to the icon Image component
    public TextMeshProUGUI itemAmount; // Reference to the TextMeshProUGUI component for the item count
    public Image amountBG;
    private InventoryItem item;
    private int count;

    private void Start()
    {
        // Ensure the icon and text are invisible initially
        icon.enabled = false;
        itemAmount.enabled = false;
        amountBG.enabled = false;
    }

    public void AddItem(InventoryItem newItem, int newCount)
    {
        item = newItem;
        count = newCount;
        icon.sprite = item.icon;
        icon.enabled = true;

        if (item.isStackable)
        {
            itemAmount.text = count.ToString();
            itemAmount.enabled = true;
            amountBG.enabled = true;

        }
        else
        {
            itemAmount.enabled = false;
            amountBG.enabled = false;
        }

        // Make the icon draggable
        if (icon.gameObject.GetComponent<DraggableItem>() == null)
        {
            icon.gameObject.AddComponent<DraggableItem>();
        }
    }

    public void ClearSlot()
    {
        item = null;
        count = 0;
        icon.sprite = null;
        icon.enabled = false;
        itemAmount.text = null;
        itemAmount.enabled = false;
        amountBG.enabled = false;

        // Remove the DraggableItem component if it exists
        DraggableItem draggableItem = icon.GetComponent<DraggableItem>();
        if (draggableItem != null)
        {
            Destroy(draggableItem);
        }
    }

    public InventoryItem GetItem() => item;
    public int GetItemCount() => count;

    public void OnRemoveButton()
    {
        // Implement remove item functionality if needed
    }
}
