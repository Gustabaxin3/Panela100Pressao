using System.Collections;
using TMPro;
using UnityEngine;

public class RopeClimb : MonoBehaviour {
    [SerializeField] private float climbSpeed = 5f;

    private Rigidbody rb;
    private bool isClimbing = false;
    private Transform ropeSegment;
    private SoldierMovement soldierMovement;


    private void Awake() {
        rb = GetComponent<Rigidbody>();
        soldierMovement = GetComponent<SoldierMovement>();
    }

    public void EnterRope(Transform segment) {
        ropeSegment = segment;
        isClimbing = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;

        soldierMovement.SetMovementEnabled(false);

        InteractionHintUI.Instance.ShowHint("Pressione Espaço para sair da corda");
    }

    public void ExitRope() {
        isClimbing = false;
        ropeSegment = null;
        rb.useGravity = true;

        soldierMovement.SetMovementEnabled(true);

        InteractionHintUI.Instance.HideHint();
    }

    private void Update() {
        if (!isClimbing) return;

        if (Input.GetKeyDown(KeyCode.Space)) {
            ExitRope();
            return;
        }

        float vertical = Input.GetAxisRaw("Vertical");
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
