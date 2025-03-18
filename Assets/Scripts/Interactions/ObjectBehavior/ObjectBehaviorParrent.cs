using UnityEngine;

public abstract class ObjectBehaviorParrent : MonoBehaviour
{
    public abstract float Volume { get; set; }

    private Vector3 targetPosition;

    public abstract void PlayOn();
  
  


    // Method to set the target position
    public void StorePosition(Vector3 position)
    {
        targetPosition = position;
    }

    // Method to return the object to the target position
    public void ReturnObject() // Denne her skal kaldes når objektet er OOB!
    {
        // Calculate the direction to the target position
        Vector3 direction = (targetPosition - transform.position).normalized;

     
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Calculate the jump force
            float jumpForce = CalculateJumpForce(transform.position, targetPosition);
            rb.AddForce(direction * jumpForce, ForceMode.Impulse);
        }
    }
   
    // Helper method to calculate the jump force
    private float CalculateJumpForce(Vector3 startPosition, Vector3 endPosition)
    {
        // Calculate the distance to the target position
        float distance = Vector3.Distance(startPosition, endPosition);

        // Calculate the jump force based on the distance (this is a simple example, you may need to adjust it)
        float jumpForce = Mathf.Sqrt(distance) * 10f; // Adjust the multiplier as needed
        return jumpForce;
    }
}
