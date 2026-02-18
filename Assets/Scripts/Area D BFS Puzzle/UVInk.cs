using UnityEngine;
using TMPro;

public class UVInk : MonoBehaviour
{
    public float fadeSpeed = 3f;

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

        isBeingLit = false;
    }

    public void Reveal()
    {
        isBeingLit = true;
        Debug.Log("Reveal called");
    }

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
        // 控制材质 Face Color
        tmp.fontMaterial.SetColor("_FaceColor", 
            new Color(1f, 1f, 1f, alpha));
    }
}
}
