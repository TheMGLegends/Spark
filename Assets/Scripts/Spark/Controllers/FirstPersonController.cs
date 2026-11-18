using System;
using UnityEngine;

namespace Spark
{
    [RequireComponent(typeof(Rigidbody))]
    public class FirstPersonController : BaseController
    {
        [Header("Look Settings")]
        [SerializeField, Range(0.01f, 1.0f)] private float mouseSensitivity = 0.1f;
        [SerializeField] private Vector2 mouseRange = new(-90.0f, 90.0f);

        private new Rigidbody rigidbody;
        private IA_FirstPerson firstPerson;

        // Movement Settings
        private Vector2 moveInput;

        // Look Settings
        private float horizontalRotation;
        private float verticalRotation;

#if UNITY_EDITOR
        private void OnValidate()
        {
            ValidateMouseRange();
        }

        private void ValidateMouseRange()
        {
            if (mouseRange.x > mouseRange.y)
            {
                mouseRange.x = mouseRange.y;
            }
        }
#endif

        protected override void Awake()
        {
            base.Awake();

            rigidbody = GetComponent<Rigidbody>();
            rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rigidbody.freezeRotation = true;

            firstPerson = new IA_FirstPerson();
        }

        private void OnEnable()
        {
            firstPerson.Player.Enable();
            firstPerson.Player.Interact.performed += OnInteract;
            firstPerson.Player.HotbarSelect.performed += OnHotbarSelect;
        }

        private void OnDisable()
        {
            firstPerson.Player.HotbarSelect.performed -= OnHotbarSelect;
            firstPerson.Player.Interact.performed -= OnInteract;
            firstPerson.Player.Disable();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            GetMoveInput();
        }

        private void FixedUpdate()
        {
            OnMove();
        }

        private void LateUpdate()
        {
            OnLook();
        }

        private void OnMove()
        {
            Vector3 moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
            rigidbody.linearVelocity = moveDirection.normalized * movementSpeed;
        }

        private void OnLook()
        {
            if (!camera) { return; }

            Vector2 lookInput = firstPerson.Player.Look.ReadValue<Vector2>();

            horizontalRotation += lookInput.x * mouseSensitivity;
            rigidbody.MoveRotation(Quaternion.Euler(0.0f, horizontalRotation, 0.0f));

            verticalRotation -= lookInput.y * mouseSensitivity;
            verticalRotation = Mathf.Clamp(verticalRotation, mouseRange.x, mouseRange.y);
            camera.transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0.0f);
        }

        private void GetMoveInput()
        {
            moveInput = firstPerson.Player.Move.ReadValue<Vector2>();
        }
    }
}
