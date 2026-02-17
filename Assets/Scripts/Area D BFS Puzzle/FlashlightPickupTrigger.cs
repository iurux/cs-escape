using UnityEngine;

public class FlashlightPickupTrigger : MonoBehaviour
{
    // [Header("이 칸에 플레이어의 FlashlightHolder를 드래그해서 넣으세요")]
    public GameObject playerFlashlightHolder;

    // 이 함수는 이 물체가 파괴될 때 자동으로 실행됩니다.
    private void OnDestroy()
    {
        // 게임이 꺼지는 중이 아니고, 플레이어 전등 홀더가 설정되어 있다면
        if (playerFlashlightHolder != null && !gameObject.scene.isLoaded) return; 
        
        // 아이템을 집어서 이 오브젝트가 파괴되는 순간 플레이어 손의 전등을 켭니다.
        if (playerFlashlightHolder != null)
        {
            playerFlashlightHolder.SetActive(true);
            Debug.Log("손전등을 획득하여 플레이어 전등을 활성화했습니다.");
        }
    }
}