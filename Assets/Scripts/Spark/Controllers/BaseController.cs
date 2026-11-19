using UnityEngine;
using UnityEngine.InputSystem;

namespace Spark
{
    [RequireComponent(typeof(Player))]
    public abstract class BaseController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField, Min(0.1f)] protected float movementSpeed = 2.0f;

        [Header("Interaction Settings")]
        [SerializeField] private LayerMask interactableMask;
        [SerializeField, Min(0.1f)] private float interactDistance = Mathf.Infinity;

        protected new Camera camera;
        protected Player player;

        protected virtual void Awake()
        {
            camera = Camera.main;
            player = GetComponent<Player>();
        }

        protected void OnInteract(InputAction.CallbackContext context)
        {
            if (TryGetInteractable(out IInteractable interactable))
            {
                interactable.OnInteract(player);
            }
        }

        protected void OnSecondaryInteract(InputAction.CallbackContext context)
        {
            if (TryGetInteractable(out IInteractable interactable))
            {
                interactable.OnSecondaryInteract(player);
            }
        }

        protected void OnHotbarSelect(InputAction.CallbackContext context)
        {
            if (!player) { return; }

            int slotIndex = Mathf.RoundToInt(context.ReadValue<float>());
            player.SelectHotbarSlot(slotIndex);
        }

        private bool TryGetInteractable(out IInteractable interactable)
        {
            interactable = null;
            if (!camera) { return false; }

            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit, interactDistance, interactableMask))
            {
                hit.transform.TryGetComponent(out interactable);
            }

            return interactable != null;
        }
    }
}
