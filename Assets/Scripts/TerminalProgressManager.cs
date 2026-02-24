using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TerminalProgressManager : MonoBehaviour
{
    public static TerminalProgressManager Instance { get; private set; }

    [Header("Dialogue")]
    public DialogueUI dialogueUI;

    [Header("3 Computers")]
    public TerminalInteractable[] targetComputers;

    [Header("Progress Screen (4th)")]
    public TMP_Text progressText;

    [Header("Objects To Move When All Solved")]
    public Transform object1;
    public Transform object2;

    // ===== Analytics =====
    string puzzleID = "terminal_cluster_puzzle";
    float puzzleStartTime;
    bool puzzleActive = false;

    int solvedCount = 0;
    TerminalInteractable currentComputer;
    bool hasTriggeredMove = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        RefreshProgress();
    }

    public void OnTerminalAccessed()
    {
        if (puzzleActive) return;

        puzzleActive = true;
        puzzleStartTime = Time.time;

        AnalyticsManager.LogEvent("puzzle_start",
            new Dictionary<string, object>
            {
                { "puzzle_id", puzzleID }
            });
    }

    public void SetCurrentComputer(TerminalInteractable computer)
    {
        currentComputer = computer;
        OnTerminalAccessed();
    }

    public void RegisterTerminalAttempt()
    {
        if (currentComputer == null) return;
        if (currentComputer.IsSolved) return;

        currentComputer.RegisterAttempt();
    }
    public void MarkCurrentComputerSolved()
    {
        if (currentComputer == null) return;
        if (currentComputer.IsSolved) return;

        currentComputer.MarkSolved();

        solvedCount = CountSolved();
        RefreshProgress();

        float timeSincePuzzleStart = Time.time - puzzleStartTime;
        float timeSinceGameStart = Time.time - AnalyticsManager.gameStartTime;

        // 🔥 每个 terminal 单独记录
        AnalyticsManager.LogEvent("terminal_solved",
            new Dictionary<string, object>
            {
                { "puzzle_id", puzzleID },
                { "terminal_id", currentComputer.terminalID },
                { "solved_order", solvedCount },
                { "attempts", currentComputer.GetAttemptCount() },
                { "time_since_puzzle_start", timeSincePuzzleStart },
                { "time_since_game_start", timeSinceGameStart }
            });

        CheckAllSolved();
    }

    int CountSolved()
    {
        int c = 0;
        if (targetComputers == null) return 0;

        foreach (var t in targetComputers)
            if (t != null && t.IsSolved) c++;

        return c;
    }

    void RefreshProgress()
    {
        if (progressText != null)
            progressText.text = $"{solvedCount} / 3";
    }

    void CheckAllSolved()
    {
        if (solvedCount == 3 && !hasTriggeredMove)
        {
            hasTriggeredMove = true;

            float duration = Time.time - puzzleStartTime;

            AnalyticsManager.LogEvent("puzzle_complete",
                new Dictionary<string, object>
                {
                    { "puzzle_id", puzzleID },
                    { "duration", duration },
                    { "total_terminals", 3 }
                });

            if (object1 != null)
                object1.position += new Vector3(0.3f, 0f, 0f);

            if (object2 != null)
                object2.position += new Vector3(0.3f, 0f, 0f);

            if (dialogueUI != null)
            {
                string[] lines = new string[]
                {
                    "Something just popped out...",
                    "Is that... a key?",
                    "There was a locked door in Lab 1.",
                    "Maybe this opens it."
                };

                dialogueUI.StartDialogue(lines);
            }
        }
    }
}