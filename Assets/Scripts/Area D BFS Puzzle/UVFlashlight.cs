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
    public bool isPickedUp = false;      // 손전등을 획득했는가?
    public bool canUseInArea = false;    // 현재 사용 가능한 영역(예: Lab2)인가?
    
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
        // 획득 전이거나, 사용 가능 영역이 아니면 작동하지 않음
        Debug.Log("PickedUp: " + isPickedUp + " Area: " + canUseInArea);
        if (!isPickedUp || !canUseInArea) return;

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (!canUseInArea)
            {
                Debug.Log("사용 가능한 구역이 아닙니다.");
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

    // 힌트 감지: 레이캐스트를 통해 힌트를 탐색함
    void CheckForHints()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        // SphereCast를 사용하여 조명 범위 내의 콜라이더들을 감지
        RaycastHit[] hits = Physics.SphereCastAll(ray, 1.0f, detectRange, targetLayer);

        foreach (RaycastHit hit in hits)
        {
            UVInk inkScript = hit.collider.GetComponent<UVInk>();
            // 자식이나 부모 오브젝트에서 UVInk 스크립트를 탐색
            if (inkScript == null) inkScript = hit.collider.GetComponentInParent<UVInk>();

            if (inkScript != null)
            {
                Vector3 dirToTarget = (hit.transform.position - playerCamera.transform.position).normalized;
                float angle = Vector3.Angle(playerCamera.transform.forward, dirToTarget);

                // 카메라 정면과 오브젝트 사이의 각도가 조명 각도 안에 있다면 힌트 표시
                if (angle < lightAngle)
                {
                    inkScript.Reveal();
                }
            }
        }
    }
}