// ChildColliderForwarder.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ChildColliderForwarder : MonoBehaviour
{
    [SerializeField] private ObjectManager objectManager;

    private void OnTriggerEnter(Collider other)
    {
        // Forward the collision event to the manager
        objectManager.ChildTriggerEnter(other);
    }
}
