using UnityEngine;

public class RiverCurrent : MonoBehaviour
{
    public float riverForce = 3f;
    public float driftStrength = 0.5f;

    private Rigidbody rb;
    private int currentWaypointIndex = 0;

    private int waterTriggerCount = 0;
    public bool isInWater => waterTriggerCount > 0;

    //REPELFORCE
    public float repelForce = 5f;
    public float repelDistance = 2f;

    public Transform[] waypoints; // Assign in Inspector
    public GameObject waypointParent;

    public float waypointDistance = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        waypoints = new Transform[waypointParent.transform.childCount];
        for (int i = 0; i < waypointParent.transform.childCount; i++)
        {
            waypoints[i] = waypointParent.transform.GetChild(i);
        }
    }

    void OnTriggerEnter(Collider other)
    {
     //   Debug.Log("Trigger entered with: " + other.gameObject.name);
        if (other.CompareTag("WaterSurface"))
        {
       //     Debug.Log("Ball has hit water!");
            waterTriggerCount++;

            Vector3 forward = transform.forward;
            float bestScore = float.MinValue;
            int bestIndex = 0;

            for (int i = 0; i < waypoints.Length; i++)
            {
                Vector3 toWaypoint = (waypoints[i].position - transform.position).normalized;
                float dot = Vector3.Dot(forward, toWaypoint);

                if (dot > 0)
                {
                    float distance = Vector3.Distance(transform.position, waypoints[i].position);
                    float score = dot * 2f - distance;
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestIndex = i;
                    }
                }
            }

            // After finding the best forward waypoint, immediately move to the next
            currentWaypointIndex = (bestIndex + 1) % waypoints.Length;
        }
    }

    void OnTriggerExit(Collider other)
    {
      //  Debug.Log("Trigger exited with: " + other.gameObject.name);
        if (other.CompareTag("WaterSurface"))
        {
        //    Debug.Log("Ball left the water!");
            waterTriggerCount = Mathf.Max(0, waterTriggerCount - 1);
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
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < waypointDistance)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0; // Loop
            }
        }

        // Repel force
        GameObject[] floopObjects = GameObject.FindGameObjectsWithTag("Floop");

        foreach (GameObject floop in floopObjects)
        {
            float distance = Vector3.Distance(transform.position, floop.transform.position);
            if (distance < repelDistance && distance > 0.1f)
            {
                Vector3 repelDirection = (transform.position - floop.transform.position).normalized;
                float forceAmount = repelForce * (1 - (distance / repelDistance));
                rb.AddForce(repelDirection * forceAmount, ForceMode.Force);
            }
        }

        // Dampen velocity
        rb.linearVelocity *= 0.95f;
    }
}
