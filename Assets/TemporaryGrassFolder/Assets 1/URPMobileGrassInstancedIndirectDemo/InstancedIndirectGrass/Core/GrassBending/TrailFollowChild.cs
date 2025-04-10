using UnityEngine;

public class TrailFollowChild : MonoBehaviour
{
    private Transform childTransform;
    private Quaternion initialRotation;

    void Start()
    {
        // Get the child transform (this object itself)
        childTransform = transform;

        // Lock initial rotation
        initialRotation = transform.rotation;
    }

    void LateUpdate()
    {
        if (childTransform == null) return;

        // Set trail position to follow child's X, Y, and Z coordinates.
        // You can modify this if you want to apply specific offsets, but for now, it's simply following the child.
        transform.position = childTransform.position;

        // Lock rotation to initial state
        transform.rotation = initialRotation;
    }
}

