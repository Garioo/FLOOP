using UnityEngine;

public abstract class ObjectBehaviorParrent : MonoBehaviour
{
    private Vector3 targetPosition;
    public bool isPlaying = false;

    // -----------------------------
    // 1) Your abstract methods
    // -----------------------------
    public abstract void PlayOn();
    public abstract void PlayOff();

    // -----------------------------
    // 2) Store target position
    // -----------------------------
    public void StorePosition(Vector3 position)
    {
        targetPosition = position;
    }

    // -----------------------------
    // 3) ReturnObject method
    // -----------------------------
    public void ReturnObject()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // EXAMPLE: Use a 45� launch angle. Adjust as needed
            float launchAngle = 45f;

            // Calculate the velocity vector needed
            Vector3 launchVelocity = CalculateBallisticVelocity(
                startPos: transform.position,
                targetPos: targetPosition,
                launchAngleDeg: launchAngle,
                gravity: 9.81f
            );

            if (launchVelocity != Vector3.zero)
            {
                // Option A: Directly set velocity
                rb.linearVelocity = launchVelocity;

                // Option B: Use impulse force instead
                // float mass = rb.mass;
                // rb.AddForce(launchVelocity * mass, ForceMode.Impulse);
            }
            else
            {
                Debug.LogWarning("No valid ballistic solution with that angle.");
            }
        }
    }

    // -----------------------------
    // 4) Helper method:
    //    Calculate ballistic velocity
    // -----------------------------
    private Vector3 CalculateBallisticVelocity(
        Vector3 startPos,
        Vector3 targetPos,
        float launchAngleDeg,
        float gravity = 9.81f)
    {
        // Convert angle to radians
        float theta = launchAngleDeg * Mathf.Deg2Rad;

        // Planar distance (ignoring y)
        float horizontalDist = Mathf.Sqrt(
            (targetPos.x - startPos.x) * (targetPos.x - startPos.x) +
            (targetPos.z - startPos.z) * (targetPos.z - startPos.z)
        );

        // Vertical difference
        float deltaY = targetPos.y - startPos.y;

        // Edge case: if horizontally negligible, can't really do ballistic
        if (horizontalDist < 0.01f)
        {
            // For example, could return straight up or zero
            return Vector3.zero;
        }

        // We use the formula:
        // v^2 = g * R^2 / (2 * cos^2(theta) * (R * tan(theta) - deltaY))
        float denom = horizontalDist * Mathf.Tan(theta) - deltaY;
        if (denom <= 0f)
        {
            // No solution if the angle is too shallow to reach that height
            return Vector3.zero;
        }

        float numerator = gravity * (horizontalDist * horizontalDist);
        float denominator = 2f * Mathf.Cos(theta) * Mathf.Cos(theta) * denom;
        float vSquared = numerator / denominator;

        // If we get a negative or NaN, no valid solution
        if (vSquared <= 0f)
        {
            return Vector3.zero;
        }

        // Final launch speed
        float v = Mathf.Sqrt(vSquared);

        // Direction in the x-z plane
        Vector3 dir = new Vector3(
            targetPos.x - startPos.x,
            0f,
            targetPos.z - startPos.z
        ).normalized;

        // Decompose velocity into horizontal & vertical
        float vHorizontal = v * Mathf.Cos(theta);

        // Build the 3D velocity (Unity y-axis is up)
        Vector3 launchVelocity = dir * vHorizontal;
        launchVelocity.y = v * Mathf.Sin(theta);

        return launchVelocity;
    }
}
