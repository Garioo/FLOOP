// Attach this to an empty GameObject in your scene
using UnityEngine;

public class VRBoundaryManager : MonoBehaviour
{
    [Header("References")]
    public Transform xrOrigin;            // XR Origin
    public Transform vrCamera;            // Main Camera (head)
    public Collider boundaryCollider;     // The in-game boundary
    public Canvas fadeCanvas;             // World-space or screen-space UI
    public float fadeSpeed = 3f;
    public float clampBuffer = 0.05f;     // How far outside before clamping

    private CanvasGroup fadeGroup;
    private bool isFading = false;

    void Start()
    {
        if (fadeCanvas != null)
        {
            fadeGroup = fadeCanvas.GetComponent<CanvasGroup>();
            if (fadeGroup == null)
                fadeGroup = fadeCanvas.gameObject.AddComponent<CanvasGroup>();

            fadeGroup.alpha = 0f;
            fadeGroup.interactable = false;
            fadeGroup.blocksRaycasts = false;
            fadeCanvas.enabled = true;
        }
    }

    void LateUpdate()
    {
        if (boundaryCollider == null || xrOrigin == null || vrCamera == null) return;

        Vector3 camPos = vrCamera.position;

        Vector3 closest = boundaryCollider.ClosestPoint(camPos);
        float distance = Vector3.Distance(closest, camPos);

        if (distance > clampBuffer)
        {
            Vector3 offset = camPos - closest;
            xrOrigin.position -= new Vector3(offset.x, 0f, offset.z);
            StartFade(true);
        }
        else
        {
            StartFade(false);
        }
    }

    void StartFade(bool fadeIn)
    {
        if (fadeGroup == null) return;
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(fadeIn));
    }

    System.Collections.IEnumerator FadeRoutine(bool fadeIn)
    {
        float targetAlpha = fadeIn ? 1f : 0f;
        fadeCanvas.enabled = true;

        while (!Mathf.Approximately(fadeGroup.alpha, targetAlpha))
        {
            fadeGroup.alpha = Mathf.MoveTowards(fadeGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
            yield return null;
        }

        if (!fadeIn)
        {
            fadeCanvas.enabled = false;
        }
    }
}
