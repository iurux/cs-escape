using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("Main UI")]
    public GameObject panel;
    public TMP_Text dialogueText;
    public TMP_Text continueHint;

    [Header("History UI")]
    public GameObject historyPanel;
    public TMP_Text historyText;
    public ScrollRect historyScrollRect;

    [Header("Input")]
    public Key advanceKey = Key.Space;

    [Header("Blink")]
    public float blinkSpeed = 0.6f;

    [Header("History Settings")]
    public int maxHistoryEntries = 100;

    bool waitingForInput;
    Coroutine blinkRoutine;
    Coroutine dialogueRoutine;
    Coroutine scrollRoutine;

    public bool IsPlaying { get; private set; }
    public FPSControllerSimple player;

    private List<string> dialogueHistory = new List<string>();

    void Awake()
    {
        if (panel != null)
            panel.SetActive(false);

        if (continueHint != null)
            continueHint.gameObject.SetActive(false);

        if (historyPanel != null)
            historyPanel.SetActive(false);
    }

    /// <summary>
    /// The only public method for starting dialogue
    /// </summary>
    public void StartDialogue(string[] lines)
    {
        if (dialogueRoutine != null)
            StopCoroutine(dialogueRoutine);

        dialogueRoutine = StartCoroutine(PlayDialogue(lines));
    }

    private IEnumerator PlayDialogue(string[] lines)
    {
        bool forceRead = ABTestManager.Variant == "B";

        IsPlaying = forceRead;

        if (forceRead && player != null)
            player.SetUILock(true);

        if (panel != null)
            panel.SetActive(true);

        yield return null;

        if (dialogueText != null)
        {
            Color tc = dialogueText.color;
            tc.a = 1f;
            dialogueText.color = tc;
        }

        for (int i = 0; i < lines.Length; i++)
        {
            if (dialogueText != null)
                dialogueText.text = lines[i];

            AddToHistory(lines[i]);

            yield return null;

            if (i < lines.Length - 1)
                yield return WaitForAdvance();
            else
                yield return new WaitForSeconds(2f);
        }

        if (dialogueText != null)
            dialogueText.text = "";

        if (continueHint != null)
            continueHint.gameObject.SetActive(false);

        if (panel != null)
            panel.SetActive(false);

        if (forceRead && player != null)
            player.SetUILock(false);

        IsPlaying = false;
        dialogueRoutine = null;
    }

    IEnumerator WaitForAdvance()
    {
        waitingForInput = true;

        if (continueHint != null)
            continueHint.gameObject.SetActive(true);

        blinkRoutine = StartCoroutine(BlinkHint());

        while (waitingForInput)
        {
            bool advanceByKeyboard =
                Keyboard.current != null &&
                Keyboard.current[advanceKey].wasPressedThisFrame;

            bool advanceByMouse =
                (historyPanel == null || !historyPanel.activeSelf) &&
                Mouse.current != null &&
                Mouse.current.leftButton.wasPressedThisFrame;

            if (advanceByKeyboard || advanceByMouse)
            {
                waitingForInput = false;
            }

            yield return null;
        }

        if (blinkRoutine != null)
            StopCoroutine(blinkRoutine);

        if (continueHint != null)
            continueHint.gameObject.SetActive(false);
    }

    IEnumerator BlinkHint()
    {
        while (true)
        {
            if (continueHint != null)
                continueHint.alpha = 1f;

            yield return new WaitForSeconds(blinkSpeed);

            if (continueHint != null)
                continueHint.alpha = 0f;

            yield return new WaitForSeconds(blinkSpeed);
        }
    }

    private void AddToHistory(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return;

        dialogueHistory.Add(line);

        if (dialogueHistory.Count > maxHistoryEntries)
            dialogueHistory.RemoveAt(0);

        RefreshHistoryUI();
    }

    private void RefreshHistoryUI()
    {
        if (historyText == null)
            return;

        historyText.text = string.Join("\n\n", dialogueHistory);

        LayoutRebuilder.ForceRebuildLayoutImmediate(historyText.rectTransform);
        Canvas.ForceUpdateCanvases();
    }

    public void OpenHistory()
    {
        Debug.Log("OpenHistory called");

        if (historyPanel == null)
        {
            Debug.LogError("historyPanel is NULL");
            return;
        }

        historyPanel.SetActive(true);
        Debug.Log("historyPanel active = " + historyPanel.activeSelf);

        RefreshHistoryUI();

        if (player != null)
            player.SetUILock(true);

        if (scrollRoutine != null)
            StopCoroutine(scrollRoutine);

        scrollRoutine = StartCoroutine(ScrollToBottomNextFrame());
    }

    public void CloseHistory()
    {
        if (historyPanel != null)
            historyPanel.SetActive(false);

        if (scrollRoutine != null)
        {
            StopCoroutine(scrollRoutine);
            scrollRoutine = null;
        }

        if (player != null && !IsPlaying)
            player.SetUILock(false);
    }

    public void ToggleHistory()
    {
        if (historyPanel == null)
            return;

        bool isOpen = historyPanel.activeSelf;

        if (isOpen)
            CloseHistory();
        else
            OpenHistory();
    }

    public void ClearHistory()
    {
        dialogueHistory.Clear();
        RefreshHistoryUI();

        if (scrollRoutine != null)
            StopCoroutine(scrollRoutine);

        scrollRoutine = StartCoroutine(ScrollToBottomNextFrame());
    }

    private IEnumerator ScrollToBottomNextFrame()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();

        if (historyScrollRect != null)
        {
            historyScrollRect.verticalNormalizedPosition = 0f;
        }

        scrollRoutine = null;
    }
}