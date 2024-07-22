using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    public GameObject equipmentUI; // The Equipment UI
    public EquipmentSlot[] equipmentSlots; // Array to hold references to equipment slots

    private void Start()
    {
        equipmentUI.SetActive(false); // Start with the equipment UI closed
        InitializeSlots(); // Initialize all slots
    }

    private void InitializeSlots()
    {
        foreach (var slot in equipmentSlots)
        {
            slot.ClearSlot(); // Ensure each slot is cleared initially
        }
    }

    public void ToggleEquipmentUI(bool isActive)
    {
        equipmentUI.SetActive(isActive);
        if (isActive)
        {
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        // Add any additional logic for updating the equipment UI here
    }
}
