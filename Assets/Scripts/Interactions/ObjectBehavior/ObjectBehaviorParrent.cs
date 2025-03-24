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
    //    - Finds a ballistic velocity by scanning angles
    // -----------------------------
    public void ReturnObject()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // We'll scan angles from 10° to 80° in increments of 1°
            float minAngleDeg = 10f;
            float maxAngleDeg = 50f;
            float angleStep = 1f;
            float gravity = 9.81f;

            // Calculate the velocity vector by scanning possible angles
            Vector3 launchVelocity = CalculateBallisticVelocity(
                transform.position,   // start position
                targetPosition,       // target position
                minAngleDeg,
                maxAngleDeg,
                angleStep,
                gravity
            );

            Debug.Log("Launch velocity: " + launchVelocity);
            Debug.Log("Target position: " + targetPosition);
            Debug.Log("Start position: " + transform.position);

            if (launchVelocity != Vector3.zero)
            {
                // Option A: Directly set velocity
                 rb.linearVelocity = launchVelocity;

                // Option B: Use impulse force instead
                //float mass = rb.mass;
                //rb.AddForce(launchVelocity * mass, ForceMode.Impulse);
            }
            else
            {
                Debug.LogWarning("No valid ballistic solution found in angle range.");
            }
        }
    }

    // -----------------------------
    // 4) Method to scan angles
    //    until a valid solution is found.
    // -----------------------------
    private Vector3 CalculateBallisticVelocity(
        Vector3 startPos,
        Vector3 targetPos,
        float minAngleDeg,
        float maxAngleDeg,
        float angleStep,
        float gravity = 9.81f)
    {
        // Loop over angles from minAngleDeg to maxAngleDeg
        for (float angle = maxAngleDeg; angle >= minAngleDeg; angle -= angleStep)
        {
            // Try a single-angle solution
            Vector3 candidate = CalculateSingleAngleVelocity(startPos, targetPos, angle, gravity);

            // If it's valid (non-zero), return immediately
            if (candidate != Vector3.zero) return candidate;
            
        }

        // If no angle worked, return zero
        return Vector3.zero;
    }

    // -----------------------------
    // 5) Helper method to compute
    //    ballistic velocity for a single angle
    // -----------------------------
    private Vector3 CalculateSingleAngleVelocity(
        Vector3 startPos,
        Vector3 targetPos,
        float launchAngleDeg,
        float gravity)
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

        // Edge case: if horizontally negligible, can't do ballistic
        if (horizontalDist < 0.01f) return Vector3.zero;

        // Denominator in the standard ballistic formula
        //  v^2 = g*R^2 / [ 2*cos^2(theta)*(R*tan(theta) - deltaY ) ]
        float denom = horizontalDist * Mathf.Tan(theta) - deltaY;
        if (denom <= 0f)
        {
            // No solution if angle is too shallow to get that high
            return Vector3.zero;
        }

        float numerator = gravity * horizontalDist * horizontalDist;
        float denominator = 2f * Mathf.Cos(theta) * Mathf.Cos(theta) * denom;
        if (denominator <= 0f) return Vector3.zero;

        float vSquared = numerator / denominator;
        if (vSquared <= 0f) return Vector3.zero;

        float v = Mathf.Sqrt(vSquared);

        // Determine direction in the x-z plane
        Vector3 dir = new Vector3(
            targetPos.x - startPos.x,
            0f,
            targetPos.z - startPos.z
        ).normalized;

        // Decompose velocity into horizontal & vertical components
        float vHorizontal = v * Mathf.Cos(theta);
        Vector3 launchVelocity = dir * vHorizontal;
        launchVelocity.y = v * Mathf.Sin(theta);

        return launchVelocity;
    }
}
