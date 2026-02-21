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
    public bool isPickedUp = false;      // 아이템을 획득했는가?
    public bool canUseInArea = false;    // 현재 사용 가능한 구역(Lab2)인가?
    
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
        // 획득 전이거나, 허용된 구역이 아니면 작동 안 함
        if (!isPickedUp || !canUseInArea) return;

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
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

    // 에러 해결: 레이캐스트로 힌트를 찾는 로직
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