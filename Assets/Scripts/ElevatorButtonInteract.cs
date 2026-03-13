using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ElevatorButtonInteract : MonoBehaviour
{
    [Header("Sound Settings")]
    public AudioSource sfxSource;       // 사운드를 재생할 소스
    public AudioClip elevatorOpenClip;  // 엘리베이터 문 열리는 소리

    [Header("UI & Systems")]
    public ChecklistUI checklistUI;
    public DialogueUI dialogueUI;
    public GameObject blackScreen;
    public GameObject endText;

    private bool triggered = false;

    public void Interact()
    {
        if (triggered) return;

        if (checklistUI == null)
        {
            Debug.LogError("ChecklistUI not assigned!");
            return;
        }

        // 1. 아이템을 다 모으지 못한 경우
        if (!checklistUI.AllCollected())
        {
            if (dialogueUI != null)
            {
                dialogueUI.StartDialogue(new string[]
                {
                    "Something’s not right...",
                    "Better check the checklist again."
                });
            }
        }
        // 2. 모든 조건을 만족하여 탈출하는 경우
        else
        {
            triggered = true;

            // [사운드 재생] 문 열리는 소리 한 번 출력
            if (sfxSource != null && elevatorOpenClip != null)
            {
                sfxSource.PlayOneShot(elevatorOpenClip);
            }

            OnGameComplete();
            StartCoroutine(EndSequence());
        }
    }

    void OnGameComplete()
    {
        float totalTime = Time.time - AnalyticsManager.gameStartTime;
        int collectedCount = 0;

        if (checklistUI != null)
        {
            collectedCount = checklistUI.TotalCollectedCount();
        }

        AnalyticsManager.LogEvent("game_complete",
            new Dictionary<string, object>
            {
                { "time_spent", totalTime },
                { "total_items_collected", collectedCount },
                { "completion_state", "all_items_collected" },
                { "room_id", SceneManager.GetActiveScene().name }
            });

        Debug.Log("Game Complete Logged");
    }

    IEnumerator EndSequence()
    {
        if (blackScreen != null)
            blackScreen.SetActive(true);

        // 사운드가 울려 퍼질 시간을 고려하여 2초 대기
        yield return new WaitForSecondsRealtime(2f);

        if (endText != null)
            endText.SetActive(true);

        yield return new WaitForSecondsRealtime(3f);

        SceneManager.LoadScene("OpeningScene");
    }
}