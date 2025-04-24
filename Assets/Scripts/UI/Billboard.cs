using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main != null)
        {
            Vector3 cameraPosition = Camera.main.transform.position;

            // Calculate the direction from this object to the camera
            Vector3 lookDirection = transform.position - cameraPosition;
            lookDirection.y = 0f; // optional: keep it upright if needed

            // Face the camera while keeping the text readable
            transform.forward = lookDirection.normalized;
        }
    }
}