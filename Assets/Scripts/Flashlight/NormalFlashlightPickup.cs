using UnityEngine;
using TMPro; // TextMeshPro를 사용한다면 필요

public class NormalFlashlightPickup : MonoBehaviour
{
    public GameObject playerFlashlightObject;

    //[Header("인벤토리 정보")]
    public string itemId = "Flashlight";
    public Sprite icon;
    public DialogueUI dialogueUI;

    public void Interact()
    {
        // ===== 1️⃣ 플레이어 손전등 활성화 =====
        if (playerFlashlightObject != null)
        {
            // 손전등의 부모 오브젝트(예: 팔 또는 소켓)를 활성화
            playerFlashlightObject.transform.parent.gameObject.SetActive(true);
            playerFlashlightObject.SetActive(true);

            NormalFlashlight nf = playerFlashlightObject.GetComponent<NormalFlashlight>();
            if (nf != null)
            {
                nf.isPickedUp = true; // 습득 상태 true
                nf.enabled = true;    // 스크립트 활성화
            }
        }

        // ===== 2️⃣ 인벤토리에 아이템 추가 =====
        InventorySimple inventory = FindObjectOfType<InventorySimple>();
        if (inventory != null)
        {
            inventory.Add(itemId, icon);
        }

        // ===== 3️⃣ 다이얼로그(대사) 출력 =====
        if (itemId == "Flashlight" && dialogueUI != null)
        {
            string[] pickupDialogue = new string[] 
            { 
                "Here is a flashlight...",
                "I think I could use this in a dark restroom."
            };

            dialogueUI.StartDialogue(pickupDialogue);
        }

        // ===== 4️⃣ 필드에 있는 아이템 오브젝트 제거 =====
        // 대사가 시작되더라도 물체는 사라지게 설정
        Destroy(gameObject);
    }
}