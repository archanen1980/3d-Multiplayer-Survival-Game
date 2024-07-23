using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip Instance;

    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI itemStatsText;
    public Vector3 offset = new Vector3(10f, 0f, 0f); // Offset for the tooltip position

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameObject.SetActive(false); // Hide tooltip initially
    }

    public void ShowTooltip(InventoryItem item, RectTransform iconRectTransform)
    {
        gameObject.SetActive(true);
        UpdateTooltipPosition(iconRectTransform);
        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.itemDescription;
        itemStatsText.text = GetItemStats(item);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    private void UpdateTooltipPosition(RectTransform iconRectTransform)
    {
        // Calculate the position to the right of the item's icon
        Vector3[] iconCorners = new Vector3[4];
        iconRectTransform.GetWorldCorners(iconCorners);
        Vector3 newPosition = iconCorners[2] + offset; // Top-right corner plus offset

        // Ensure the tooltip stays within screen bounds
        RectTransform tooltipRect = GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        tooltipRect.GetWorldCorners(corners);

        float tooltipWidth = corners[2].x - corners[0].x;
        float tooltipHeight = corners[2].y - corners[0].y;

        if (newPosition.x + tooltipWidth > Screen.width)
        {
            newPosition.x = Screen.width - tooltipWidth;
        }
        if (newPosition.y - tooltipHeight < 0)
        {
            newPosition.y = tooltipHeight;
        }

        transform.position = newPosition;
    }

    private string GetItemStats(InventoryItem item)
    {
        // Add logic to format item stats as needed
        string stats = "";
        if (item is EquipmentItem equipmentItem)
        {
            stats += $"Armor: {equipmentItem.armorModifier}\n";
            stats += $"Damage: {equipmentItem.damageModifier}\n";
        }
        // Add more stats as needed
        return stats;
    }
}
