using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class SheepLogic : MonoBehaviour
{
    public float detectionRadius = 5f;
    public float fleeDistance = 5f;
    public LayerMask ScaredLayer;

    public float normalSpeed = 2f;
    public float panicSpeed = 5f;
    public float calmDownRate = 1f;

    private NavMeshAgent agent;
    private Animator animator;

    private float currentSpeed;
    private bool isPanicking;
    private bool isSitting;
    private bool isWaitingToSit;

    private float nextSitTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentSpeed = normalSpeed;
        agent.speed = currentSpeed;

        ScheduleNextSit();
    }

    void Update()
    {
        Collider[] threats = Physics.OverlapSphere(transform.position, detectionRadius, ScaredLayer);

        if (threats.Length > 0)
        {
            FleeFromThreats(threats);
        }
        else
        {
            if (isPanicking)
            {
                CalmDown();
            }
            else
            {
                HandleIdleBehaviors();
            }
        }

        // Update animation states
        animator.SetBool("isRunning", isPanicking);
        animator.SetBool("isSitting", isSitting);
        animator.SetBool("isEating", !isPanicking && !isSitting);
        animator.SetBool("isWalking", !isPanicking && !isSitting && agent.hasPath && agent.velocity.magnitude > 0.1f);
    }

    void FleeFromThreats(Collider[] threats)
    {
        if (isSitting) StopCoroutine("SitRoutine");
        isSitting = false;
        isPanicking = true;
        agent.speed = panicSpeed;

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

        Vector3 fleeDirection = (transform.position - closestThreat.transform.position).normalized;
        fleeDirection += new Vector3(Random.Range(-0.2f, 0.2f), 0, Random.Range(-0.2f, 0.2f));
        fleeDirection.Normalize();

        Vector3 fleeTarget = transform.position + fleeDirection * fleeDistance;

        if (agent.isOnNavMesh)
        {
            agent.SetDestination(fleeTarget);
        }
    }

    void CalmDown()
    {
        currentSpeed = Mathf.MoveTowards(agent.speed, normalSpeed, calmDownRate * Time.deltaTime);
        agent.speed = currentSpeed;

        if (Mathf.Approximately(agent.speed, normalSpeed))
        {
            isPanicking = false;
            agent.ResetPath();
        }
    }

    void HandleIdleBehaviors()
    {
        if (!isSitting && Time.time >= nextSitTime && !isWaitingToSit)
        {
            StartCoroutine(SitRoutine());
        }

        if (!isSitting && !agent.hasPath)
        {
            Vector3 randomDirection = Random.insideUnitSphere * 5f;
            randomDirection += transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, 5f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
    }

    IEnumerator SitRoutine()
    {
        isWaitingToSit = true;
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        isSitting = true;
        agent.ResetPath();

        float sitDuration = Random.Range(15f, 25f);
        yield return new WaitForSeconds(sitDuration);

        isSitting = false;
        isWaitingToSit = false;
        ScheduleNextSit();
    }

    void ScheduleNextSit()
    {
        nextSitTime = Time.time + Random.Range(10f, 20f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

