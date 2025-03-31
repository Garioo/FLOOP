using UnityEngine;
using UnityEngine.InputSystem;
using Oculus.Interaction;

public class GrabbableWithDistance : MonoBehaviour
{
    [Header("Input Action")]
    [Tooltip("Input action for the right thumbstick (Vector2), e.g. <XRController>{RightHand}/primary2DAxis")]
    [SerializeField]
    private InputActionProperty thumbstickAction;

    [Header("Distance Settings")]
    [Tooltip("Speed at which the grabbed object moves.")]
    [SerializeField]
    private float moveSpeed = 1.0f;
    [Tooltip("Minimum allowed distance from the grab point.")]
    [SerializeField]
    private float minDistance = 0.3f;
    [Tooltip("Maximum allowed distance from the grab point.")]
    [SerializeField]
    private float maxDistance = 3.0f;

    [Header("Attach Transform")]
    [Tooltip("The transform where the object should be held when grabbed.")]
    [SerializeField]
    private Transform attachTransform;

    private bool isGrabbed = false;
    private float currentOffset = 1.0f;

    private void OnEnable()
    {
        thumbstickAction.action.Enable();
    }

    private void OnDisable()
    {
        thumbstickAction.action.Disable();
    }

    /// <summary>
    /// Called when the object is grabbed. Parent it to the attachTransform.
    /// </summary>
    public void OnGrabbed(Transform newAttachTransform)
    {
        if (!isGrabbed)
        {
            isGrabbed = true;
            attachTransform = newAttachTransform;
            transform.SetParent(attachTransform, true);
            currentOffset = Vector3.Distance(attachTransform.position, transform.position);
            Debug.Log("Object grabbed: " + gameObject.name);
        }
    }

    /// <summary>
    /// Called when the object is released. Restore the original parent.
    /// </summary>
    public void OnReleased()
    {
        if (isGrabbed)
        {
            isGrabbed = false;
            transform.SetParent(null, true);
            Debug.Log("Object released: " + gameObject.name);
        }
    }

    private void Update()
    {
        if (isGrabbed && attachTransform != null)
        {
            // Read the thumbstick input (y-axis for forward/back movement).
            Vector2 thumbInput = thumbstickAction.action.ReadValue<Vector2>();
            if (Mathf.Abs(thumbInput.y) > 0.1f)
            {
                // Adjust the distance
                currentOffset += thumbInput.y * moveSpeed * Time.deltaTime;
                currentOffset = Mathf.Clamp(currentOffset, minDistance, maxDistance);

                // Move the object along the attachTransform's forward direction
                transform.position = attachTransform.position + attachTransform.forward * currentOffset;
                
                Debug.Log("Moved " + gameObject.name + " to " + transform.position);
            }
        }
    }
}
