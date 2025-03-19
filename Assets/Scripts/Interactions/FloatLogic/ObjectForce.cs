using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 3f;
    public float repelForce = 5f;
    public float repelDistance = 2f; // Max distance for repelling
    private int currentWaypointIndex = 0;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (waypoints.Length == 0)
            return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // Loop back to the start
        }

        GameObject[] floopObjects = GameObject.FindGameObjectsWithTag("Floop");

        foreach (GameObject floop in floopObjects)
        {
            float distance = Vector3.Distance(transform.position, floop.transform.position);
            if (distance < repelDistance && distance > 0.1f)
            {
                Vector3 repelDirection = (transform.position - floop.transform.position).normalized;
                float forceAmount = repelForce * (1 - (distance / repelDistance)); // Mindre kraft, jo længere v�k
                rb.AddForce(repelDirection * forceAmount, ForceMode.Force);
            }
        }

        // Dæmp farten for at forhindre uendelig acceleration
        rb.linearVelocity *= 0.95f;
    }
}
