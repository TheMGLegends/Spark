using System;
using UnityEngine;

public class TopDownController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Vector2 movementSpeedBounds;
    [SerializeField] private float movementSpeed;

    [Header("Zoom Settings")]
    [SerializeField] private Vector2 zoomBounds;
    [SerializeField, Min(0.1f)] private float zoomSpeed;

    [Header("Speed Adjustment Settings")]
    [SerializeField, Min(0.1f)] private float adjustmentSpeed;

    [Header("General Settings")]
    [SerializeField, Range(0.0f, 1.0f)] private float smoothTime;

    [Header("References")]
    [SerializeField] private new Camera camera;

    // Zoom Settings
    private float targetZoom;
    private float zoomVelocity;

    // Speed Adjustment Settings
    private float targetSpeed;
    private float speedVelocity;

    private IA_TopDown inputActions;

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
        inputActions = new IA_TopDown();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
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
        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();
        transform.position += movementSpeed * Time.deltaTime * new Vector3(input.x, 0.0f, input.y);
    }

    private void OnZoom()
    {
        float scroll = inputActions.Player.Zoom.ReadValue<float>();
        targetZoom += zoomSpeed * scroll;
        ClampAndSmoothZoom();
    }

    private void OnAdjustSpeed()
    {
        float scroll = inputActions.Player.AdjustSpeed.ReadValue<float>();
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
