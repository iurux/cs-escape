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
    bool puzzleActive = false;   // 🔥 只有真正进入才为 true

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
        RefreshProgress();   // ❌ 不再自动开始 puzzle
    }

    // 🔥 当玩家 interact 任意 terminal 时调用
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

        // 🔥 第一次访问 terminal 时启动 puzzle
        OnTerminalAccessed();
    }

    public void MarkCurrentComputerSolved()
    {
        if (currentComputer == null) return;
        if (currentComputer.IsSolved) return;

        currentComputer.MarkSolved();

        solvedCount = CountSolved();
        RefreshProgress();

        AnalyticsManager.LogEvent("terminal_solved",
            new Dictionary<string, object>
            {
                { "puzzle_id", puzzleID },
                { "solved_count", solvedCount }
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