using UnityEngine;

public class TerminalInteractable : MonoBehaviour
{
    [Header("Terminal Content")]
    [TextArea(5, 20)]
    public string terminalHeader = "UW-CS LAB TERMINAL\nAuth required.";
    public string correctAnswer = "token123";

    [Header("Refs")]
    public TerminalUI terminalUI; // 拖同一个 TerminalUI（全场共用）

    [Header("Screen")]
    public GameObject screenObject;

    bool solved;

    public bool IsSolved => solved;

    // 被玩家脚本调用
    public void Interact(FPSControllerSimple player)
    {
        if (solved) return;
        if (terminalUI == null)
        {
            Debug.LogWarning("TerminalUI not assigned on " + name);
            return;
        }

        // 告诉进度管理器“当前打开的是哪台”
        TerminalProgressManager.Instance?.SetCurrentComputer(this);

        terminalUI.Open(player, terminalHeader, correctAnswer);
    }

    public void MarkSolved()
    {
        solved = true;
        // ✅ 密码正确后，关掉这个屏幕
        if (screenObject != null)
            screenObject.SetActive(false);
    }
}
