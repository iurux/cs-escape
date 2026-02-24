using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ElevatorButtonInteract : MonoBehaviour
{
    public ChecklistUI checklistUI;
    public DialogueUI dialogueUI;

    public GameObject blackScreen;
    public GameObject endText;

    bool triggered = false;

    public void Interact()
    {
        if (triggered) return;

        if (!checklistUI.AllCollected())
        {
            dialogueUI.StartDialogue(new string[]
            {
                "Something’s not right...",
                "Better check the checklist again."
            });
        }
        else
        {
            triggered = true;
            StartCoroutine(EndSequence());
        }
    }

    IEnumerator EndSequence()
    {
        blackScreen.SetActive(true);

        yield return new WaitForSecondsRealtime(2f);

        endText.SetActive(true);

        yield return new WaitForSecondsRealtime(3f);

        SceneManager.LoadScene("OpeningScene");
    }
}