using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image icon;
    public TextMeshProUGUI itemAmount;
    public GameObject durabilityBarBackground; // Reference to the durability bar background GameObject
    private Image durabilityBar; // Reference to the durability bar image
    private InventoryItem item;
    private int itemCount;

    private void Awake()
    {
        // Assign the durability bar here to ensure it's set before use
        if (durabilityBarBackground != null)
        {
            durabilityBar = durabilityBarBackground.GetComponentInChildren<Image>();
            durabilityBarBackground.SetActive(false); // Hide durability bar background initially
        }
    }

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
        UpdateDurabilityBar();

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

        if (durabilityBarBackground != null && durabilityBarBackground.activeInHierarchy)
        {
            durabilityBar.fillAmount = 0; // Reset durability bar
            durabilityBarBackground.SetActive(false); // Hide durability bar background
        }
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

    private void UpdateDurabilityBar()
    {
        if (durabilityBarBackground != null && durabilityBar != null)
        {
            if (item is EquipmentItem equipmentItem && equipmentItem.maxDurability > 0)
            {
                durabilityBar.fillAmount = (float)equipmentItem.currentDurability / equipmentItem.maxDurability;
                durabilityBarBackground.SetActive(true);
            }
            else
            {
                durabilityBarBackground.SetActive(false);
            }
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
