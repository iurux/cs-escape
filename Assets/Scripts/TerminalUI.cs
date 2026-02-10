using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TerminalUI : MonoBehaviour
{
    [Header("UI Refs")]
    public GameObject rootPanel;          // TerminalCanvas 或里面的 Panel 根节点
    public TMP_Text terminalText;         // 历史输出
    public TMP_InputField inputField;     // 输入行（自带闪烁竖光标）

    [Header("Behavior")]
    public string successLine = "ACCESS GRANTED";
    public Key closeKey = Key.Escape;

    bool isOpen;
    string expectedAnswer;
    FPSControllerSimple currentPlayer; // 用于恢复鼠标/时间等（可选）

    void Awake()
    {
        if (rootPanel != null) rootPanel.SetActive(false);

        // 确保回车提交时走 OnSubmit
        inputField.onSubmit.RemoveAllListeners();
        inputField.onSubmit.AddListener(OnSubmit);
    }

    void Update()
    {
        if (!isOpen) return;

        if (Keyboard.current != null && Keyboard.current[closeKey].wasPressedThisFrame)
        {
            Close();
        }
    }

    // 由电脑调用
    public void Open(FPSControllerSimple player, string headerContent, string expected)
    {
        if (rootPanel == null || terminalText == null || inputField == null)
        {
            Debug.LogWarning("TerminalUI refs missing.");
            return;
        }

        isOpen = true;
        currentPlayer = player;
        expectedAnswer = expected;

        // 暂停世界
        Time.timeScale = 0f;

        // 解锁鼠标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 显示UI
        rootPanel.SetActive(true);

        // 写入终端初始内容（每台电脑不同）
        terminalText.text = headerContent.TrimEnd() + "\n\n> ";

        // 清空并聚焦输入（会显示闪烁光标）
        inputField.text = "";
        inputField.ActivateInputField();
        inputField.Select();
    }

    public void Close()
    {
        if (!isOpen) return;

        isOpen = false;
        expectedAnswer = null;

        if (rootPanel != null) rootPanel.SetActive(false);

        // 恢复世界
        Time.timeScale = 1f;

        // 恢复鼠标锁定
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentPlayer = null;
    }

    void OnSubmit(string value)
    {
        if (!isOpen) return;

        // 注意：TMP_InputField onSubmit 只有在回车时触发（Single Line）
        string typed = (value ?? "").Trim();

        if (typed == expectedAnswer)
        {
            terminalText.text += typed + "\n" + successLine + "\n\n> ";
            inputField.text = "";
            inputField.ActivateInputField();

            // 通知当前电脑“已解锁”
            TerminalProgressManager.Instance?.MarkCurrentComputerSolved();
        }
        else
        {
            // 错误：不输出任何提示；清空让玩家重新输入
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }
}
