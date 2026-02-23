using System.Runtime.InteropServices;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SendUnityAnalytics(string eventName, string jsonData);
#endif

    public static string playerID;
    public static string sessionID;

    void Awake()
    {
        if (!PlayerPrefs.HasKey("player_id"))
        {
            PlayerPrefs.SetString("player_id", System.Guid.NewGuid().ToString());
        }

        playerID = PlayerPrefs.GetString("player_id");
        sessionID = System.Guid.NewGuid().ToString();

        LogEvent("game_start", new {
            player_id = playerID,
            session_id = sessionID,
            game_version = "0.9.2"
        });
    }

    public static void LogEvent(string eventName, object data)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        string json = JsonUtility.ToJson(data);
        SendUnityAnalytics(eventName, json);
#endif
    }
}