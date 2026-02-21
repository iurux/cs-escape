using UnityEngine;
using UnityEngine.InputSystem;

public class NormalFlashlight : MonoBehaviour
{
    [Header("Settings")]
    public Light lightSource;       
    public bool isPickedUp = false; 

    private bool isOn = false;

    // 오브젝트가 활성화될 때(아이템을 주웠을 때) 실행
    void OnEnable()
    {
        // 줍는 순간에는 무조건 꺼진 상태로 초기화
        isOn = false; 
        if (lightSource != null) 
        {
            lightSource.enabled = false;
        }
    }

    void Update()
    {
        // 주운 상태가 아니면 작동 안 함
        if (!isPickedUp) return;

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            ToggleFlashlight();
        }
    }

    void ToggleFlashlight()
    {
        if (lightSource == null) return;

        isOn = !isOn;
        lightSource.enabled = isOn;
        Debug.Log("일반 손전등 상태: " + (isOn ? "ON" : "OFF"));
    }
}