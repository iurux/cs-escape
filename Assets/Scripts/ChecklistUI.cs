using System.Collections.Generic;
using UnityEngine;

public class ChecklistUI : MonoBehaviour
{
    public ChecklistItemUI[] checklistItems;

    Dictionary<string, ChecklistItemUI> itemMap =
        new Dictionary<string, ChecklistItemUI>();

    bool initialized = false;

    void InitializeIfNeeded()
    {
        if (initialized) return;

        if (checklistItems == null || checklistItems.Length == 0)
        {
            Debug.LogWarning("Checklist items not assigned!");
            return;
        }

        itemMap.Clear();

        foreach (var item in checklistItems)
        {
            string id = item.GetItemId();

            if (!itemMap.ContainsKey(id))
            {
                itemMap.Add(id, item);
                Debug.Log("Registered checklist key: " + id);
            }
            else
            {
                Debug.LogWarning("Duplicate checklist id: " + id);
            }
        }

        initialized = true;
    }

    public void MarkCollected(string itemId)
    {
        InitializeIfNeeded();

        Debug.Log("Collected: [" + itemId + "]");

        if (itemMap.ContainsKey(itemId))
        {
            itemMap[itemId].MarkCompleted();
        }
        else
        {
            Debug.LogWarning("Checklist item not found: " + itemId);
        }
    }

    void OnEnable()
    {
        if (checklistItems == null) return;

        foreach (var item in checklistItems)
        {
            item.UpdateVisual();
        }
    }

    public bool AllCollected()
    {
        InitializeIfNeeded();

        foreach (var item in checklistItems)
        {
            if (!item.IsCompleted())
                return false;
        }

        return true;
    }

    public int TotalCollectedCount()
    {
        InitializeIfNeeded();

        int count = 0;

        foreach (var item in checklistItems)
        {
            if (item.IsCompleted())
                count++;
        }

        return count;
    }

    public void ResetAll()
    {
        foreach (var item in checklistItems)
        {
            item.ResetItem();
        }

        initialized = false;
        itemMap.Clear();
    }
}