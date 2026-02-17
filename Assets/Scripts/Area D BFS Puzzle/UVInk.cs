using UnityEngine;

public class UVInk : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float currentAlpha = 0f;
    public float fadeSpeed = 2.0f; // 빛이 없으면 사라지는 속도

    void Start()
    {
        // 텍스트가 아닌 SpriteRenderer를 가져옵니다.
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 시작할 때 완전히 투명하게 만듭니다.
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 0;
            spriteRenderer.color = c;
        }
    }

    void Update()
    {
        // 빛을 받지 않으면 서서히 투명해집니다.
        if (currentAlpha > 0)
        {
            currentAlpha -= Time.deltaTime * fadeSpeed;
            SetAlpha(currentAlpha);
        }
    }

    // ★ 중요: 이 부분이 있어야 UVFlashlight에서 에러가 나지 않습니다.
    public void Reveal()
    {
        currentAlpha = 1.0f; // 즉시 선명하게
        SetAlpha(1.0f);
    }

    void SetAlpha(float alpha)
    {
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = Mathf.Clamp01(alpha);
            spriteRenderer.color = c;
        }
    }
}