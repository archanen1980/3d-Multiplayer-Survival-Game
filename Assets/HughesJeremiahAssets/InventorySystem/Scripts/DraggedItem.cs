using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DraggedItem : MonoBehaviour
{
    public static DraggedItem Instance;

    public Image icon;
    public TextMeshProUGUI itemAmount; // Reference to the Text component
    private InventoryItem item;
    private int count;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            icon = GetComponentInChildren<Image>(); // Get the child Image component
            itemAmount = icon.GetComponentInChildren<TextMeshProUGUI>(); // Get the child Text component
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetDraggedItem(InventoryItem newItem, int newCount)
    {
        item = newItem;
        count = newCount;
        icon.sprite = item.icon;
        icon.enabled = true;
        itemAmount.text = count.ToString(); // Set the text for item count
        itemAmount.enabled = true;
        icon.gameObject.SetActive(true);
    }

    public void ClearDraggedItem()
    {
        item = null;
        count = 0;
        icon.sprite = null;
        icon.enabled = false;
        itemAmount.text = null; // Clear the text
        itemAmount.enabled = false;
        icon.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (icon.enabled)
        {
            icon.transform.position = Input.mousePosition; // Follow the cursor
        }
    }

    public InventoryItem GetItem() => item;
    public int GetCount() => count;
}
