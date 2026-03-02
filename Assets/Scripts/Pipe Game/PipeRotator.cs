using UnityEngine;
using UnityEngine.EventSystems;

public class PipeRotator : MonoBehaviour, IPointerClickHandler
{
    public float[] correctRotations; // 정답으로 인정되는 각도 배열

    [SerializeField] private bool isPlacedCorrectly = false; // 현재 올바른 방향으로 배치되었는지 여부

    private RectTransform rectTransform;
    private CircuitPuzzleManager puzzleManager;

    private int rotateCount = 0;   // 개별 파이프의 회전 횟수 기록

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        puzzleManager = GetComponentInParent<CircuitPuzzleManager>();
    }

    // 파이프의 회전을 무작위로 설정하는 함수
    public void RandomizeRotation()
    {
        // 0, 90, 180, 270도 중 하나로 무작위 회전
        float randomZ = Random.Range(0, 4) * 90f;
        rectTransform.localEulerAngles = new Vector3(0, 0, randomZ);

        // 회전 후 정답 여부 체크
        CheckCorrection();
    }

    // 클릭 시 호출되는 함수 (IPointerClickHandler 인터페이스 구현)
    public void OnPointerClick(PointerEventData eventData)
    {
        if (puzzleManager == null) return;
        if (puzzleManager.IsSolved) return; // 이미 퍼즐이 풀렸다면 무시

        // 시계 방향으로 90도 회전
        rectTransform.Rotate(0, 0, -90f);

        rotateCount++; 

        // 매니저에게 총 회전 횟수 누적을 알림
        puzzleManager.RegisterPipeRotation(this);

        // 정답 여부 체크
        CheckCorrection();

        // 전체 퍼즐 승리 조건 체크
        puzzleManager.CheckWinCondition();
    }

    // 현재 각도가 정답 범위에 있는지 확인하는 함수
    private void CheckCorrection()
    {
        float currentZ = rectTransform.localEulerAngles.z;
        currentZ = Mathf.Round(currentZ); // 소수점 반올림

        // 각도를 0~359도 사이로 보정
        if (currentZ >= 360) currentZ -= 360;
        if (currentZ < 0) currentZ += 360;

        isPlacedCorrectly = false;

        // 정답 각도 배열 중 하나라도 일치하는지 확인
        foreach (float target in correctRotations)
        {
            if (Mathf.Abs(currentZ - target) < 1f)
            {
                isPlacedCorrectly = true;
                break;
            }
        }
    }

    // 외부에서 현재 정답 여부를 가져올 수 있는 함수
    public bool IsCorrect()
    {
        return isPlacedCorrectly;
    }

    // 외부에서 이 파이프의 총 회전 횟수를 가져올 수 있는 함수
    public int GetRotateCount()
    {
        return rotateCount;
    }
}