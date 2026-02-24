using System;
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
        Debug.Log("=== AnalyticsManager Awake CALLED ===");

        if (Instance != null)
        {
            Debug.Log("AnalyticsManager already exists. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("AnalyticsManager initialized and marked DontDestroyOnLoad.");

        InitializePlayer();
        StartSession();
    }

    void InitializePlayer()
    {
        Debug.Log("Initializing Player...");

        if (!PlayerPrefs.HasKey("player_id"))
        {
            string newID = Guid.NewGuid().ToString();
            PlayerPrefs.SetString("player_id", newID);
            PlayerPrefs.Save();
            Debug.Log("New player_id generated: " + newID);
        }
        else
        {
            Debug.Log("Existing player_id found.");
        }

        playerID = PlayerPrefs.GetString("player_id");
        gameVersion = Application.version;

        Debug.Log("PlayerID: " + playerID);
        Debug.Log("GameVersion: " + gameVersion);
    }

    void StartSession()
    {
        Debug.Log("Starting Session...");

        sessionID = Guid.NewGuid().ToString();
        sessionStartTime = Time.time;
        gameStartTime = Time.time;

        Debug.Log("SessionID: " + sessionID);
        Debug.Log("Session start time: " + sessionStartTime);

        LogEvent("session_start", new SessionData
        {
            session_id = sessionID
        });
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application quitting. Sending session_end.");

        float totalPlayTime = Time.time - sessionStartTime;

        LogEvent("session_end", new SessionEndData
        {
            session_id = sessionID,
            total_play_time = totalPlayTime
        });
    }

    public static void LogEvent(string eventName, object data)
    {
        Debug.Log("Preparing to log event: " + eventName);

        WrappedData wrapped = WrapData(data);
        string json = JsonUtility.ToJson(wrapped);

        Debug.Log("Wrapped JSON: " + json);

#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log("Sending event to JS (WebGL build).");

        try
        {
            SendUnityAnalytics(eventName, json);
            Debug.Log("SendUnityAnalytics called successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError("SendUnityAnalytics FAILED: " + e.Message);
        }
#else
        Debug.Log("[EDITOR MODE] Event simulated: " + eventName);
#endif
    }

    static WrappedData WrapData(object data)
    {
        Debug.Log("Wrapping data for event...");

        return new WrappedData
        {
            player_id = playerID,
            session_id = sessionID,
            game_version = gameVersion,
            payload = JsonUtility.ToJson(data)
        };
    }
}

[Serializable]
public class WrappedData
{
    public string player_id;
    public string session_id;
    public string game_version;
    public string payload;
}

[Serializable]
public class SessionData
{
    public string session_id;
}

[Serializable]
public class SessionEndData
{
    public string session_id;
    public float total_play_time;
}