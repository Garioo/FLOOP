using UnityEngine;

public class LockTrailRotation : MonoBehaviour
{
    private Quaternion initialRotation;
    private float lockedYPosition;

    public bool lockYPosition = true;  // Toggle for locking Y position
    public bool lockRotation = true;   // Toggle for locking rotation

    void Start()
    {
        // Store the initial rotation and Y position
        initialRotation = transform.rotation;
        lockedYPosition = transform.position.y;
    }

    void LateUpdate()
    {
        Vector3 newPosition = transform.position;

        // Lock Y Position
        if (lockYPosition)
        {
            newPosition.y = lockedYPosition;
        }

        // Lock Rotation
        if (lockRotation)
        {
            transform.rotation = initialRotation;
        }

        // Apply position update
        transform.position = newPosition;
    }
}

