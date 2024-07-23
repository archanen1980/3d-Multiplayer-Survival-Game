using UnityEngine;
using TMPro;

public class EquipmentUI : MonoBehaviour
{
    public static EquipmentUI Instance;

    public GameObject equipmentUI; // The Equipment UI
    public EquipmentSlot[] equipmentSlots; // Array to hold references to equipment slots
    public TextMeshProUGUI totalWeightText; // Text field to display total weight

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
        equipmentUI.SetActive(false); // Start with the equipment UI closed
        InitializeSlots(); // Initialize all slots
    }

    private void InitializeSlots()
    {
        foreach (var slot in equipmentSlots)
        {
            slot.ClearSlot(); // Ensure each slot is cleared initially
        }
        UpdateTotalWeight(); // Initialize total weight calculation
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
        UpdateTotalWeight(); // Update total weight whenever the UI is updated
    }

    public void UpdateTotalWeight()
    {
        float totalWeight = 0f;
        foreach (var slot in equipmentSlots)
        {
            if (slot.GetEquippedItem() != null)
            {
                totalWeight += slot.GetEquippedItem().weight;
            }
        }
        totalWeightText.text = $"Weight: {totalWeight:F2}"; // Display the total weight with 2 decimal places
    }
}
