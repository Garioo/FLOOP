using UnityEngine;

public class IntroManager : MonoBehaviour
{
    public FogFadeOnLoad fogFade;  // Reference to the FogFade script
    public CubeFadeUnlit cubeFade;  // Reference to the CubeFade script

    public ObjectFadeOut objectFade; // Reference to the ObjectFadeOut script



    public float fadeDelay = 3f; // Time before starting the fade


    void Start()
    {
        // Start the fade actions 10 seconds after the scene load
        Invoke("StartFades", fadeDelay);
        
    }

    void StartFades()
    {
        // Trigger the fog fade and cube fade after 10 seconds
        fogFade.StartFogFade();
        cubeFade.StartCubeFade();
        objectFade.StartFade();
    }
}