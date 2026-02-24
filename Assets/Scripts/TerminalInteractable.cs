using UnityEngine;

public class TerminalInteractable : MonoBehaviour
{
    [Header("Analytics")]
    public string terminalID;

    [Header("Terminal Content")]
    [TextArea(5, 20)]
    public string terminalHeader = "UW-CS LAB TERMINAL\nAuth required.";
    public string correctAnswer = "token123";

    [Header("Refs")]
    public TerminalUI terminalUI;

    [Header("Screen")]
    public GameObject screenObject;

    bool solved;
    int attemptCount = 0;   // 🔥 记录尝试次数

    public bool IsSolved => solved;

    public void Interact(FPSControllerSimple player)
    {
        if (solved) return;

        if (terminalUI == null)
        {
            Debug.LogWarning("TerminalUI not assigned on " + name);
            return;
        }

        TerminalProgressManager.Instance?.SetCurrentComputer(this);

        terminalUI.Open(player, terminalHeader, correctAnswer);
    }

    // 🔥 每次输入提交时调用
    public void RegisterAttempt()
    {
        if (solved) return;
        attemptCount++;
    }

    public int GetAttemptCount()
    {
        return attemptCount;
    }

    public void MarkSolved()
    {
        solved = true;

        if (screenObject != null)
            screenObject.SetActive(false);
    }
}