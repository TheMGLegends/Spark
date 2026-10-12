using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField, Min(0.1f)] private Vector2 movementSpeedBounds = new(0.1f, 10.0f);
    [SerializeField] private float movementSpeed = 1.0f;

    [Header("Speed Adjustment Settings")]
    [SerializeField, Range(0.1f, 1.0f)] private float adjustmentSpeed = 0.4f;

    private IA_TopDown inputActions;

#if UNITY_EDITOR
    private void OnValidate()
    {
        ValidateMinMaxMovementSpeeds();
        ValidateMovementSpeed();
    }

    private void ValidateMinMaxMovementSpeeds()
    {
        if (movementSpeedBounds.y < movementSpeedBounds.x)
        {
            movementSpeedBounds.y = movementSpeedBounds.x + 1.0f;
        }
    }
#endif

    private void Awake()
    {
        inputActions = new IA_TopDown();
        inputActions.Player.Enable();
    }

    private void OnEnable()
    {
        inputActions.Player.Zoom.performed += OnZoom;
        inputActions.Player.AdjustSpeed.performed += OnAdjustSpeed;
    }

    private void OnDisable()
    {
        inputActions.Player.Zoom.performed -= OnZoom;
        inputActions.Player.AdjustSpeed.performed -= OnAdjustSpeed;
    }

    private void Update()
    {
        OnMove();
    }

    private void OnMove()
    {
        Vector2 inputVector = inputActions.Player.Move.ReadValue<Vector2>();
        transform.position += movementSpeed * Time.deltaTime * new Vector3(inputVector.x, 0.0f, inputVector.y);
    }

    private void OnZoom(InputAction.CallbackContext context)
    {
        float input = inputActions.Player.Zoom.ReadValue<float>();
        Debug.Log("zooming: " + input);
        // TODO: Have it so it lerps smoothly
    }

    private void OnAdjustSpeed(InputAction.CallbackContext context)
    {
        float input = inputActions.Player.AdjustSpeed.ReadValue<float>();
        movementSpeed += adjustmentSpeed * input;
        ValidateMovementSpeed();
    }

    private void ValidateMovementSpeed()
    {
        movementSpeed = Mathf.Clamp(movementSpeed, movementSpeedBounds.x, movementSpeedBounds.y);
    }
}
