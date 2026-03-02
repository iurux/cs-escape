using UnityEngine;
using UnityEngine.InputSystem;

public class NormalFlashlight : MonoBehaviour
{
    [Header("Settings")]
    public Light lightSource;       
    public bool isPickedUp = false; 

    private bool isOn = false;

    // 오브젝트가 활성화될 때(플레이어의 손에 들어왔을 때) 실행
    void OnEnable()
    {
        // 새로 활성화될 때마다 기본적으로 꺼진 상태로 초기화
        isOn = false; 
        if (lightSource != null) 
        {
            lightSource.enabled = false;
        }
    }

    void Update()
    {
        // 손전등을 획득한 상태가 아니라면 작동하지 않음
        if (!isPickedUp) return;

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Debug.Log("R detected");
            ToggleFlashlight();
        }
    }

    void ToggleFlashlight()
    {
        Debug.Log("TOGGLE BY INPUT");
        if (lightSource == null) return;

        isOn = !isOn;
        lightSource.enabled = isOn;
        Debug.Log("일반 손전등 상태: " + (isOn ? "ON" : "OFF"));
    }
}