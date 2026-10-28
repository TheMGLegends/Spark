using System;
using UnityEngine;

namespace Spark
{
    public class TopDownController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private Vector2 movementSpeedBounds = new(1.0f, 10.0f);
        [SerializeField] private float movementSpeed = 1.0f;

        [Header("Zoom Settings")]
        [SerializeField] private Vector2 zoomBounds = new(30.0f, 90.0f);
        [SerializeField, Min(0.1f)] private float zoomSpeed = 4.0f;

        [Header("Speed Adjustment Settings")]
        [SerializeField, Min(0.1f)] private float adjustmentSpeed = 0.4f;

        [Header("General Settings")]
        [SerializeField, Range(0.0f, 1.0f)] private float smoothTime = 0.25f;

        [Header("References")]
        [SerializeField] private new Camera camera;

        // Zoom Settings
        private float targetZoom;
        private float zoomVelocity;

        // Speed Adjustment Settings
        private float targetSpeed;
        private float speedVelocity;

        private IA_TopDown topDown;

#if UNITY_EDITOR
        private void OnValidate()
        {
            ValidateMovementSpeedBounds();
            ValidateZoomBounds();
        }

        private void ValidateMovementSpeedBounds()
        {
            if (movementSpeedBounds.x > movementSpeedBounds.y)
            {
                movementSpeedBounds.x = movementSpeedBounds.y;
            }

            if (movementSpeedBounds.x < 0.1f)
            {
                movementSpeedBounds.x = 0.1f;
            }

            if (movementSpeedBounds.y < 0.1f)
            {
                movementSpeedBounds.y = 0.1f;
            }

            movementSpeed = Mathf.Clamp(movementSpeed, movementSpeedBounds.x, movementSpeedBounds.y);
        }

        private void ValidateZoomBounds()
        {
            if (zoomBounds.x > zoomBounds.y)
            {
                zoomBounds.x = zoomBounds.y;
            }

            if (zoomBounds.x < 0.1f)
            {
                zoomBounds.x = 0.1f;
            }

            if (zoomBounds.y < 0.1f)
            {
                zoomBounds.y = 0.1f;
            }

            if (zoomBounds.y > 179.0f)
            {
                zoomBounds.y = 179.0f;
            }
        }
#endif

        private void Awake()
        {
            topDown = new IA_TopDown();
        }

        private void OnEnable()
        {
            topDown.Player.Enable();
        }

        private void OnDisable()
        {
            topDown.Player.Disable();
        }

        private void Start()
        {
            targetZoom = camera ? camera.fieldOfView : 60.0f;
            targetSpeed = movementSpeed;
        }

        private void Update()
        {
            OnMove();
            OnZoom();
            OnAdjustSpeed();
        }

        private void OnMove()
        {
            Vector2 moveInput = topDown.Player.Move.ReadValue<Vector2>();
            transform.position += movementSpeed * Time.deltaTime * new Vector3(moveInput.x, 0.0f, moveInput.y);
        }

        private void OnZoom()
        {
            float scroll = topDown.Player.Zoom.ReadValue<float>();
            targetZoom += zoomSpeed * scroll;
            ClampAndSmoothZoom();
        }

        private void OnAdjustSpeed()
        {
            float scroll = topDown.Player.AdjustSpeed.ReadValue<float>();
            targetSpeed += adjustmentSpeed * scroll;
            ClampAndSmoothSpeed();
        }

        private void ClampAndSmoothZoom()
        {
            if (!camera) { return; }

            targetZoom = Mathf.Clamp(targetZoom, zoomBounds.x, zoomBounds.y);
            camera.fieldOfView = Mathf.SmoothDamp(camera.fieldOfView, targetZoom, ref zoomVelocity, smoothTime);
        }

        private void ClampAndSmoothSpeed()
        {
            targetSpeed = Mathf.Clamp(targetSpeed, movementSpeedBounds.x, movementSpeedBounds.y);
            movementSpeed = Mathf.SmoothDamp(movementSpeed, targetSpeed, ref speedVelocity, smoothTime);
        }
    }
}
