using UnityEngine;

public class FlashlightPickupTrigger : MonoBehaviour
{
    public GameObject playerFlashlightHolder;
    public GameObject guideHUD;

    [Header("Inventory Settings")]
    public string itemId = "UV Light";
    public Sprite itemIcon;

    [Header("Dialogue")]
    public DialogueUI dialogueUI;   // 인스펙터(Inspector)에서 드래그하여 할당

    public void Interact()
    {
        // ===== 1️⃣ 인벤토리에 아이템 추가 =====
        InventorySimple inv = FindObjectOfType<InventorySimple>();
        if (inv != null)
        {
            inv.Add(itemId, itemIcon);
        }

        // ===== 2️⃣ 플레이어의 UV 손전등 활성화 =====
        if (playerFlashlightHolder != null)
        {
            playerFlashlightHolder.SetActive(true);
            
            UVFlashlight uv = playerFlashlightHolder.GetComponentInChildren<UVFlashlight>(true);
            if (uv != null)
            {
                uv.isPickedUp = true;
                uv.canUseInArea = false;
            }
        }

       // ===== 3️⃣ 가이드 HUD 표시 =====
        if (guideHUD != null)
            guideHUD.SetActive(true);

        // ===== 4️⃣ 다이얼로그(대사) 트리거 =====
        if (dialogueUI != null)
        {
            string[] lines = new string[]
            {
                "Is this... a UV light?",
                "Maybe I can use it to reveal something hidden..."
            };

            dialogueUI.StartDialogue(lines);
        }

        // ===== 5️⃣ 필드에 배치된 아이템 오브젝트 제거 =====
        Destroy(gameObject);
    }
}