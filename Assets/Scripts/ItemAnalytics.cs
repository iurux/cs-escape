using UnityEngine;

public class ItemAnalytics : MonoBehaviour
{
    public string itemID;

    public void OnItemCollected()
    {
        float timeSinceGameStart = Time.time - AnalyticsManager.gameStartTime;

        AnalyticsManager.LogEvent("item_collected", new ItemData
        {
            item_id = itemID,
            time_since_game_start = timeSinceGameStart
        });
    }
}

[System.Serializable]
public class ItemData
{
    public string item_id;
    public float time_since_game_start;
}