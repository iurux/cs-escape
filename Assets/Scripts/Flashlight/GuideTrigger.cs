using UnityEngine;

public class GuideTrigger : MonoBehaviour
{
    public GameObject guideHUD;        // 보여줄 일반 손전등 가이드 UI
    public string requiredItemId = "NormalFlashlight"; // 인벤토리에서 확인할 아이템 ID

    // 1. 구역에 들어왔을 때
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InventorySimple inv = other.GetComponent<InventorySimple>();

            // 손전등을 가지고 있는지 체크해서 있으면 켭니다.
            if (inv != null && inv.Has(requiredItemId))
            {
                guideHUD.SetActive(true); 
                Debug.Log("화장실 진입: 가이드 표시");
            }
        }
    }

    // 2. 구역에서 나갔을 때 
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어가 구역을 벗어나면 무조건 가이드를 끕니다.
            if (guideHUD != null)
            {
                guideHUD.SetActive(false);
                Debug.Log("화장실 이탈: 가이드 숨김");
            }
        }
    }
}