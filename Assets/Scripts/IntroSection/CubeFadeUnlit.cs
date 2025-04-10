using UnityEngine;

public class CubeFadeUnlit : MonoBehaviour
{
    public float fadeDelay = 2f; // Time before starting the fade
    public static float fadeDuration = 3f; // Duration of the fade
    private Material cubeMaterial;
    private Color targetColor;
    private Color startColor;

    private bool isFading = false;

    public void StartCubeFade()
    {
        if (isFading) return;  // Prevent multiple fades at the same time

        // Get the material of the cube (assuming the object has a Renderer with a material)
        cubeMaterial = GetComponent<Renderer>().material;

        // Initial color setup
        startColor = cubeMaterial.color;
        targetColor = startColor;
        targetColor.a = 0f; // Set the target alpha to 0 (fully transparent)

        // Start fading after a delay
        Invoke("StartFade", fadeDelay);
    }

    void StartFade()
    {
        if (!isFading)
        {
            isFading = true;
            StartCoroutine(FadeAlphaCoroutine());
        }
    }

    System.Collections.IEnumerator FadeAlphaCoroutine()
    {
        float timeElapsed = 0f;
        while (timeElapsed < fadeDuration)
        {
            // Lerp between the start and target color based on time
            cubeMaterial.color = Color.Lerp(startColor, targetColor, timeElapsed / fadeDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the final color is set after fading is done
        cubeMaterial.color = targetColor;
        isFading = false; // Reset the fading state
    }
}

