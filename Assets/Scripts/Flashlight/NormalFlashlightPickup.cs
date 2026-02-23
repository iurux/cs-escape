using UnityEngine;

public class NormalFlashlightPickup : MonoBehaviour
{
    public GameObject playerFlashlightObject;

    [Header("Inventory Info")]
    public string itemId = "Flashlight";
    public Sprite icon;

    public void Interact()
    {
        // ===== 1️⃣ 激活玩家手电筒 =====
        if (playerFlashlightObject != null)
        {
            playerFlashlightObject.transform.parent.gameObject.SetActive(true);
            playerFlashlightObject.SetActive(true);

            NormalFlashlight nf = playerFlashlightObject.GetComponent<NormalFlashlight>();
            if (nf != null)
            {
                nf.isPickedUp = true;
                nf.enabled = true;
            }
        }

        // ===== 2️⃣ 加入 Inventory =====
        InventorySimple inventory = FindObjectOfType<InventorySimple>();
        if (inventory != null)
        {
            inventory.Add(itemId, icon);
        }

        // ===== 3️⃣ 销毁场景物体 =====
        Destroy(gameObject);
    }
}
