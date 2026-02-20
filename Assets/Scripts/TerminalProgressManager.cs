using UnityEngine;
using TMPro;

public class TerminalProgressManager : MonoBehaviour
{
    public static TerminalProgressManager Instance { get; private set; }

    [Header("3 Computers")]
    public TerminalInteractable[] targetComputers; // 3 台电脑拖进来

    [Header("Progress Screen (4th)")]
    public TMP_Text progressText; // 第4台屏幕上的 TMP_Text（显示 0/3）

    [Header("Objects To Move When All Solved")]
    public Transform object1;
    public Transform object2;

    int solvedCount = 0;
    TerminalInteractable currentComputer;
    bool hasTriggeredMove = false; // 防止重复触发

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        RefreshProgress();
    }

    public void SetCurrentComputer(TerminalInteractable computer)
    {
        currentComputer = computer;
    }

    public void MarkCurrentComputerSolved()
    {
        if (currentComputer == null) return;
        if (currentComputer.IsSolved) return;

        currentComputer.MarkSolved();
        solvedCount = CountSolved();
        RefreshProgress();

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

            if (object1 != null)
                object1.position += new Vector3(0.3f, 0f, 0f);

            if (object2 != null)
                object2.position += new Vector3(0.3f, 0f, 0f);
        }
    }
}
