using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoldierMovement : MonoBehaviour {
    [HideInInspector] public bool WantsToJump;

    [Header("Running")]
    public bool CanRun = true;

    [Range(0f, 15f)]
    public float WalkSpeed = 5f;

    [Range(0f, 15f)]
    public float RunSpeed = 9f;

    public bool IsRunning { get; private set; }

    public KeyCode RunningKey = KeyCode.LeftShift;
    public List<Func<float>> SpeedOverrides = new List<Func<float>>();

    private Rigidbody _rigidbody;
    private Transform _cameraTransform;

    [Header("Rotation")]
    public bool RotateWithCamera = false;

    [Range(0f, 20f)]
    public float RotationSpeed = 10f;

    [HideInInspector] public Vector3 MoveDirection;
    [HideInInspector] public Vector3 LookDirection;
    [HideInInspector] public Vector2 InputMove;
    [HideInInspector] public bool HeldJump;

    [SerializeField] private bool IsActive = false;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();

        _rigidbody.freezeRotation = true;
    }

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        _cameraTransform = Camera.main.transform;
    }

    private void FixedUpdate() {
        HandleInput();
    }

    private void HandleInput() {
        if (!IsActive) return;

        InputMove.x = Input.GetAxis("Horizontal");
        InputMove.y = Input.GetAxis("Vertical");
        IsRunning = CanRun && Input.GetKey(RunningKey);

        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement() {
        float targetSpeed = IsRunning ? RunSpeed : WalkSpeed;

        if (SpeedOverrides.Count > 0) {
            targetSpeed = SpeedOverrides[^1](); 
        }

        Vector3 cameraForward = _cameraTransform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        Vector3 cameraRight = _cameraTransform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();

        MoveDirection = (cameraRight * InputMove.x + cameraForward * InputMove.y).normalized;
        Vector3 targetVelocity = MoveDirection * targetSpeed;

        _rigidbody.linearVelocity = new Vector3(targetVelocity.x, _rigidbody.linearVelocity.y, targetVelocity.z);
    }

    private void HandleRotation() {
        if (RotateWithCamera) {
            LookDirection = _cameraTransform.forward;
        } else if (MoveDirection != Vector3.zero) {
            LookDirection = MoveDirection;
        } else {
            return;
        }

        RotateTowards(LookDirection);
    }

    private void RotateTowards(Vector3 direction) {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
    }

    public void SetMovementEnabled(bool enabled) => IsActive = enabled;
}
