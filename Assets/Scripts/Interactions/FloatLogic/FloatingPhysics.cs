using UnityEngine;

public class FloatingPhysics : MonoBehaviour
{
    public float floatForce = 9.81f; // Adjust to match gravity
    public float waterDrag = 1.5f; // Helps stabilize movement
    public float waterHeight = 5f;
    public Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = waterDrag; // Simulate water resistance
        rb.angularDamping = waterDrag;
    }

    private void FixedUpdate()
    {
        RaycastHit hitUp;
        if (Physics.Raycast(transform.position, Vector3.up, out hitUp, Mathf.Infinity))
        {
            waterHeight = hitUp.point.y;
            print("Water height above: " + waterHeight);
        }

        RaycastHit hitDown;
        if (Physics.Raycast(transform.position, Vector3.down, out hitDown, Mathf.Infinity))
        {
            waterHeight = hitDown.point.y;
            print("Water height below: " + waterHeight);
        }

        if (transform.position.y < waterHeight) // Adjust based on water level
        {
            rb.AddForce(Vector3.up * floatForce, ForceMode.Acceleration);
        }


    }

    
}