using UnityEngine;

public class Lab1ExitDialogueTrigger : MonoBehaviour
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
                    "Finally got out.",
                    "Okay... now let's check the other lab...",
                    "Still missing some stuff."
                };

                dialogueUI.StartDialogue(lines);
            }
        }
    }
}