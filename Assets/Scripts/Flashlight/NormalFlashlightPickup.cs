using UnityEngine;

public class NormalFlashlightPickup : MonoBehaviour
{
    public GameObject playerFlashlightObject;

    public void Interact()
    {
        if (playerFlashlightObject != null)
        {
            // 부모(Holder)가 꺼져있을 수 있으므로 부모부터 활성화
            playerFlashlightObject.transform.parent.gameObject.SetActive(true);
            playerFlashlightObject.SetActive(true);

            NormalFlashlight nf = playerFlashlightObject.GetComponent<NormalFlashlight>();
            if (nf != null)
            {
                nf.isPickedUp = true;
                // 줍는 순간 불이 꺼져있도록 명시적으로 호출
                nf.enabled = true; 
            }
        }
        Destroy(gameObject);
    }
}