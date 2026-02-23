using UnityEngine;

public class SnackOverFlowEnterDialogueTrigger : MonoBehaviour
{
    public DialogueUI dialogueUI;

    bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;

            if (dialogueUI != null)
            {
                string[] lines = new string[]
                {
                    "...I’m getting pretty thirsty.",
                    "I should get some water.",
                    "Maybe I can use that mug over there...",
                    "...and where is the water dispenser?"
                };

                dialogueUI.StartDialogue(lines);
            }
        }
    }
}