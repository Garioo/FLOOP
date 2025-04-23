using UnityEngine;
using System.Collections;

public class BirdLogic : MonoBehaviour
{
    [Header("Detection")]
    public string threatTag = "Floop";
    public float detectionRadius = 5f;

    [Header("Flight Settings")]
    public float flightSpeed = 5f;
    public float rotationSpeed = 5f;
    public float noiseAmount = 1f;

    [Header("Circle Settings")]
    public float circleRadius = 5f;
    public float circleHeight = 2f;
    public float circleSpeed = 1f;

    private bool isFlying = false;
    private bool isReturning = false;
    private Vector3 flightDirection;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private float angle;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        startPosition = transform.position;
        startRotation = transform.rotation;
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
                    FlyAway(hit.transform.position);
                    break;
                }
            }
        }
        else
        {

            if (isReturning)
            {
                Vector3 toStart = (startPosition - transform.position).normalized;
                transform.position += toStart * flightSpeed * Time.deltaTime;
                RotateTowards(toStart);

                // Stop exactly at position
                if (Vector3.Distance(transform.position, startPosition) < 0.2f)
                {
                    transform.position = startPosition;
                    StartCoroutine(SmoothAlignToStart());
                }
            }
            else
            {
                angle += circleSpeed * Time.deltaTime;
                float x = Mathf.Cos(angle) * circleRadius;
                float z = Mathf.Sin(angle) * circleRadius;
                Vector3 targetPos = startPosition + new Vector3(x, circleHeight, z);
                flightDirection = (targetPos - transform.position).normalized;
                transform.position += flightDirection * flightSpeed * Time.deltaTime;

                RotateTowards(flightDirection);
            }
        }
    }

    void FlyAway(Vector3 threatPosition)
    {
        isFlying = true;
        isReturning = false;
        angle = Random.Range(0f, Mathf.PI * 2f); // random start angle
        animator.SetBool("Flying", true);
        Invoke(nameof(ReturnToOriginalPosition), Random.Range(8f, 15f)); // Delay before returning
    }

    void ReturnToOriginalPosition()
    {
        isReturning = true;
    }

    IEnumerator SmoothAlignToStart()
    {
        Quaternion currentRot = transform.rotation;
        float t = 0f;
        float duration = 1f;

        while (t < duration)
        {
            transform.rotation = Quaternion.Slerp(currentRot, startRotation, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        transform.rotation = startRotation;
        StopFlying();
    }

    void StopFlying()
    {
        isFlying = false;
        isReturning = false;
        animator.SetBool("Flying", false);
        flightDirection = Vector3.zero;
    }

    void RotateTowards(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }
}


