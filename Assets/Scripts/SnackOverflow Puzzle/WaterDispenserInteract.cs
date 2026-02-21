using UnityEngine;

public class WaterDispenserInteract : MonoBehaviour
{
    // [Header("확인 및 켜야 할 오브젝트들")]
    public GameObject playerCupHand;   // 플레이어가 컵을 들고 있는지 확인용
    public GameObject waterInsideCup;  // 컵 안의 파란 물 (Cylinder)
    public GameObject hiddenText;      // 컵 바닥의 WAKE UP 글자 (TextMeshPro)

    // 플레이어가 정수기를 클릭했을 때 실행
    public void Interact()
    {
        // 플레이어 손에 있는 컵이 켜져 있는지(활성화 상태인지) 검사
        if (playerCupHand != null && playerCupHand.activeSelf == true)
        {
            // 1. 컵 안에 물과 글자를 켭니다.
            if(waterInsideCup != null) waterInsideCup.SetActive(true);
            if(hiddenText != null) hiddenText.SetActive(true);

            Debug.Log("물을 채웠습니다! 컵 바닥에 무언가 나타났습니다!");
        }
        else
        {
            // 컵이 없으면 거절 메시지
            Debug.Log("물을 받을 빈 컵이 필요합니다.");
        }
    }
}