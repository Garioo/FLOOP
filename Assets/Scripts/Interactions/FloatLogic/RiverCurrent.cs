using UnityEngine;

public class RiverCurrent : MonoBehaviour
{
    public Transform[] waypoints; // Assign in Inspector
    public float riverForce = 3f;
    public float driftStrength = 0.5f;

    private Rigidbody rb;
    private int currentWaypointIndex = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (waypoints.Length == 0) return;

        // Get direction to next waypoint
        Vector3 targetDirection = (waypoints[currentWaypointIndex].position - transform.position).normalized;

        // Apply a forward force to push the object
        rb.AddForce(targetDirection * riverForce, ForceMode.Acceleration);

        // Add slight Perlin noise drift for realism
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
