using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TerminalUI : MonoBehaviour
{
    [Header("UI Refs")]
    public GameObject rootPanel;          // TerminalCanvas 或里面的 Panel 根节点
    public TMP_Text terminalText;         // 历史输出
    public TMP_InputField inputField;     // 输入行（自带闪烁竖光标）

    [Header("ControlCanvas")]
    public GameObject controlCanvas;

    [Header("Behavior")]
    public string successLine = "ACCESS GRANTED";
    public Key closeKey = Key.Escape;

    bool isOpen;
    public bool IsOpen => isOpen;
    string expectedAnswer;
    FPSControllerSimple currentPlayer; // 用于恢复鼠标/时间等（可选）

    void Awake()
    {
        //if (rootPanel != null) rootPanel.SetActive(false);

        // 确保回车提交时走 OnSubmit
        inputField.onSubmit.RemoveAllListeners();
        inputField.onSubmit.AddListener(OnSubmit);
    }

    void Update()
    {
        if (!isOpen) return;

        // if (Keyboard.current != null && Keyboard.current[closeKey].wasPressedThisFrame)
        // {
        //     Close();
        // }
        
        // 如果玩家点鼠标，强制把焦点拉回输入框
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            inputField.ActivateInputField();
            inputField.Select();
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

        // ✅ 锁住玩家控制（相机/移动）
        if (currentPlayer != null) currentPlayer.SetUILock(true);

        // UI 先开（更稳定）
        rootPanel.SetActive(true);

        terminalText.text = headerContent.TrimEnd() + "\n\n> ";
        inputField.text = "";

        // Pause world
        Time.timeScale = 0f;
        controlCanvas.SetActive(false);

        // 鼠标放开
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // ✅ 下一帧再强制聚焦（避免第一次不生效/要点第二次）
        StartCoroutine(FocusInputNextFrame());
    }

    System.Collections.IEnumerator FocusInputNextFrame()
    {
        yield return null; // 等一帧（不受 timeScale 影响）
        inputField.ActivateInputField();
        inputField.Select();
    }

    public void Close()
    {
        if (!isOpen) return;

        isOpen = false;
        expectedAnswer = null;

        if (rootPanel != null) rootPanel.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // ✅ 恢复玩家控制
        controlCanvas.SetActive(true);
        if (currentPlayer != null) currentPlayer.SetUILock(false);
        currentPlayer = null;
    }

    void OnSubmit(string value)
    {
        if (!isOpen) return;

        string typed = (value ?? "").Trim();

        if (typed == "quit")
        {
            Close();
            return;
        }

        // 🔥 每次提交都记录一次 attempt
        TerminalProgressManager.Instance?.RegisterTerminalAttempt();

        if (typed == expectedAnswer)
        {
            terminalText.text += typed + "\n" + successLine + "\n\n> ";
            inputField.text = "";
            inputField.ActivateInputField();
            // ✅ 把“密码”改成 quit（下一步必须输入 quit 才能退出）
            expectedAnswer = "quit";

            TerminalProgressManager.Instance?.MarkCurrentComputerSolved();
        }
        else
        {
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }
}
