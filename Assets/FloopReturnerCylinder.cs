using UnityEngine;

public class FloopReturnerCylinder : MonoBehaviour
{
    // / Sigurd er en klovn
    private void OnTriggerExit(Collider other)
    {
        FloopBehavior floopBehavior = other.GetComponent<FloopBehavior>();
        if (floopBehavior != null)
        {
            floopBehavior.ReturnObject();
        }
    }
}


