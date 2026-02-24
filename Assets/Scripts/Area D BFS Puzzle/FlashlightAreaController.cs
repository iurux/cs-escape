using UnityEngine;

public class FlashlightAreaController : MonoBehaviour
{
    public GameObject guideHUD;
    public UVFlashlight playerFlashlight;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ENTER AREA");
        // Lab2 ����(ū �ڽ�) ������ ������ ��
        if (other.CompareTag("Player") && playerFlashlight.isPickedUp)
        {
            playerFlashlight.canUseInArea = true;
            if (guideHUD != null) guideHUD.SetActive(true);
            Debug.Log("Lab2 ����: ������ ��� ����");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("EXIT AREA");
        // Lab2 ����(ū �ڽ�) ������ ������ ��
        if (other.CompareTag("Player"))
        {
            playerFlashlight.canUseInArea = false;
            playerFlashlight.ForceTurnOff(); // �� ����
            if (guideHUD != null) guideHUD.SetActive(false); // ���̵� �����
            Debug.Log("Lab2 ����: ������ ��� �Ұ�");
        }
    }
}