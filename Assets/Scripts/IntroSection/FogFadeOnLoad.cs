using UnityEngine;

public class FogFadeOnLoad : MonoBehaviour
{
  public float startDensity = 1.0f;
    public float endDensity = 0.0f;
    public float duration = 6f;

    private float timer = 0f;
    private bool isFading = false;

    void Start()
    {
        RenderSettings.fog = true;
        RenderSettings.fogDensity = startDensity;
        Debug.Log("Fog initialized to density: " + startDensity);
    }

    public void StartFogFade()
    {
        Debug.Log("StartFogFade triggered!");
        isFading = true;
        timer = 0f;
    }

    void Update()
    {
        if (isFading)
        {
            FogFade();
        }
    }

    private void FogFade()
    {
        if (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            RenderSettings.fogDensity = Mathf.Lerp(startDensity, endDensity, t);
            Debug.Log("Fading... density: " + RenderSettings.fogDensity);
        }
        else
        {
            isFading = false;
            RenderSettings.fogDensity = endDensity;
            Debug.Log("Fog fade complete.");
        }
    }
}






