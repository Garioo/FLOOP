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
        if (!rayInteractor.hasSelection)
            return;

        attachTransform = rayInteractor.attachTransform;
        if (attachTransform == null) return;

        Vector2 input = translateInput.action.ReadValue<Vector2>();
        float forwardInput = input.y;

        Vector3 localPos = attachTransform.localPosition;
        float newZ = localPos.z + forwardInput * Time.deltaTime;

        // Clamp the new Z position before applying
        if ((forwardInput > 0 && newZ > maxDistance) ||
            (forwardInput < 0 && newZ < minDistance))
        {
            // Do not apply movement if it goes out of bounds
            return;
        }

        localPos.z = newZ;
        attachTransform.localPosition = localPos;
    }

    void BlockInputThisFrame()
    {
        // Disable the input action just for this frame
        translateInput.action.Disable();
        translateInput.action.Enable(); // Re-enable to keep Unity happy
    }
}