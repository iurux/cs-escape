using UnityEngine;

public class EmptyCupPickup : MonoBehaviour
{
    // [Header("플레이어 손에 있는 컵 오브젝트 연결")]
    public GameObject playerCupHand; 

    // 플레이어가 크로스헤어로 컵을 보고 클릭(F키 등)했을 때 실행
    public void Interact() 
    {
        if (playerCupHand != null)
        {
            // 1. 플레이어 화면에 컵이 나타나게 함
            playerCupHand.SetActive(true);
            Debug.Log("빈 컵을 획득했습니다!");
        }

        // 2. 책상 위에 있던 컵 오브젝트는 삭제
        Destroy(gameObject); 
    }
}