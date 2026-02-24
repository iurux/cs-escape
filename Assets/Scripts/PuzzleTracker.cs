using UnityEngine;

public class PuzzleTracker : MonoBehaviour
{
    public string puzzleID;
    public string roomID;

    private float puzzleEnterTime;
    private int attemptCount;
    private float idleTime;
    private float lastInputTime;

    public void EnterPuzzle()
    {
        puzzleEnterTime = Time.time;
        attemptCount = 0;
        idleTime = 0;
        lastInputTime = Time.time;

        AnalyticsManager.LogEvent("puzzle_enter", new PuzzleData
        {
            room_id = roomID,
            puzzle_id = puzzleID
        });
    }

    public void Attempt(bool success)
    {
        attemptCount++;
        lastInputTime = Time.time;

        AnalyticsManager.LogEvent("puzzle_attempt", new AttemptData
        {
            room_id = roomID,
            puzzle_id = puzzleID,
            attempt_index = attemptCount,
            success = success
        });

        if (success)
            CompletePuzzle();
    }

    void Update()
    {
        if (Time.time - lastInputTime > 30f)
        {
            idleTime += Time.time - lastInputTime;
            lastInputTime = Time.time;

            AnalyticsManager.LogEvent("player_idle", new IdleData
            {
                room_id = roomID,
                puzzle_id = puzzleID,
                idle_duration = 30f
            });
        }
    }

    void CompletePuzzle()
    {
        float totalTime = Time.time - puzzleEnterTime;

        AnalyticsManager.LogEvent("puzzle_complete", new CompleteData
        {
            room_id = roomID,
            puzzle_id = puzzleID,
            total_time = totalTime,
            total_attempts = attemptCount,
            total_idle_time = idleTime
        });
    }
}

[System.Serializable]
public class PuzzleData
{
    public string room_id;
    public string puzzle_id;
}

[System.Serializable]
public class AttemptData
{
    public string room_id;
    public string puzzle_id;
    public int attempt_index;
    public bool success;
}

[System.Serializable]
public class CompleteData
{
    public string room_id;
    public string puzzle_id;
    public float total_time;
    public int total_attempts;
    public float total_idle_time;
}

[System.Serializable]
public class IdleData
{
    public string room_id;
    public string puzzle_id;
    public float idle_duration;
}