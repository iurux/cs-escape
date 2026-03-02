using UnityEngine;
using TMPro;

public class UVInk : MonoBehaviour
{
    public float fadeSpeed = 3f;
    public SpriteRenderer targetToHide; // 여기에 bfsTree를 드래그해서 넣으세요

    private SpriteRenderer spriteRenderer;
    private TextMeshPro tmp;

    private float currentAlpha = 0f;
    private bool isBeingLit = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        tmp = GetComponent<TextMeshPro>();
        SetAlpha(0f);
    }

    void Update()
    {
        if (isBeingLit)
        {
            currentAlpha += Time.deltaTime * fadeSpeed;
        }
        else
        {
            currentAlpha -= Time.deltaTime * fadeSpeed;
        }

        currentAlpha = Mathf.Clamp01(currentAlpha);
        SetAlpha(currentAlpha);

        // [추가] 배경이 나타날 때 기존 글자는 반대로 투명해짐
        if (targetToHide != null)
        {
            Color c = targetToHide.color;
            c.a = 1f - currentAlpha; // 힌트가 1이면 기존 글자는 0
            targetToHide.color = c;
        }

        isBeingLit = false;
    }

    public void Reveal() { isBeingLit = true; }

    void SetAlpha(float alpha)
    {
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = alpha;
            spriteRenderer.color = c;
        }

        if (tmp != null)
        {
            // FaceColor의 알파를 조절하여 TMP 글자 투명도 제어
            tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, alpha);
        }
    }
}