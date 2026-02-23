using UnityEngine;

public class FlashlightPickupTrigger : MonoBehaviour
{
    public GameObject playerFlashlightHolder;
    public GameObject guideHUD;

    [Header("Inventory Settings")]
    public string itemId = "UV Light";
    public Sprite itemIcon;

    [Header("Dialogue")]
    public DialogueUI dialogueUI;   // 拖进 Inspector

    public void Interact()
    {
        // ===== 1️⃣ 加入 Inventory =====
        InventorySimple inv = FindObjectOfType<InventorySimple>();
        if (inv != null)
        {
            inv.Add(itemId, itemIcon);
        }

        // ===== 2️⃣ 激活玩家 UV 手电 =====
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

        // ===== 3️⃣ 显示 Guide HUD =====
        if (guideHUD != null)
            guideHUD.SetActive(true);

        // ===== 4️⃣ 触发 Dialogue =====
        if (dialogueUI != null)
        {
            string[] lines = new string[]
            {
                "Is this... a UV light?",
                "Maybe I can use it to reveal something hidden..."
            };

            dialogueUI.StartDialogue(lines);
        }

        // ===== 5️⃣ 删除场景物体 =====
        Destroy(gameObject);
    }
}