using UnityEngine;

public class RiverCurrent : MonoBehaviour
{
    public Transform[] waypoints; // Assign in Inspector
    public float riverForce = 3f;
    public float driftStrength = 0.5f;

    private Rigidbody rb;
    private int currentWaypointIndex = 0;
    public bool isInWater = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered with: " + other.gameObject.name); // Debugging
        if (other.CompareTag("WaterSurface"))
        {
            Debug.Log("Ball has hit water!");
            isInWater = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger exited with: " + other.gameObject.name); // Debugging
        if (other.CompareTag("WaterSurface"))
        {
            Debug.Log("Ball left the water!");
            isInWater = false;
        }
    }

    void FixedUpdate()
    {
        if (!isInWater || waypoints.Length == 0) return;

        // Get direction to next waypoint
        Vector3 targetDirection = (waypoints[currentWaypointIndex].position - transform.position).normalized;

        // Apply force towards the waypoint
        rb.AddForce(targetDirection * riverForce, ForceMode.Acceleration);

        // Add slight Perlin noise drift
        float driftX = Mathf.PerlinNoise(Time.time, 0) * driftStrength - (driftStrength / 2);
        float driftZ = Mathf.PerlinNoise(0, Time.time) * driftStrength - (driftStrength / 2);
        rb.AddForce(new Vector3(driftX, 0, driftZ), ForceMode.Acceleration);

        // Check if close to waypoint, move to the next
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 2f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0; // Loop or remove object
            }
        }
    }
}

