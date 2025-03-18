using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class PickupHandler : MonoBehaviour
{
    private Rigidbody rb;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        rb.isKinematic = true; // Stop physics while grabbed
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        rb.isKinematic = false; // Restore physics when dropped
    }
}