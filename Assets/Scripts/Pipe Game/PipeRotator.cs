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

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        puzzleManager = GetComponentInParent<CircuitPuzzleManager>();
    }

    // Called by manager to scramble puzzle
    public void RandomizeRotation()
    {
        float randomZ = Random.Range(0, 4) * 90f;
        rectTransform.localEulerAngles = new Vector3(0, 0, randomZ);

        CheckCorrection();
    }

    // Mouse click on pipe
    public void OnPointerClick(PointerEventData eventData)
    {
        if (puzzleManager == null) return;
        if (puzzleManager.IsSolved) return;

        // Rotate 90 degrees clockwise
        rectTransform.Rotate(0, 0, -90f);

        CheckCorrection();

        // Let manager handle attempt counting + win check
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
}