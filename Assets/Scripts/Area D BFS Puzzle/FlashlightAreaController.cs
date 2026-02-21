using UnityEngine;

public class FlashlightAreaController : MonoBehaviour
{
    public GameObject guideHUD;
    public UVFlashlight playerFlashlight;

    private void OnTriggerEnter(Collider other)
    {
        // Lab2 구역(큰 박스) 안으로 들어왔을 때
        if (other.CompareTag("Player") && playerFlashlight.isPickedUp)
        {
            playerFlashlight.canUseInArea = true;
            if (guideHUD != null) guideHUD.SetActive(true);
            Debug.Log("Lab2 진입: 손전등 사용 가능");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Lab2 구역(큰 박스) 밖으로 나갔을 때
        if (other.CompareTag("Player"))
        {
            playerFlashlight.canUseInArea = false;
            playerFlashlight.ForceTurnOff(); // 불 끄기
            if (guideHUD != null) guideHUD.SetActive(false); // 가이드 숨기기
            Debug.Log("Lab2 퇴장: 손전등 사용 불가");
        }
    }
}