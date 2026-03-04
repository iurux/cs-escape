using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Newtonsoft.Json;

public class AnalyticsManager : MonoBehaviour
{
#if (UNITY_WEBGL || UNITY_WEB) && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SendUnityAnalytics(string eventName, string jsonData);
#else
    private static void SendUnityAnalytics(string eventName, string jsonData)
    {
        Debug.Log("SendUnityAnalytics called (non-web)");
    }
#endif

#if (UNITY_WEBGL || UNITY_WEB) && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void RegisterUnloadHandler(
        string playerID,
        string sessionID,
        string gameStartTime
    );
#else
    private static void RegisterUnloadHandler(
        string playerID,
        string sessionID,
        string gameStartTime
    ) { }
#endif

    public static AnalyticsManager Instance;

    public static string playerID;
    public static string sessionID;
    public static string gameVersion;

    private float sessionStartTime;
    public static float gameStartTime;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializePlayer();
    }

    void Start()
    {
        StartSession();
    }

    void InitializePlayer()
    {
        if (!PlayerPrefs.HasKey("player_id"))
        {
            PlayerPrefs.SetString("player_id", Guid.NewGuid().ToString());
            PlayerPrefs.Save();
        }

        playerID = PlayerPrefs.GetString("player_id");
        gameVersion = Application.version;
    }

    void StartSession()
    {
        sessionID = Guid.NewGuid().ToString();
        sessionStartTime = Time.time;
        gameStartTime = Time.time;

        LogEvent("session_start", new Dictionary<string, object>());

#if UNITY_WEBGL && !UNITY_EDITOR
        // 注册浏览器关闭监听
        RegisterUnloadHandler(
            playerID,
            sessionID,
            gameStartTime.ToString()
        );
#endif
    }

    void OnApplicationQuit()
    {
        float totalPlayTime = Time.time - sessionStartTime;

        LogEvent("session_end", new Dictionary<string, object>
        {
            { "total_play_time", totalPlayTime },
            { "closed_by", "application_quit" }
        });
    }

    // 核心函数
    public static void LogEvent(string eventName, Dictionary<string, object> eventData)
    {
        Debug.Log("=== BUILD CHECK ===");

#if UNITY_WEBGL
    Debug.Log("UNITY_WEBGL defined");
#endif

#if UNITY_WEB
    Debug.Log("UNITY_WEB defined");
#endif

#if UNITY_EDITOR
    Debug.Log("UNITY_EDITOR defined");
#endif
        var finalData = new Dictionary<string, object>
        {
            { "event_name", eventName },
            { "player_id", playerID },
            { "session_id", sessionID },
            { "game_version", gameVersion },
            { "ab_variant", ABTestManager.Variant },
            { "server_timestamp", DateTime.UtcNow.ToString("o") },
            { "time_since_game_start", Time.time - gameStartTime },
            { "metadata", eventData ?? new Dictionary<string, object>() }
        };

        string json = JsonConvert.SerializeObject(finalData);

// #if (UNITY_WEBGL || UNITY_WEB) && !UNITY_EDITOR
    SendUnityAnalytics(eventName, json);
// #else
    // Debug.Log("Event written: " + eventName);
// #endif
    }

    // static string DictionaryToJson(Dictionary<string, object> dict)
    // {
    //     List<string> entries = new List<string>();

    //     foreach (var kv in dict)
    //     {
    //         string value;

    //         if (kv.Value is string)
    //             value = $"\"{kv.Value}\"";
    //         else if (kv.Value is bool)
    //             value = kv.Value.ToString().ToLower();
    //         else
    //             value = kv.Value.ToString();

    //         entries.Add($"\"{kv.Key}\":{value}");
    //     }

    //     return "{" + string.Join(",", entries) + "}";
    // }
}