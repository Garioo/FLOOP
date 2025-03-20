using UnityEngine;
using UnityEngine.InputSystem;

namespace StarterAssets
{
    public class PlayerPickupDrop : MonoBehaviour
    {
        [SerializeField] private Transform playerCameraTransform;
        [SerializeField] private LayerMask pickupLayerMask;
        
        private StarterAssetsInputs _input;

        private void Start()
        {
            _input = GetComponent<StarterAssetsInputs>();
        }

        private void Update()
        {
            // This bool is only true on the single frame the button is first pressed
            if (_input.pickup)
            {
                _input.pickup = false; // reset immediately so it doesn’t repeat

                float pickupDistance = 2f;
                Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward * pickupDistance, Color.red, 1f);

                if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward,
                                    out RaycastHit hit, pickupDistance, pickupLayerMask))
                {
                    Debug.Log("Picked up: " + hit.transform.name);
                    // Insert pickup logic here
                }
            }
        }
    }
}