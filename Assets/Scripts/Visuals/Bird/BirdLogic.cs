using UnityEngine;

public class BirdLogic : MonoBehaviour
{
    public string threatTag = "Floop"; // Tag of the object to flee from
    public float detectionRadius = 5f;
    public float flightSpeed = 5f;
    public float noiseAmount = 1f;
    public float rotationSpeed = 5f;
    public bool isFlying = false;
    private Animator animator;

    private Vector3 flightDirection;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isFlying)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
            foreach (Collider hit in hits)
            {
                if (hit.CompareTag(threatTag))
                {
                    FlyAway(hit.transform.position); // Pass the position of the threat
                    break;
                }
            }
        }
        else
        {
            transform.position += flightDirection * flightSpeed * Time.deltaTime;

            if (flightDirection != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(flightDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    void FlyAway(Vector3 threatPosition)
    {
        animator.SetTrigger("Fly");
        isFlying = true;

        Vector3 awayDirection = (transform.position - threatPosition).normalized;
        Vector3 randomOffset = new Vector3(
            Random.Range(-noiseAmount, noiseAmount),
            Random.Range(0.5f, 1.5f),
            Random.Range(-noiseAmount, noiseAmount)
        );
        flightDirection = (awayDirection + randomOffset).normalized;
    }
}


