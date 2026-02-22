using UnityEngine;

public class BookReadable : MonoBehaviour
{
    [TextArea(5, 20)]
    public string content;

    [Header("Optional Dialogue After Reading")]
    public bool triggerDialogueAfterReading = false;

    [TextArea]
    public string[] afterReadingLines;
}
