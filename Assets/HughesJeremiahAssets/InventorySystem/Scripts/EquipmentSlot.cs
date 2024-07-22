using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    public ItemType slotType;  // The type of item this slot can hold
    public Image icon;
    private EquipmentItem currentItem;

    [SerializeField] public ItemContainer associatedContainer; // Reference to the associated ItemContainer
    private int additionalSlots = 0; // Track additional slots provided by the equipped item

    private void Awake()
    {
        // Dynamically find the icon GameObject in the children
        icon = transform.GetChild(0).GetComponent<Image>(); // Adjust the index as needed
        if (icon == null)
        {
            Debug.LogError("Icon Image not found in EquipmentSlot's children.");
        }
    }

    private void Start()
    {
        ClearSlot();
    }

    public void EquipItem(EquipmentItem item)
    {
        if (item.itemType == slotType)
        {
            currentItem = item;
            icon.sprite = item.icon;
            icon.enabled = true;

            if (associatedContainer != null)
            {
                additionalSlots = item.slotsToAdd;
                associatedContainer.SetSlots(additionalSlots);
            }
        }
        else
        {
            Debug.LogWarning($"Item {item.itemName} cannot be equipped in {slotType} slot.");
        }
    }

    public EquipmentItem GetEquippedItem()
    {
        return currentItem;
    }

    public void ClearSlot()
    {
        if (currentItem != null && associatedContainer != null)
        {
            additionalSlots = 0;
            associatedContainer.SetSlots(additionalSlots);
        }

        currentItem = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public bool CanEquipItem(EquipmentItem item)
    {
        return item.itemType == slotType;
    }
}
