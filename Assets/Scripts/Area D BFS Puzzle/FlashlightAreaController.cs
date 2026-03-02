using UnityEngine;

public class FlashlightAreaController : MonoBehaviour
{
    public GameObject guideHUD;      // 화면에 표시될 가이드 UI (예: "F키를 눌러 UV등 켜기")
    public UVFlashlight playerFlashlight; // 플레이어의 UV 손전등 스크립트 참조

    // 트리거 범위 안으로 들어왔을 때 실행
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("구역 진입 (ENTER AREA)");

        // 충돌한 물체가 "Player" 태그를 가졌고, 손전등을 이미 획득한 상태라면
        if (other.CompareTag("Player") && playerFlashlight.isPickedUp)
        {
            playerFlashlight.canUseInArea = true; // 해당 구역에서 손전등 사용 가능 설정
            
            if (guideHUD != null) 
                guideHUD.SetActive(true); // 가이드 HUD 표시

            Debug.Log("특정 구역 진입: 손전등 사용 가능");
        }
    }

    // 트리거 범위 밖으로 나갔을 때 실행
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("구역 이탈 (EXIT AREA)");

        // 충돌한 물체가 "Player" 태그라면
        if (other.CompareTag("Player"))
        {
            playerFlashlight.canUseInArea = false; // 해당 구역에서 손전등 사용 불가 설정
            playerFlashlight.ForceTurnOff();       // 구역을 벗어나면 손전등 강제 종료
            
            if (guideHUD != null) 
                guideHUD.SetActive(false); // 가이드 HUD 숨기기

            Debug.Log("특정 구역 이탈: 손전등 사용 불가");
        }
    }
}