using UnityEngine;

public class FloatingPhysics : MonoBehaviour
{
    public float floatForce = 9.81f; // Adjust to match gravity
    public float waterDrag = 1.5f; // Helps stabilize movement
    public float waterHeight = 5f;
    [SerializeField] private Rigidbody rb;

    private void Start()
    {
        rb.linearDamping = waterDrag; // Simulate water resistance
        rb.angularDamping = waterDrag;
    }

    private void FixedUpdate()
    {
        RaycastHit hitUp;
        if (Physics.Raycast(transform.position, Vector3.up, out hitUp, Mathf.Infinity))
        {
           if (hitUp.collider.CompareTag("WaterSurface"))
            {
            waterHeight = hitUp.point.y;
            print(waterHeight);
            }
        }

        RaycastHit hitDown;
        if (Physics.Raycast(transform.position, Vector3.down, out hitDown, Mathf.Infinity))
        {
            if (hitDown.collider.CompareTag("WaterSurface"))
            {
            waterHeight = hitDown.point.y;
            print(waterHeight);
            }
        }

        if (transform.position.y < waterHeight) // Adjust based on water level
        {
            rb.AddForce(Vector3.up * floatForce, ForceMode.Acceleration);
        }


    }

    
}