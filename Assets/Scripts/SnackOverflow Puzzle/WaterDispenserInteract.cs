using UnityEngine;
using System.Collections; // 코루틴 사용을 위해 필요합니다.

public class WaterDispenserInteract : MonoBehaviour
{
    [Header("오브젝트 연결")]
    public GameObject realMugModel;   
    public GameObject waterInsideCup;  
    public GameObject hiddenText;      

    // 정수기에서 'E'를 눌렀을 때 실행되는 함수
    public void Interact()
    {
        if (realMugModel != null && realMugModel.activeInHierarchy)
        {
            // 바로 실행하지 않고, 'FillWaterRoutine'이라는 코루틴을 시작합니다.
            StartCoroutine(FillWaterRoutine());
        }
        else
        {
            Debug.Log("컵을 들고 있지 않습니다.");
        }
    }

    // 시간을 지연시키는 로직을 담은 코루틴
    IEnumerator FillWaterRoutine()
    {
        Debug.Log("물을 따르기 시작합니다...");

        // 1. 여기서 2초 동안 기다립니다.
        yield return new WaitForSeconds(2.0f); 

        // 2. 2초가 지난 후, 물과 글자를 활성화합니다.
        if(waterInsideCup != null) waterInsideCup.SetActive(true);
        if(hiddenText != null) hiddenText.SetActive(true);

        // 3. 컵 모델의 위치, 회전, 크기를 이미지에서 설정한 수치로 변경합니다.
        realMugModel.transform.localPosition = new Vector3(-0.006f, -0.063f, 1.002f);
        realMugModel.transform.localRotation = Quaternion.Euler(70.941f, 117.214f, -61.782f);
        realMugModel.transform.localScale = new Vector3(2.705f, 2.705f, 2.705f);

        Debug.Log("2초 후: 물이 다 찼고 컵을 들어 올렸습니다!");
    }
}