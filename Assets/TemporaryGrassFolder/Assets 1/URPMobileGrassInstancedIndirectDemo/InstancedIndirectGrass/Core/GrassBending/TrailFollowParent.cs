using UnityEngine;

public class TrailFollowParent : MonoBehaviour
{
    private Transform parentTransform;
    private Quaternion initialRotation;

    void Start()
    {
        // Get parent transform
        parentTransform = transform.parent;
        if (parentTransform == null)
        {
            Debug.LogError("TrailFollowParent script requires a parent object!");
            return;
        }

        // Lock initial rotation
        initialRotation = transform.rotation;
    }

    void LateUpdate()
    {
        if (parentTransform == null) return;

        // Set trail position to follow parent's X and Z, but Y is always parent Y - 1
        transform.position = new Vector3(parentTransform.position.x, parentTransform.position.y - 1f, parentTransform.position.z);

        // Lock rotation to initial state
        transform.rotation = initialRotation;
    }
}
