using UnityEngine;
using TMPro;

public class ChecklistItemUI : MonoBehaviour
{
    public TMP_Text text;
    public string itemName;

    bool completed = false;

    void Start()
    {
        text.text = itemName;
    }

    public void MarkCompleted()
    {
        if (completed) return;

        completed = true;
        UpdateVisual();
    }

    public bool IsCompleted()
    {
        return completed;
    }

    public void UpdateVisual()
    {
        if (!completed) return;

        text.color = new Color(1, 1, 1, 0); // 直接隐藏
    }

    public string GetItemId()
    {
        return itemName;
    }

    public void ResetItem()
    {
        completed = false;
        UpdateVisual();
    }
}
