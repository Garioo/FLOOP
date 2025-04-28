using UnityEngine;

public class FloopKurvBehavior : MonoBehaviour
{
    public void OnKurvPlayOff()
    {
        // Get all colliders inside the trigger box collider
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Floop")) // Ensure the tag matches exactly
            {
       
                FloopBehavior floopBehavior = col.GetComponent<FloopBehavior>();
                {
          
                    floopBehavior.PlayOff();
                }
            }
        }
    }
}
