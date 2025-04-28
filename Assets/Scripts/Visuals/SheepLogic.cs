using UnityEngine;

public class SheepLogic : MonoBehaviour
{
    public float detectionRadius = 5f;
    public float fleeDistance = 5f;
    public LayerMask ScaredLayer; // LayerMask for the objects that scare the sheep

    public float normalSpeed = 2f;
    public float panicSpeed = 5f;
    public float calmDownRate = 1f; // How quickly the sheep calms down

    private UnityEngine.AI.NavMeshAgent agent;
    private float currentSpeed;
    private bool isPanicking;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        currentSpeed = normalSpeed;
        agent.speed = currentSpeed;
    }

    void Update()
    {
        Collider[] threats = Physics.OverlapSphere(transform.position, detectionRadius, ScaredLayer);

        if (threats.Length > 0)
        {
            // Sheep gets scared
            isPanicking = true;
            agent.speed = panicSpeed;

            // Find closest threat
            Collider closestThreat = threats[0];
            float closestDistance = Vector3.Distance(transform.position, closestThreat.transform.position);

            foreach (Collider threat in threats)
            {
                float distance = Vector3.Distance(transform.position, threat.transform.position);
                if (distance < closestDistance)
                {
                    closestThreat = threat;
                    closestDistance = distance;
                }
            }

            // Flee away with slight randomness
            Vector3 fleeDirection = (transform.position - closestThreat.transform.position).normalized;
            fleeDirection += new Vector3(Random.Range(-0.2f, 0.2f), 0, Random.Range(-0.2f, 0.2f)); // add randomness
            fleeDirection.Normalize();

            Vector3 fleeTarget = transform.position + fleeDirection * fleeDistance;

            // Move to flee target
            if (agent.isOnNavMesh)
            {
                agent.SetDestination(fleeTarget);
            }
        }
        else
        {
            if (isPanicking)
            {
                // Calm down over time
                currentSpeed = Mathf.MoveTowards(agent.speed, normalSpeed, calmDownRate * Time.deltaTime);
                agent.speed = currentSpeed;

                if (Mathf.Approximately(agent.speed, normalSpeed))
                {
                    isPanicking = false;
                    agent.ResetPath(); // Stop moving after calm
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

