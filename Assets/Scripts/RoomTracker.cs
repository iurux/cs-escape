using UnityEngine;
using System.Collections.Generic;

public class RoomTracker : MonoBehaviour
{
    public string roomID;

    private float roomEnterTime;
    private bool playerInside = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (playerInside) return;  // 🔥 防止重复进入触发

        playerInside = true;
        roomEnterTime = Time.time;

        AnalyticsManager.LogEvent("room_enter",
            new Dictionary<string, object>
            {
                { "room_id", roomID },
                { "time_since_game_start", Time.time - AnalyticsManager.gameStartTime }
            });
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!playerInside) return;

        float duration = Time.time - roomEnterTime;

        AnalyticsManager.LogEvent("room_exit",
            new Dictionary<string, object>
            {
                { "room_id", roomID },
                { "duration", duration },
                { "time_since_game_start", Time.time - AnalyticsManager.gameStartTime }
            });

        playerInside = false;
    }
}