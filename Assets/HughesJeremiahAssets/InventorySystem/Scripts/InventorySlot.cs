using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image icon;
    public TextMeshProUGUI itemAmount;
    private InventoryItem item;
    private int itemCount;

    private void Start()
    {
        ClearSlot();
    }

    public void AddItem(InventoryItem newItem, int newCount)
    {
        if (newItem == null)
        {
            Debug.LogError("NewItem is null in AddItem.");
            return;
        }

        item = newItem;
        itemCount = newCount;

        if (item.icon == null)
        {
            Debug.LogError($"Item icon is null for item: {item.itemName}");
            return;
        }

        icon.sprite = item.icon;
        icon.enabled = true;

        UpdateItemAmountText();

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            ItemTooltip.Instance.ShowTooltip(item, icon.rectTransform);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltip.Instance.HideTooltip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && InventoryManager.instance.useContextMenuToDelete && item != null)
        {
            ContextMenu.Instance.ShowContextMenu(this);
        }
    }
}
