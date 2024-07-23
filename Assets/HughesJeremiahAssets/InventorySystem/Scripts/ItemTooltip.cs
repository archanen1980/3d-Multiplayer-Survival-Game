using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip Instance;

    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI itemStatsText;
    public TextMeshProUGUI itemDurabilityText; // Text to display durability
    public TextMeshProUGUI itemWeightText; // Text to display weight
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
        itemNameText.color = GetColorByRarity(item.rarity); // Set color based on rarity
        itemDescriptionText.text = item.itemDescription;
        itemStatsText.text = GetItemStats(item);

        if (item is EquipmentItem equipmentItem)
        {
            itemDurabilityText.text = $"Durability: {equipmentItem.currentDurability}/{equipmentItem.maxDurability}";
            itemDurabilityText.gameObject.SetActive(true);
        }
        else
        {
            itemDurabilityText.gameObject.SetActive(false);
        }

        itemWeightText.text = $"Weight: {item.weight}"; // Display the weight
        itemWeightText.gameObject.SetActive(true);
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

    private Color GetColorByRarity(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Common:
                return Color.white;
            case ItemRarity.Uncommon:
                return Color.green;
            case ItemRarity.Rare:
                return Color.blue;
            case ItemRarity.Epic:
                return Color.magenta;
            case ItemRarity.Legendary:
                return new Color(1f, 0.5f, 0f); // Orange color for Legendary
            case ItemRarity.Mythic:
                return Color.red;
            case ItemRarity.Artifact:
                return new Color(0.5f, 0f, 0.5f); // Purple color for Artifact
            default:
                return Color.white;
        }
    }
}
