using UnityEngine;

public class FloatingPhysics : MonoBehaviour
{
    public float floatForce = 9.81f; // Adjust to match gravity
    public float waterHeight = 5f;
    [SerializeField] private Rigidbody rb;

    private void FixedUpdate()
    {
        // Raycast upwards
        RaycastHit hitUp;
        if (Physics.Raycast(transform.position, Vector3.up, out hitUp, Mathf.Infinity))
        {
            // Visualize the upward raycast
            Debug.DrawRay(transform.position, Vector3.up * hitUp.distance, Color.blue);

            if (hitUp.collider.CompareTag("WaterSurface"))
            {
                waterHeight = hitUp.point.y;
            }
            else
            {
          
                waterHeight = -500f;
            }
        }

     
        // Raycast downwards
        RaycastHit hitDown;
        if (Physics.Raycast(transform.position, Vector3.down, out hitDown, Mathf.Infinity))
        {
            // Visualize the downward raycast
            Debug.DrawRay(transform.position, Vector3.down * hitDown.distance, Color.green);

            if (hitDown.collider.CompareTag("WaterSurface"))
            {
                waterHeight = hitDown.point.y;
            }
        }

        // Apply floating force if below water height
        if (transform.position.y < waterHeight)
        {
            rb.AddForce(Vector3.up * floatForce, ForceMode.Acceleration);
        }
    }
}
