using UnityEngine;

public class RiverCurrent : MonoBehaviour
{
    public Transform[] waypoints; // Assign in Inspector
    public float riverForce = 3f;
    public float driftStrength = 0.5f;

    private Rigidbody rb;
    private int currentWaypointIndex = 0;
    public bool isInWater = false;
    private bool waypointInitialized = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered with: " + other.gameObject.name);
        if (other.CompareTag("WaterSurface"))
        {
            Debug.Log("Ball has hit water!");
            isInWater = true;

            // Find nearest waypoint only once when entering water
            if (!waypointInitialized)
            {
                Transform nearestWaypoint = FindNearestWaypoint(transform.position);
                if (nearestWaypoint != null)
                {
                    currentWaypointIndex = System.Array.IndexOf(waypoints, nearestWaypoint);
                    waypointInitialized = true;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger exited with: " + other.gameObject.name);
        if (other.CompareTag("WaterSurface"))
        {
            Debug.Log("Ball left the water!");
            isInWater = false;
        }
    }

    void FixedUpdate()
    {
        if (!isInWater || waypoints.Length == 0) return;

        // Move towards the current waypoint
        Vector3 targetDirection = (waypoints[currentWaypointIndex].position - transform.position).normalized;
        rb.AddForce(targetDirection * riverForce, ForceMode.Acceleration);

        // Apply slight Perlin noise drift
        float driftX = Mathf.PerlinNoise(Time.time, 0) * driftStrength - (driftStrength / 2);
        float driftZ = Mathf.PerlinNoise(0, Time.time) * driftStrength - (driftStrength / 2);
        rb.AddForce(new Vector3(driftX, 0, driftZ), ForceMode.Acceleration);

        // Check if close to waypoint, move to the next one in order
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 2f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0; // Loop waypoints
            }
        }
    }

    Transform FindNearestWaypoint(Vector3 position)
    {
        if (waypoints == null || waypoints.Length == 0) return null;

        Transform nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform waypoint in waypoints)
        {
            float distance = Vector3.Distance(position, waypoint.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = waypoint;
            }
        }

        return nearest;
    }
}
