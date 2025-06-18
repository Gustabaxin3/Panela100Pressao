using UnityEngine;

public class RopeClimb : MonoBehaviour {
    [SerializeField] private float climbSpeed = 5f;

    private Rigidbody rb;
    private bool isClimbing = false;
    private Transform ropeSegment;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    public void EnterRope(Transform segment) {
        ropeSegment = segment;
        isClimbing = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
    }

    public void ExitRope() {
        isClimbing = false;
        ropeSegment = null;
        rb.useGravity = true;
    }

    private void Update() {
        if (!isClimbing) return;

        if (Input.GetKeyDown(KeyCode.Space)) {
            ExitRope();
            return;
        }

        float vertical = Input.GetAxisRaw("Vertical"); // W = 1, S = -1, nada = 0

        Vector3 move = Vector3.up * vertical * climbSpeed;
        rb.linearVelocity = move;

        Vector3 alignedPos = new Vector3(ropeSegment.position.x, transform.position.y, ropeSegment.position.z);
        transform.position = Vector3.Lerp(transform.position, alignedPos, Time.deltaTime * 10f);
    }

    private void FixedUpdate() {
        if (!isClimbing && rb.useGravity == false) {
            rb.useGravity = true;
        }
    }
}
