using UnityEngine;
using System.Collections;

public class WaterDispenserInteract : MonoBehaviour
{
    [Header("Objects")]
    public GameObject realMugModel;
    public GameObject waterInsideCup;
    public GameObject hiddenText;

    [Header("Scene Transition")]
    public Transform tutorialSpawnPoint;
    public GameObject blackScreen;
    public FPSControllerSimple player;
    public DialogueUI dialogueUI;

    bool hasTriggered = false;

    public void Interact()
    {
        if (realMugModel != null && realMugModel.activeInHierarchy && !hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(DrinkRoutine());
        }
    }

    IEnumerator DrinkRoutine()
    {
        Debug.Log("Drinking...");

        // 물 따르는 시간동안 멈추기
        yield return new WaitForSecondsRealtime(2f);

        if (waterInsideCup != null) waterInsideCup.SetActive(true);
        if (hiddenText != null) hiddenText.SetActive(true);

        realMugModel.transform.localPosition =
            new Vector3(-0.006f, -0.063f, 1.002f);
        realMugModel.transform.localRotation =
            Quaternion.Euler(70.941f, 117.214f, -61.782f);
        realMugModel.transform.localScale =
            new Vector3(2.705f, 2.705f, 2.705f);

        yield return new WaitForSecondsRealtime(1.5f);

        /*if (dialogueUI != null)
        {
            dialogueUI.StartDialogue(new string[]
            {
                "Wake up...?",
                "What does it mean?"
            });
        }*/

        yield return new WaitForSecondsRealtime(1f);

        // 🌑 黑屏
        if (blackScreen != null)
            blackScreen.SetActive(true);

        yield return new WaitForSecondsRealtime(2f);

        // 🚪 传送回 tutorial
        if (player != null && tutorialSpawnPoint != null)
        {
            Debug.Log("Spawn position: " + tutorialSpawnPoint.position);
            Debug.Log("Player before: " + player.transform.position);

            CharacterController cc = player.GetComponent<CharacterController>();
            FPSControllerSimple controller = player.GetComponent<FPSControllerSimple>();

            if (cc != null) cc.enabled = false;
            if (controller != null) controller.enabled = false;

            player.transform.position = tutorialSpawnPoint.position;
            player.transform.rotation = tutorialSpawnPoint.rotation;

            if (cc != null) cc.enabled = true;
            if (controller != null) controller.enabled = true;

            Debug.Log("Player after: " + player.transform.position);
        }

        // ☕ 杯子消失
        if (realMugModel != null)
        {
            realMugModel.SetActive(false);
        }

        yield return new WaitForSecondsRealtime(1f);

        // 🌤 黑屏消失（blink）
        if (blackScreen != null)
            blackScreen.SetActive(false);

        yield return new WaitForSecondsRealtime(0.5f);

        // 🌙 梦醒对白
        if (dialogueUI != null)
        {
            dialogueUI.StartDialogue(new string[]
            {
                "Was that a dream...?",
                "I must be seriously sleep-deprived...",
                "Alright. Time to head home.",
                "Better make sure I have everything in my bag before I leave."
            });
        }

        Debug.Log("Dream sequence finished.");
        GameProgress.snackOverflowFinished = true;

        HallwayPaintingsController hallway = FindObjectOfType<HallwayPaintingsController>();

        if (hallway != null)
        {
            hallway.ShowPaintings();
        }
        else
        {
            Debug.LogError("HallwayPaintingsController not found!");
        }
    }
}