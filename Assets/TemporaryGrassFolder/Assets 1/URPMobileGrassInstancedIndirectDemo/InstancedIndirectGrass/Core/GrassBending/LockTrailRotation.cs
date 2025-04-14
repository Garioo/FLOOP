using UnityEngine;

public class LockTrailRotation : MonoBehaviour
{
    [Tooltip("The GameObject to follow")]
    public Transform target;

    private Quaternion initialRotation;

    void Start()
    {
        // Store the initial rotation of this object
        initialRotation = transform.rotation;

        if (target == null)
        {
            Debug.LogWarning("No target assigned for FollowTargetWithLockedRotation.");
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Follow the target's position
        transform.position = target.position;

        // Keep original rotation
        transform.rotation = initialRotation;
    }
}

