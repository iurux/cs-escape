using UnityEngine;
using System.Collections;

public class WaterDispenserInteract : MonoBehaviour
{
    public AudioSource sfxSource;       //  효과음 추가
    public AudioClip waterPourClip;     // 효과음 추가

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
        // PlayOneShot 대신 직접 할당하여 Play (중지 가능하게 함)
        if (sfxSource != null && waterPourClip != null)
        {
            sfxSource.clip = waterPourClip;
            sfxSource.Play();
        }

        // 물 따르는 시간동안 멈추기
        yield return new WaitForSecondsRealtime(2f);

        // 텍스트가 보이거나 다음 단계로 넘어갈 때 소리 중지
        if (sfxSource != null)
        {
            sfxSource.Stop();
        }

        // 물과 텍스트가 나타남
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