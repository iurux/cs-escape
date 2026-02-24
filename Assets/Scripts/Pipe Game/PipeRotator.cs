using UnityEngine;
using UnityEngine.EventSystems;

public class PipeRotator : MonoBehaviour, IPointerClickHandler
{
    [Header("Settings")]
    public float[] correctRotations;

    [Header("Debug")]
    [SerializeField] private bool isPlacedCorrectly = false;

    private RectTransform rectTransform;
    private CircuitPuzzleManager puzzleManager;

    private int rotateCount = 0;   // 🔥 记录单个pipe旋转次数

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        puzzleManager = GetComponentInParent<CircuitPuzzleManager>();
    }

    public void RandomizeRotation()
    {
        float randomZ = Random.Range(0, 4) * 90f;
        rectTransform.localEulerAngles = new Vector3(0, 0, randomZ);

        CheckCorrection();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (puzzleManager == null) return;
        if (puzzleManager.IsSolved) return;

        // Rotate 90 degrees clockwise
        rectTransform.Rotate(0, 0, -90f);

        rotateCount++;   // 🔥 单pipe计数

        // 🔥 通知manager累计总旋转次数
        puzzleManager.RegisterPipeRotation(this);

        CheckCorrection();

        puzzleManager.CheckWinCondition();
    }

    private void CheckCorrection()
    {
        float currentZ = rectTransform.localEulerAngles.z;
        currentZ = Mathf.Round(currentZ);

        if (currentZ >= 360) currentZ -= 360;
        if (currentZ < 0) currentZ += 360;

        isPlacedCorrectly = false;

        foreach (float target in correctRotations)
        {
            if (Mathf.Abs(currentZ - target) < 1f)
            {
                isPlacedCorrectly = true;
                break;
            }
        }
    }

    public bool IsCorrect()
    {
        return isPlacedCorrectly;
    }

    public int GetRotateCount()
    {
        return rotateCount;
    }
}