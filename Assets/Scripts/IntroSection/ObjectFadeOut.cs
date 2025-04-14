using UnityEngine;

public class ObjectFadeOut : MonoBehaviour
{
    public float fadeDuration = 2f;           // How long to fade
    public Renderer targetRenderer;           // Optional: Specify a renderer, otherwise use attached
    private Material targetMaterial;          // The material we will fade
    private Color originalColor;
    private bool isFading = false;

    void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }

        if (targetRenderer != null)
        {
            targetMaterial = targetRenderer.material;
            originalColor = targetMaterial.color;

            // Make sure the shader supports transparency
            SetMaterialToTransparent(targetMaterial);
        }
        else
        {
            Debug.LogWarning("No Renderer found on " + gameObject.name);
        }
    }

    public void StartFade()
    {
        if (!isFading && targetMaterial != null)
        {
            StartCoroutine(FadeOut());
        }
    }

    private System.Collections.IEnumerator FadeOut()
    {
        isFading = true;
        float elapsed = 0f;
        Color fadeColor = originalColor;

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(originalColor.a, 0f, elapsed / fadeDuration);
            fadeColor.a = alpha;
            targetMaterial.color = fadeColor;

            elapsed += Time.deltaTime;
            yield return null;
        }

        fadeColor.a = 0f;
        targetMaterial.color = fadeColor;

        gameObject.SetActive(false);
    }

    // Ensure the material uses a transparent rendering mode
    private void SetMaterialToTransparent(Material mat)
    {
        if (mat.HasProperty("_Mode"))
        {
            mat.SetFloat("_Mode", 2f); // Fade mode
        }

        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }
}
