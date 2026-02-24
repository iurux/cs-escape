using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ElevatorButtonInteract : MonoBehaviour
{
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
        else
        {
            triggered = true;

            // 🔥 记录 game_complete
            OnGameComplete();

            StartCoroutine(EndSequence());
        }
    }

    void OnGameComplete()
    {
        float totalTime = Time.time - AnalyticsManager.gameStartTime;

        int collectedCount = 0;

        // 如果 ChecklistUI 有这个方法就用它
        if (checklistUI != null)
        {
            collectedCount = checklistUI.TotalCollectedCount();
        }

        AnalyticsManager.LogEvent("game_complete", new GameCompleteData
        {
            total_time = totalTime,
            total_items_collected = collectedCount
        });

        Debug.Log("Game Complete Logged");
    }

    IEnumerator EndSequence()
    {
        if (blackScreen != null)
            blackScreen.SetActive(true);

        yield return new WaitForSecondsRealtime(2f);

        if (endText != null)
            endText.SetActive(true);

        yield return new WaitForSecondsRealtime(3f);

        SceneManager.LoadScene("OpeningScene");
    }
}

[System.Serializable]
public class GameCompleteData
{
    public float total_time;
    public int total_items_collected;
}