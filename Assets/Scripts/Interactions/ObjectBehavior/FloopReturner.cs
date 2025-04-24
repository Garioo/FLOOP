using Unity.VisualScripting;
using UnityEngine;

public class FloopReturner : MonoBehaviour
{
    // Simon er en klovn

    private void OnTriggerEnter(Collider other)
    {
        FloopBehavior floopBehavior = other.GetComponent<FloopBehavior>();
        if (floopBehavior != null)
        {
            floopBehavior.ReturnObject();
        }
    }
}
