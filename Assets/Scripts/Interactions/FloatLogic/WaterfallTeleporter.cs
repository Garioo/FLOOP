using UnityEngine;
using System.Collections.Generic;

public class WaterfallTeleporter : MonoBehaviour
{
    public Transform targetPosition; // The location to teleport to
    public List<string> teleportableTags = new List<string>(); // List of tags that can be teleported

    private void Start()
    {
        if (targetPosition == null)
        {
            Debug.LogWarning("Target position is not assigned!");
        }
    }

   private void OnTriggerEnter(Collider other)
{
    if (teleportableTags.Contains(other.gameObject.tag)) // Check if object's tag is in the list
    {
        if (targetPosition != null)
        {
            other.gameObject.transform.position = targetPosition.position; // Only move the colliding object
            Debug.Log($"{other.gameObject.name} teleported.");
        }
        else
        {
            Debug.LogWarning("Target position is missing!");
        }
    }
}
}