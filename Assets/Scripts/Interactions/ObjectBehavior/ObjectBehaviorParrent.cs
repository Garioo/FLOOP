using UnityEngine;

public abstract class ObjectBehaviorParrent : MonoBehaviour
{


    private Vector3 targetPosition;

    public bool isPlaying = false;

    public abstract void PlayOn();
    public abstract void PlayOff();


    // Method to set the target position
    public void StorePosition(Vector3 position)
    {
        targetPosition = position;
    }

    public void ReturnObject()
    {

        Vector3 direction = (targetPosition - transform.position).normalized;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            float angleDegrees = 45f;

            Vector3 launchVelocity = CalculateBallisticVelocity(
                startPosition: transform.position,
                endPosition: targetPosition,
                angleDegrees: angleDegrees
            );

            rb.AddForce(launchVelocity * 2, ForceMode.Impulse);
            
        }
    }

    private Vector3 CalculateBallisticVelocity(Vector3 startPosition, Vector3 endPosition, float angleDegrees)
    {
        
        float g = Mathf.Abs(Physics.gravity.y);
       
        Vector3 planarStart = new Vector3(startPosition.x, 0, startPosition.z);
        Vector3 planarEnd = new Vector3(endPosition.x, 0, endPosition.z);
        float distance = Vector3.Distance(planarStart, planarEnd);

        float yOffset = endPosition.y - startPosition.y;

      
        float angleRad = angleDegrees * Mathf.Deg2Rad;
    
        float distanceTimesTan = distance * Mathf.Tan(angleRad);
        float cosAngleSq = Mathf.Pow(Mathf.Cos(angleRad), 2);

        float denominator = 2f * cosAngleSq * (distanceTimesTan + yOffset);
        float velocitySquare = (g * distance * distance) / denominator;

        if (velocitySquare <= 0f)
        {
      
            velocitySquare = 0f;
        }
        float initialVelocity = Mathf.Sqrt(velocitySquare);
   
        float vY = initialVelocity * Mathf.Sin(angleRad);  // Up/down
        float vXz = initialVelocity * Mathf.Cos(angleRad); // In the plane

      
        float angleToTarget = Mathf.Atan2(
            endPosition.z - startPosition.z,
            endPosition.x - startPosition.x
        );
        
        Quaternion horizRotation = Quaternion.Euler(0, Mathf.Rad2Deg * angleToTarget, 0);
        Vector3 launchVelocity = horizRotation * new Vector3(vXz, 0f, 0f);
        launchVelocity.y = vY;

        return launchVelocity;
    }

}
