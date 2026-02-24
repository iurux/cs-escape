using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SendUnityAnalytics(string eventName, string jsonData);
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
        StartSession();
    }

    void InitializePlayer()
    {
        if (!PlayerPrefs.HasKey("player_id"))
        {
            string newID = Guid.NewGuid().ToString();
            PlayerPrefs.SetString("player_id", newID);
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
    }

    void OnApplicationQuit()
    {
        float totalPlayTime = Time.time - sessionStartTime;

        LogEvent("session_end", new Dictionary<string, object>
        {
            { "total_play_time", totalPlayTime }
        });
    }

    // 🔥 核心函数
    public static void LogEvent(string eventName, Dictionary<string, object> eventData)
    {
        Dictionary<string, object> finalData = new Dictionary<string, object>();

        // 基础字段
        finalData["event_name"] = eventName;
        finalData["player_id"] = playerID;
        finalData["session_id"] = sessionID;
        finalData["game_version"] = gameVersion;

        // 追加自定义字段
        if (eventData != null)
        {
            foreach (var kv in eventData)
            {
                finalData[kv.Key] = kv.Value;
            }
        }

        string json = DictionaryToJson(finalData);

#if UNITY_WEBGL && !UNITY_EDITOR
        SendUnityAnalytics(eventName, json);
#else
        Debug.Log("[Analytics] " + json);
#endif
    }

    // 🔥 简单 JSON 转换（支持 string/float/int/bool）
    static string DictionaryToJson(Dictionary<string, object> dict)
    {
        List<string> entries = new List<string>();

        foreach (var kv in dict)
        {
            string value;

            if (kv.Value is string)
                value = $"\"{kv.Value}\"";
            else if (kv.Value is bool)
                value = kv.Value.ToString().ToLower();
            else
                value = kv.Value.ToString();

            entries.Add($"\"{kv.Key}\":{value}");
        }

        return "{" + string.Join(",", entries) + "}";
    }
}