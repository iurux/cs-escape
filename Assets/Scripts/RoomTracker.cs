using UnityEngine;

public class RoomTracker : MonoBehaviour
{
    public string roomID;

    private float roomEnterTime;
    private bool playerInside = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;
        roomEnterTime = Time.time;

        AnalyticsManager.LogEvent("room_enter", new RoomData
        {
            room_id = roomID
        });
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!playerInside) return;

        float duration = Time.time - roomEnterTime;

        AnalyticsManager.LogEvent("room_exit", new RoomData
        {
            room_id = roomID,
            duration = duration
        });

        playerInside = false;
    }
}

[System.Serializable]
public class RoomData
{
    public string room_id;
    public float duration;
}