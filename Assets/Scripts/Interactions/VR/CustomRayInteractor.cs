using UnityEngine;
using Oculus.Interaction;
using UnityEngine.Events;

public class CustomRayInteractor : RayInteractor
{
    // Define UnityEvents that pass the grabbed interactable.
    [System.Serializable]
    public class RayInteractableEvent : UnityEvent<RayInteractable> {}

    [Header("Custom Grab Events")]
    public RayInteractableEvent OnGrabbed;
    public RayInteractableEvent OnReleased;

    // Called when an interactable is selected (grabbed) by the ray.
    protected override void InteractableSelected(RayInteractable interactable)
    {
        // Call the base functionality (which moves the object, etc.)
        base.InteractableSelected(interactable);
        
        // Your custom behavior: for example, log and invoke the custom event.
        Debug.Log("Grabbed object: " + interactable.name);
        OnGrabbed?.Invoke(interactable);
    }

    // Called when an interactable is unselected (released) by the ray.
    protected override void InteractableUnselected(RayInteractable interactable)
    {
        // Call the base functionality first.
        base.InteractableUnselected(interactable);
        
        // Your custom behavior: log and invoke the custom event.
        Debug.Log("Released object: " + interactable.name);
        OnReleased?.Invoke(interactable);
    }
}