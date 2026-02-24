using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class CircuitPuzzleManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject puzzleCanvasPanel;
    public Button closeButton;

    [Header("System References")]
    public PowerSystem powerSystem;

    [Header("Status")]
    public bool IsSolved { get; private set; } = false;

    [Header("Screen Feedback")]
    public GameObject screenFlicker;

    // ===== Analytics =====
    float puzzleStartTime;
    int attemptCount = 0;
    int totalRotateCount = 0;
    string puzzleID = "circuit_puzzle";
    bool puzzleActive = false;   // 🔥 关键修复

    private List<PipeRotator> allPipes = new List<PipeRotator>();

    private void Start()
    {
        PipeRotator[] pipes = GetComponentsInChildren<PipeRotator>();
        foreach (var pipe in pipes)
        {
            allPipes.Add(pipe);
            pipe.RandomizeRotation();
        }

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePuzzle);

        puzzleCanvasPanel.SetActive(false);
    }

    public void OpenPuzzle()
    {
        if (IsSolved) return;

        puzzleCanvasPanel.SetActive(true);

        puzzleStartTime = Time.time;
        totalRotateCount = 0;
        attemptCount = 0;
        puzzleActive = true;   // 🔥 标记已开始

        AnalyticsManager.LogEvent("puzzle_start",
            new Dictionary<string, object>
            {
                { "puzzle_id", puzzleID }
            });

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RegisterPipeRotation(PipeRotator pipe)
    {
        if (!puzzleActive) return;

        totalRotateCount++;
    }
    public void ClosePuzzle()
    {
        if (puzzleActive && !IsSolved)
        {
            float duration = Time.time - puzzleStartTime;

            AnalyticsManager.LogEvent("puzzle_exit",
                new Dictionary<string, object>
                {
                    { "puzzle_id", puzzleID },
                    { "attempts", attemptCount },
                    { "duration", duration }
                });
        }

        puzzleActive = false;  // 🔥 重置状态

        puzzleCanvasPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void CheckWinCondition()
    {
        if (!puzzleActive) return;  // 🔥 防止异常调用

        attemptCount++;

        foreach (var pipe in allPipes)
        {
            if (!pipe.IsCorrect()) return;
        }

        OnPuzzleSolved();
    }

    private void OnPuzzleSolved()
    {
        if (IsSolved) return;

        IsSolved = true;
        puzzleActive = false;  // 🔥 结束状态

        float duration = Time.time - puzzleStartTime;

        AnalyticsManager.LogEvent("puzzle_complete",
            new Dictionary<string, object>
            {
                { "puzzle_id", puzzleID },
                { "attempts", attemptCount },
                { "duration", duration },
                { "total_rotations", totalRotateCount } 
            });

        if (screenFlicker != null)
            screenFlicker.SetActive(true);

        foreach (var pipe in allPipes)
            pipe.enabled = false;

        StartCoroutine(WinSequence());
    }

    private IEnumerator WinSequence()
    {
        yield return new WaitForSeconds(0.5f);

        if (powerSystem != null)
            powerSystem.RestorePower();

        FPSControllerSimple player = FindObjectOfType<FPSControllerSimple>();
        if (player != null)
        {
            player.ClosePuzzleAndResumeGame();
        }
        else
        {
            ClosePuzzle();
        }
    }
}