using UnityEngine;

public class FlashlightGuideController : MonoBehaviour
{
    public GameObject guideHUD;
    public UVFlashlight playerFlashlight; // 인스펙터에서 플레이어의 손전등 연결

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 Lab2 구역에 들어왔고, 손전등을 이미 주운 상태라면
        if (other.CompareTag("Player") && playerFlashlight != null && playerFlashlight.isPickedUp)
        {
            playerFlashlight.canUseInArea = true;
            if (guideHUD != null) guideHUD.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 구역을 벗어나면 사용 불가 처리
        if (other.CompareTag("Player") && playerFlashlight != null)
        {
            playerFlashlight.canUseInArea = false;
            playerFlashlight.ForceTurnOff();
            if (guideHUD != null) guideHUD.SetActive(false);
        }
    }
}