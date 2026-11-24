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
                return;
            }

            if (TryGetPlaceable(out IPlaceable placeable, out RaycastHit hit))
            {
                placeable.OnPlace(hit, player.transform);
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

            if (TryGetHit(out RaycastHit hit))
            {
                hit.transform.TryGetComponent(out interactable);
            }

            return interactable != null;
        }

        private bool TryGetPlaceable(out IPlaceable placeable, out RaycastHit hit)
        {
            placeable = null;
            hit = default;

            if (!player || !player.IsHoldingItemType<IPlaceable>()) { return false; }

            return player.TryInstantiateHeldItem(out placeable) && TryGetHit(out hit);
        }

        private bool TryGetHit(out RaycastHit hit)
        {
            hit = default;
            return camera ? Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, interactDistance, interactableMask) : false;
        }
    }
}
