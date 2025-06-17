using System.Collections.Generic;
using UnityEngine;

public class Movimento : MonoBehaviour
{
    [HideInInspector] public bool wantsToJump;
    [Header("Running")]
    public bool canRun = true;
    [Range(0f, 15f)]
    public float speed = 5;
    public bool IsRunning { get; private set; }
    [Range(0f, 15f)]
    public float runSpeed = 9;
    public KeyCode runningKey = KeyCode.LeftShift;
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    private Rigidbody rb;
    private Transform myCamera;

    [Header("Rotação")]
    public bool rodaJunto = false;
    public float rotationSpeed = 10f;

    [HideInInspector] public Vector3 movimento;
    [HideInInspector] public Vector3 ondeOlha;

    [HideInInspector] public Vector2 inputMove;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        myCamera = Camera.main.transform;
    }

    void Update()
    {
        inputMove.x = Input.GetAxis("Horizontal");
        inputMove.y = Input.GetAxis("Vertical");

        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        Vector3 cameraForward = myCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 cameraRight = myCamera.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        movimento = (cameraRight * inputMove.x + cameraForward * inputMove.y).normalized;

        Vector3 targetVelocity = movimento * targetMovingSpeed;

        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

        if (rodaJunto)
        {
            ondeOlha = myCamera.forward;
            Olha();
        }
        else
        {
            ondeOlha = movimento;
            if (movimento != Vector3.zero)
            {
                Olha();
            }
        }

    }

    void Olha()
    {
        Quaternion targetRotation = Quaternion.LookRotation(ondeOlha);
        targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
