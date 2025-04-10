using UnityEngine;

public class FogFadeOnLoad : MonoBehaviour
{

    [Header("Fade Settings")]
    public float startDensity = 1.0f;
    public float endDensity = 0.0f;
   
    [Tooltip("Time before starting the fade (in seconds)")]
     public static float fogduration = 6f;



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
        if (timer < fogduration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fogduration);
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






