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

        itemMap.Clear();

        foreach (var item in checklistItems)
        {
            itemMap[item.GetItemId()] = item;
            Debug.Log("Registered checklist key: " + item.GetItemId());
        }

        initialized = true;
    }

    public void MarkCollected(string itemId)
    {
        InitializeIfNeeded();

        Debug.Log("Collected: [" + itemId + "]");

        if (itemMap.ContainsKey(itemId))
        {
            Debug.Log("MATCH FOUND");
            itemMap[itemId].MarkCompleted();
        }
        else
        {
            Debug.Log("NO MATCH");
        }
    }

    void OnEnable()
    {
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
            if (!item.IsCompleted())   // 需要下面补这个函数
                return false;
        }

        return true;
    }

    public void ResetAll()
    {
        foreach (var item in checklistItems)
        {
            item.ResetItem();
        }
    }


    // int completedCount = 0;

    // public void MarkCollected(string itemId)
    // {
    //     if (itemMap.ContainsKey(itemId))
    //     {
    //         itemMap[itemId].MarkCompleted();
    //         completedCount++;

    //         if (completedCount >= itemMap.Count)
    //         {
    //             Debug.Log("All items collected!");
    //             // Trigger new dialogue here
    //         }
    //     }
    // }
}
