using UnityEngine;
using UnityEngine.InputSystem;

public class UVFlashlight : MonoBehaviour
{
    [Header("Settings")]
    public Light uvLightSource;
    public LayerMask targetLayer;
    public float detectRange = 5.0f;
    public float lightAngle = 45.0f;
    public GameObject guideHUD;

    [Header("Status")]
    public bool isPickedUp = false;      // �������� ȹ���ߴ°�?
    public bool canUseInArea = false;    // ���� ��� ������ ����(Lab2)�ΰ�?
    
    private bool isLightOn = false;
    private Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main;
        if (uvLightSource != null) uvLightSource.enabled = false;
        if (guideHUD != null) guideHUD.SetActive(false);
    }

    void Update()
    {
        // ȹ�� ���̰ų�, ���� ������ �ƴϸ� �۵� �� ��
        Debug.Log("PickedUp: " + isPickedUp + " Area: " + canUseInArea);
        if (!isPickedUp || !canUseInArea) return;

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (!canUseInArea)
            {
                Debug.Log("Not in usable area");
                return;
            }
            ToggleLight();
        }

        if (isLightOn)
        {
            CheckForHints();
        }
    }

    void ToggleLight()
    {
        isLightOn = !isLightOn;
        if (uvLightSource != null) uvLightSource.enabled = isLightOn;
    }

    public void ForceTurnOff()
    {
        isLightOn = false;
        if (uvLightSource != null) uvLightSource.enabled = false;
    }

    // ���� �ذ�: ����ĳ��Ʈ�� ��Ʈ�� ã�� ����
    void CheckForHints()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit[] hits = Physics.SphereCastAll(ray, 1.0f, detectRange, targetLayer);

        foreach (RaycastHit hit in hits)
        {
            UVInk inkScript = hit.collider.GetComponent<UVInk>();
            if (inkScript == null) inkScript = hit.collider.GetComponentInParent<UVInk>();

            if (inkScript != null)
            {
                Vector3 dirToTarget = (hit.transform.position - playerCamera.transform.position).normalized;
                float angle = Vector3.Angle(playerCamera.transform.forward, dirToTarget);

                if (angle < lightAngle)
                {
                    inkScript.Reveal();
                }
            }
        }
    }
}