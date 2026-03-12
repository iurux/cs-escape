using UnityEngine;

public class ABTestManager : MonoBehaviour
{
    // public static string Variant { get; private set; }
    public static string Variant { get; private set; } = "A";

    void Awake()
    
    {
        DontDestroyOnLoad(gameObject);
    #if UNITY_EDITOR
        if (overrideVariant)
        {
            Variant = forcedVariant;
            Debug.Log("Editor Forced Variant: " + Variant);
            return;
        }
    #endif

        if (PlayerPrefs.HasKey("ab_variant"))
        {
            Variant = PlayerPrefs.GetString("ab_variant");
        }
        else
        {
            Variant = Random.value < 0.5f ? "A" : "B";
            PlayerPrefs.SetString("ab_variant", Variant);
            PlayerPrefs.Save();
        }

        Debug.Log("A/B Variant: " + Variant);
    }
    #if UNITY_EDITOR
    [Header("Editor Override")]
    public bool overrideVariant = false;
    public string forcedVariant = "A";
    #endif
}