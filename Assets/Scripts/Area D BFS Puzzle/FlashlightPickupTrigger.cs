using UnityEngine;

public class FlashlightPickupTrigger : MonoBehaviour
{
    public GameObject playerFlashlightHolder;
    public GameObject guideHUD;

    public void Interact()
    {
        if (playerFlashlightHolder != null)
        {
            playerFlashlightHolder.SetActive(true);
            
            UVFlashlight uv = playerFlashlightHolder.GetComponentInChildren<UVFlashlight>(true);
            if (uv != null)
            {
                uv.isPickedUp = true;   // 획득 상태로 변경
                uv.canUseInArea = true; // 주운 직후엔 일단 쓸 수 있게 함 (문을 통과하기 전까지)
            }
        }

        if (guideHUD != null) guideHUD.SetActive(true);
        
        // 아이템 오브젝트 삭제
        Destroy(gameObject);
    }
}