using UnityEngine;
using System.Collections.Generic;

public class ItemAnalytics : MonoBehaviour
{
    public string itemID;

    public void OnItemCollected()
    {
        if (string.IsNullOrEmpty(itemID))
        {
            Debug.LogWarning("ItemAnalytics: itemID is empty.");
            return;
        }
    }
}