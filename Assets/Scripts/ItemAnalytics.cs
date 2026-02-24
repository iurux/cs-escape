using UnityEngine;
using System.Collections.Generic;

public class ItemAnalytics : MonoBehaviour
{
    public string itemID;

    public void OnItemCollected()
    {
        if (string.IsNullOrEmpty(itemID))
        {
            Debug.LogWarning("ItemAnalytics: itemID is empty.");
            return;
        }

        float timeSinceGameStart = Time.time - AnalyticsManager.gameStartTime;

        AnalyticsManager.LogEvent("item_collected",
            new Dictionary<string, object>
            {
                { "item_id", itemID },
                { "time_since_game_start", timeSinceGameStart }
            });

        Debug.Log("[Analytics] item_collected: " + itemID);
    }
}