using UnityEngine;

public class RiverCurrent : MonoBehaviour
{
    public Transform[] waypoints; // Assign in Inspector
    public float riverForce = 3f;
    public float driftStrength = 0.5f;

    [SerializeField] private Rigidbody rb;
    private int currentWaypointIndex = 0;
    public bool isInWater = false;

    //REPELFORCE
    public float repelForce = 5f;
    public float repelDistance = 2f;

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


        //REPELFORCE
        GameObject[] floopObjects = GameObject.FindGameObjectsWithTag("Floop");

        foreach (GameObject floop in floopObjects)
        {
            float distance = Vector3.Distance(transform.position, floop.transform.position);
            if (distance < repelDistance && distance > 0.1f)
            {
                Vector3 repelDirection = (transform.position - floop.transform.position).normalized;
                float forceAmount = repelForce * (1 - (distance / repelDistance)); // Mindre kraft, jo l�ngere v?k
                rb.AddForce(repelDirection * forceAmount, ForceMode.Force);
            }
        }

        // D�mp farten for at forhindre uendelig acceleration
        rb.linearVelocity *= 0.95f;
    }
}

