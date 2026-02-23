using UnityEngine;

public class FlashlightPickupTrigger : MonoBehaviour
{
    public GameObject playerFlashlightHolder;
    public GameObject guideHUD;

    [Header("Inventory Settings")]
    public string itemId = "UV Light"; // 인벤토리용 ID
    public Sprite itemIcon;          // 인벤토리에 표시될 이미지

    public void Interact()
    {
        // 1. 인벤토리에 추가 (추가된 부분)
        InventorySimple inv = FindObjectOfType<InventorySimple>();
        if (inv != null)
        {
            inv.Add(itemId, itemIcon);
        }

        // 2. 플레이어 손의 손전등 활성화 (기존 로직)
        if (playerFlashlightHolder != null)
        {
            playerFlashlightHolder.SetActive(true);
            
            UVFlashlight uv = playerFlashlightHolder.GetComponentInChildren<UVFlashlight>(true);
            if (uv != null)
            {
                uv.isPickedUp = true;
                uv.canUseInArea = true; 
            }
        }

        if (guideHUD != null) guideHUD.SetActive(true);
        
        Destroy(gameObject);
    }
}