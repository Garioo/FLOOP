using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClampRayGrabDistance : MonoBehaviour
{
    public float maxGrabDistance = 2f;

    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor;

    void OnEnable()
    {
        var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        interactable.selectEntered.AddListener(OnSelectEntered);
        interactable.selectExited.AddListener(OnSelectExited);
    }

    void OnDisable()
    {
        var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        interactable.selectEntered.RemoveListener(OnSelectEntered);
        interactable.selectExited.RemoveListener(OnSelectExited);
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor;
    }

    void OnSelectExited(SelectExitEventArgs args)
    {
        interactor = null;
    }

    void LateUpdate()
    {
        if (interactor != null && interactor is UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor)
        {
            Vector3 handPosition = interactor.transform.position;
            Vector3 dir = transform.position - handPosition;
            float currentDistance = dir.magnitude;

            if (currentDistance > maxGrabDistance)
            {
                transform.position = handPosition + dir.normalized * maxGrabDistance;
            }
        }
    }
}
