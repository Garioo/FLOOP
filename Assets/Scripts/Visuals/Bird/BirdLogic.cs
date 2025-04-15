using UnityEngine;

public class BirdLogic : MonoBehaviour
{
    public Transform threatTarget; // Player or object to flee from
    public float detectionRadius = 5f;
    public float flightSpeed = 5f;
    public float noiseAmount = 1f;
    public float rotationSpeed = 5f;
    public bool isFlying = false;
    private Animator animator; // Reference to the Animator component

    private Vector3 flightDirection;
    void Start()
    {
        animator = GetComponent<Animator>(); // Initialize the Animator component
    }

    
    void Update()
    {
        if (!isFlying)
        {
            float distance = Vector3.Distance(transform.position, threatTarget.position);
            if (distance < detectionRadius)
            {
                FlyAway();
            }
        }
        else
        {
            // Fly in chosen direction
            transform.position += flightDirection * flightSpeed * Time.deltaTime;

            // Rotate toward flight direction
            if (flightDirection != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(flightDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    void FlyAway()
    {
        animator.SetTrigger("Fly"); // Trigger the fly animation
        isFlying = true;
       
        // Pick a random horizontal direction with some vertical lift and noise
        Vector3 awayDirection = (transform.position - threatTarget.position).normalized;
        Vector3 randomOffset = new Vector3(
            Random.Range(-noiseAmount, noiseAmount),
            Random.Range(0.5f, 1.5f), // Add upward lift
            Random.Range(-noiseAmount, noiseAmount)
        );
        flightDirection = (awayDirection + randomOffset).normalized;
    }
}

