using UnityEngine;
using UnityEngine.InputSystem; // 팀원분이 이걸 쓰고 계시네요!

public class UVFlashlight : MonoBehaviour
{
    [Header("UV Light Settings")]
    public Light uvLightSource;      // 실제 보라색 조명
    public LayerMask targetLayer;    // 무엇을 비출지 (HiddenHint)
    public float detectRange = 5.0f; // 빛이 닿는 거리
    public float lightAngle = 45.0f; // 빛의 각도

    private bool isLightOn = false;
    private Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main; // 메인 카메라 찾기
        if (uvLightSource != null) uvLightSource.enabled = false; // 시작할 땐 꺼둠
    }

    void Update()
    {
        // T키를 누르면 켜지고 꺼짐
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            ToggleLight();
        }

        // 빛이 켜져 있을 때만 검사
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

    void CheckForHints()
    {
        // 카메라 위치에서 정면으로 레이저 발사
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
    
        // 0.5f 반경의 구체를 발사하여 HiddenHint 레이어를 가진 물체들 감지
        // 힌트가 작을 수 있으므로 반경을 1.0f 정도로 키워보는 것도 방법입니다.
        RaycastHit[] hits = Physics.SphereCastAll(ray, 1.0f, detectRange, targetLayer);

        foreach (RaycastHit hit in hits)
        {
            // 1. 맞은 물체의 "부모"나 "본인"에게 UVInk 스크립트가 있는지 확인
            // GetComponentInParent를 쓰면 콜라이더가 자식에 있어도 찾을 수 있어 더 안전합니다.
            UVInk inkScript = hit.collider.GetComponent<UVInk>();
        
            if (inkScript == null) 
                inkScript = hit.collider.GetComponentInParent<UVInk>();

            if (inkScript != null)
            {
                // 2. 시야각 체크 (손전등의 빛 범위 안에 있는지)
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