using UnityEngine;
using TMPro;

public class EquipmentUI : MonoBehaviour
{
    public static EquipmentUI Instance;

    public GameObject equipmentUI; // The Equipment UI
    public EquipmentSlot[] equipmentSlots; // Array to hold references to equipment slots
    public TextMeshProUGUI weightText; // Text field to display current and max weight

    public float maxWeight = 100f; // Maximum weight capacity

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
        UpdateWeight(); // Initialize weight calculation
    }

    private void InitializeSlots()
    {
        foreach (var slot in equipmentSlots)
        {
            slot.ClearSlot(); // Ensure each slot is cleared initially
        }
        UpdateWeight(); // Initialize weight calculation
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
        UpdateWeight(); // Update weight whenever the UI is updated
    }

    public void UpdateWeight()
    {
        float currentWeight = 0f;
        foreach (var slot in equipmentSlots)
        {
            if (slot.GetEquippedItem() != null)
            {
                currentWeight += slot.GetEquippedItem().weight;
            }
        }
        weightText.text = $"Weight: {currentWeight:F2} / {maxWeight:F2}"; // Display the current and max weight with 2 decimal places
    }
}
