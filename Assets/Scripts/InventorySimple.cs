using System.Collections.Generic;
using UnityEngine;

public class InventorySimple : MonoBehaviour
{
    public InventoryUI ui;
    public ChecklistUI checklistUI;

    private HashSet<string> items = new HashSet<string>();

    public bool Has(string itemId)
    {
        return !string.IsNullOrEmpty(itemId) && items.Contains(itemId);
    }

    public void Add(string itemId, Sprite icon = null)
    {
        if (string.IsNullOrEmpty(itemId)) return;

        // 已有就不重复加（防止重复 analytics）
        if (!items.Add(itemId)) return;

        if (ui != null)
            ui.AddItem(itemId, icon);

        if (checklistUI != null)
            checklistUI.MarkCollected(itemId);

        Debug.Log("[Inventory] Add: " + itemId);

        // 🔥 统一记录 analytics
        LogItemCollected(itemId);
    }

    void LogItemCollected(string itemId)
    {
        AnalyticsManager.LogEvent("item_collected",
            new Dictionary<string, object>
            {
                { "item_id", itemId },
                { "room_id", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name },
                { "inventory_count", items.Count }
            });

        Debug.Log("[Analytics] item_collected sent: " + itemId);
    }
}