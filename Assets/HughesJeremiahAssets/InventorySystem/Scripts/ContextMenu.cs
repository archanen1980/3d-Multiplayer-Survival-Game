using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ContextMenu : MonoBehaviour
{
    public static ContextMenu Instance;

    private GameObject contextMenu; // Reference to the context menu object
    private Button removeButton; // Reference to the remove button
    private InventorySlot currentSlot;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Dynamically find the ContextMenu object and RemoveButton
            contextMenu = this.gameObject; // Assuming the script is on this object
            if (contextMenu == null)
            {
                Debug.LogError("ContextMenu object not found. Ensure it is a child of this GameObject.");
                return;
            }

            removeButton = contextMenu.GetComponentInChildren<Button>();
            if (removeButton == null)
            {
                Debug.LogError("RemoveButton not found. Ensure it is a child of the ContextMenu.");
                return;
            }

            contextMenu.SetActive(false); // Ensure the context menu is hidden initially
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (contextMenu.activeSelf && Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
        {
            HideContextMenu();
        }
    }

    public void ShowContextMenu(InventorySlot slot)
    {
        if (contextMenu == null)
        {
            Debug.LogError("ContextMenu object is not assigned.");
            return;
        }

        if (removeButton == null)
        {
            Debug.LogError("RemoveButton is not assigned.");
            return;
        }

        contextMenu.SetActive(true);
        contextMenu.transform.position = Input.mousePosition;
        currentSlot = slot;

        removeButton.onClick.RemoveAllListeners();
        removeButton.onClick.AddListener(RemoveItem);

        Debug.Log("Context menu shown for slot: " + slot);
    }

    public void HideContextMenu()
    {
        contextMenu.SetActive(false);
    }

    private void RemoveItem()
    {
        if (currentSlot == null)
        {
            Debug.LogError("Current slot is null.");
            return;
        }

        Debug.Log("Removing item from slot: " + currentSlot);
        currentSlot.ClearSlot();
        HideContextMenu();
        InventoryManager.instance.RefreshUI(); // Refresh the UI
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (RaycastResult result in results)
        {
            if (result.gameObject == contextMenu || result.gameObject.transform.IsChildOf(contextMenu.transform))
            {
                return true;
            }
        }
        return false;
    }
}
