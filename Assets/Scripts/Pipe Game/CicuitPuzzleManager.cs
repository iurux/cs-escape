using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class CircuitPuzzleManager : MonoBehaviour
{
    //[Header("UI 참조")]
    public GameObject puzzleCanvasPanel; // 퍼즐 UI가 담긴 패널
    public Button closeButton;           // 닫기 버튼

    //[Header("시스템 참조")]
    public PowerSystem powerSystem;      // 전력 복구 시스템

    //[Header("상태")]
    public bool IsSolved { get; private set; } = false; // 퍼즐 해결 여부

    //[Header("화면 피드백")]
    public GameObject screenFlicker;     // 퍼즐 해결 시 화면 깜빡임 효과

    // ===== 분석 데이터 (Analytics) =====
    float puzzleStartTime;      // 퍼즐 시작 시간
    int attemptCount = 0;       // 클릭(시도) 횟수
    int totalRotateCount = 0;   // 총 회전 횟수
    string puzzleID = "circuit_puzzle"; // 퍼즐 식별자
    bool puzzleActive = false;  // 현재 퍼즐이 활성화 상태인지 확인

    private List<PipeRotator> allPipes = new List<PipeRotator>();

    private void Start()
    {
        // 자식 오브젝트들에서 모든 PipeRotator를 찾아 리스트에 추가
        PipeRotator[] pipes = GetComponentsInChildren<PipeRotator>();
        foreach (var pipe in pipes)
        {
            allPipes.Add(pipe);
            pipe.RandomizeRotation(); // 시작 시 무작위 회전 설정
        }

        // 닫기 버튼 이벤트 연결
        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePuzzle);

        // 시작 시 퍼즐 패널은 비활성화
        puzzleCanvasPanel.SetActive(false);
    }

    void Update()
    {
        if (!puzzleActive) return;

        // Q 키를 눌러 퍼즐 나가기 기능 (새로운 Input System 사용)
        if (UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.qKey.wasPressedThisFrame)
        {
            FPSControllerSimple player = FindObjectOfType<FPSControllerSimple>();

            if (player != null)
            {
                player.ClosePuzzleAndResumeGame(); // 플레이어 컨트롤러를 통해 퍼즐 닫기
            }
            else
            {
                ClosePuzzle(); // 일반 닫기
            }
        }
    }

    // 퍼즐 시작 시 호출
    public void OpenPuzzle()
    {
        if (IsSolved) return; // 이미 해결했다면 열지 않음

        puzzleCanvasPanel.SetActive(true);

        // 분석용 데이터 초기화
        puzzleStartTime = Time.time;
        totalRotateCount = 0;
        attemptCount = 0;
        puzzleActive = true;   // 퍼즐 시작됨을 마킹

        // 분석 서버에 퍼즐 시작 이벤트 로그 전송
        AnalyticsManager.LogEvent("puzzle_start",
            new Dictionary<string, object>
            {
                { "puzzle_id", puzzleID }
            });

        // 마우스 커서 해제 및 표시
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // 파이프가 회전할 때마다 카운트를 등록
    public void RegisterPipeRotation(PipeRotator pipe)
    {
        if (!puzzleActive) return;

        totalRotateCount++;
    }

    // 퍼즐 창을 닫을 때 호출
    public void ClosePuzzle()
    {
        // 퍼즐을 풀지 않고 나갈 때 분석 데이터 전송
        if (puzzleActive && !IsSolved)
        {
            float duration = Time.time - puzzleStartTime;

            AnalyticsManager.LogEvent("puzzle_exit",
                new Dictionary<string, object>
                {
                    { "puzzle_id", puzzleID },
                    { "time_spent", duration },
                    { "attempts", attemptCount },
                    { "total_rotations", totalRotateCount },
                    { "exit_reason", "manual_exit" }
                });
        }

        puzzleActive = false;  // 상태 초기화

        puzzleCanvasPanel.SetActive(false);
    }

    // 승리 조건(모든 파이프의 방향) 확인
    public void CheckWinCondition()
    {
        if (!puzzleActive) return;  // 비정상적인 호출 방지

        attemptCount++;

        // 하나라도 틀린 파이프가 있으면 리턴
        foreach (var pipe in allPipes)
        {
            if (!pipe.IsCorrect()) return;
        }

        // 모두 정답이면 해결 함수 호출
        OnPuzzleSolved();
    }

    // 퍼즐을 완벽히 풀었을 때 실행되는 로직
    private void OnPuzzleSolved()
    {
        if (IsSolved) return;

        IsSolved = true;
        puzzleActive = false;  // 종료 상태로 변경

        float duration = Time.time - puzzleStartTime;

        // 분석 서버에 퍼즐 완료 이벤트 로그 전송
        AnalyticsManager.LogEvent("puzzle_complete",
            new Dictionary<string, object>
            {
                { "puzzle_id", puzzleID },
                { "time_spent", duration },
                { "attempts", attemptCount },
                { "total_rotations", totalRotateCount }
            });

        // 화면 피드백(깜빡임) 활성화
        if (screenFlicker != null)
            screenFlicker.SetActive(true);

        // 모든 파이프 상호작용 비활성화
        foreach (var pipe in allPipes)
            pipe.enabled = false;

        // 승리 연출 코루틴 시작
        StartCoroutine(WinSequence());
    }

    // 승리 시 연출 및 게임 복귀 시퀀스
    private IEnumerator WinSequence()
    {
        yield return new WaitForSeconds(0.5f); // 잠시 대기

        // 전력 시스템 복구 호출
        if (powerSystem != null)
            powerSystem.RestorePower();

        // 플레이어 상태 복구 및 퍼즐 창 닫기
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