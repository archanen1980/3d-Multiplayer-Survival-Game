using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Image itemAmountBG;
    public TextMeshProUGUI itemAmount;  // Assuming you have a Text component for item amount
    private InventoryItem item;
    private int amount;

    private void Start()
    {
        // Ensure the icon and text are invisible initially
        icon.enabled = false;
        itemAmountBG.enabled = false;
    }

    public void AddItem(InventoryItem newItem, int count)
    {
        item = newItem;
        amount = count;
        icon.sprite = item.icon;
        icon.enabled = true;

        if (item.isStackable)
        {
            itemAmount.text = amount.ToString();
            itemAmountBG.enabled = true;
        }
        else
        {
            itemAmountBG.enabled = false;
        }

        // make the icon Draggable
        icon.gameObject.AddComponent<DraggableItem>();
    }

    public void ClearSlot()
    {
        item = null;
        amount = 0;
        icon.sprite = null;
        itemAmount.text = null;
        icon.enabled = false;
        //itemAmount.enabled = false;
        itemAmountBG.enabled = false;
    }

    public void OnRemoveButton()
    {
        // Call Inventory remove method
    }
}
