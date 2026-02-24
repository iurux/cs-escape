using UnityEngine;
using UnityEngine.InputSystem;

public class NormalFlashlight : MonoBehaviour
{
    [Header("Settings")]
    public Light lightSource;       
    public bool isPickedUp = false; 

    private bool isOn = false;

    // ������Ʈ�� Ȱ��ȭ�� ��(�������� �ֿ��� ��) ����
    void OnEnable()
    {
        // �ݴ� �������� ������ ���� ���·� �ʱ�ȭ
        isOn = false; 
        if (lightSource != null) 
        {
            lightSource.enabled = false;
        }
    }

    void Update()
    {
        // �ֿ� ���°� �ƴϸ� �۵� �� ��
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
        Debug.Log("�Ϲ� ������ ����: " + (isOn ? "ON" : "OFF"));
    }
}