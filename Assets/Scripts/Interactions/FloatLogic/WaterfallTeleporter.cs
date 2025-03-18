using UnityEngine;

public class WaterfallTeleporter : MonoBehaviour
{
    public Transform targetPosition; // The empty GameObject's position

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TeleportTrigger")) // Make sure the collider has this tag
        {
            if (targetPosition != null)
            {
                transform.position = targetPosition.position;
            }
            else
            {
                Debug.LogWarning("Target position is not assigned!");
            }
        }
    }
}
