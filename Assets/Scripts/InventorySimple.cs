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

        // 已有就不重复加
        if (!items.Add(itemId)) return;

        if (ui != null)
            ui.AddItem(itemId, icon);

        if (checklistUI != null)
            checklistUI.MarkCollected(itemId);

        Debug.Log("[Inventory] Add: " + itemId);

        // 🔥 在这里统一记录 analytics
        LogItemCollected(itemId);
    }

    void LogItemCollected(string itemId)
    {
        float timeSinceGameStart = Time.time - AnalyticsManager.gameStartTime;

        AnalyticsManager.LogEvent("item_collected", new ItemData
        {
            item_id = itemId,
            time_since_game_start = timeSinceGameStart
        });

        Debug.Log("[Analytics] item_collected sent: " + itemId);
    }
}