using UnityEngine;
using UnityEngine.InputSystem;
using Oculus.Interaction;

public class GrabbedObjectDistanceAdjuster : MonoBehaviour
{
    [Header("Input Action")]
    [Tooltip("Input action for the right thumbstick (Vector2), e.g. bound to <XRController>{RightHand}/primary2DAxis.")]
    [SerializeField]
    private InputActionProperty rightThumbstickAction;

    [Header("Distance Settings")]
    [Tooltip("Speed at which the object's local Z is adjusted.")]
    [SerializeField]
    private float distanceAdjustSpeed = 1.0f;
    [Tooltip("Minimum allowed local Z value.")]
    [SerializeField]
    private float minLocalZ = 0.3f;
    [Tooltip("Maximum allowed local Z value.")]
    [SerializeField]
    private float maxLocalZ = 3.0f;

    // Flag set when this object is grabbed.
    private bool isGrabbed = false;
    // Store the original parent so we can restore it on release.
    private Transform originalParent;

    private void OnEnable()
    {
        rightThumbstickAction.action.Enable();
    }

    private void OnDisable()
    {
        rightThumbstickAction.action.Disable();
    }

    /// <summary>
    /// Call this method (for example, from your grab event callback) when the object is grabbed.
    /// Pass in the interactor’s attach transform.
    /// </summary>
    /// <param name="interactorAttachTransform">The attach transform of the interactor (e.g. a child GameObject on your controller).</param>
    public void OnGrabbed(Transform interactorAttachTransform)
    {
        if (!isGrabbed)
        {
            isGrabbed = true;
            // Save the original parent.
            originalParent = transform.parent;
            // Parent the object to the interactor's attach transform.
            transform.SetParent(interactorAttachTransform, true);
            Debug.Log("Object grabbed. Now parented to interactor attach transform.");
        }
    }

    /// <summary>
    /// Call this method (for example, from your release event callback) when the object is released.
    /// </summary>
    public void OnReleased()
    {
        if (isGrabbed)
        {
            isGrabbed = false;
            // Restore the original parent.
            transform.SetParent(originalParent, true);
            Debug.Log("Object released. Parent restored.");
        }
    }

    private void Update()
    {
        if (isGrabbed)
        {
            // Read the right thumbstick input.
            Vector2 thumbInput = rightThumbstickAction.action.ReadValue<Vector2>();
            if (Mathf.Abs(thumbInput.y) > 0.1f)
            {
                // Adjust local position along Z.
                Vector3 localPos = transform.localPosition;
                float newZ = Mathf.Clamp(localPos.z + thumbInput.y * distanceAdjustSpeed * Time.deltaTime, minLocalZ, maxLocalZ);
                transform.localPosition = new Vector3(localPos.x, localPos.y, newZ);
                Debug.Log("Adjusted grabbed object's local Z to: " + newZ);
            }
        }
    }
}