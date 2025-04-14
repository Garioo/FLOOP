using UnityEngine;
using UnityEngine.InputSystem;


public class GrabDistanceInputBlocker : MonoBehaviour
{
    public InputActionReference translateInput;
    public float minDistance = 0.2f;
    public float maxDistance = 2f;

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor rayInteractor;
    private Transform attachTransform;

    private Vector2 cachedInput;

    void Awake()
    {
        rayInteractor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>();
        translateInput.action.Enable();
    }

    void Update()
    {
        // Only check while an object is selected
        if (rayInteractor.hasSelection)
        {
            attachTransform = rayInteractor.attachTransform;
            if (attachTransform == null) return;

            Vector2 input = translateInput.action.ReadValue<Vector2>();
            float forwardInput = input.y;
            float currentZ = attachTransform.localPosition.z;
            float nextZ = currentZ + forwardInput * Time.deltaTime;

            // Intercept the input value if it would move beyond limits
            if ((forwardInput > 0 && currentZ >= maxDistance) ||
                (forwardInput < 0 && currentZ <= minDistance))
            {
                // CANCEL the input: block Unity’s translator from using it
                BlockInputThisFrame();
            }
        }
    }

    void BlockInputThisFrame()
    {
        // Disable the input action just for this frame
        translateInput.action.Disable();
        translateInput.action.Enable(); // Re-enable to keep Unity happy
    }
}
