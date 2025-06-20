using System.Collections;
using TMPro;
using UnityEngine;

public class RopeClimb : MonoBehaviour {
    [SerializeField] private float _climbSpeed = 5f;

    private Rigidbody _rigidBody;
    private bool _isClimbing = false;
    private Transform _ropeSegment;
    private SoldierMovement _soldierMovement;

    private void Awake() {
        _rigidBody = GetComponent<Rigidbody>();
        _soldierMovement = GetComponent<SoldierMovement>();
    }

    public void EnterRope(Transform segment) {
        _ropeSegment = segment;
        _isClimbing = true;
        _rigidBody.useGravity = false;
        _rigidBody.linearVelocity = Vector3.zero;

        _soldierMovement.SetMovementEnabled(false);

        InteractionHintUI.Instance.ShowHint("Pressione Espaço para sair da corda.");
    }

    public void ExitRope() {
        _isClimbing = false;
        _ropeSegment = null;
        _rigidBody.useGravity = true;

        _soldierMovement.SetMovementEnabled(true);

        InteractionHintUI.Instance.HideHint();
    }

    private void Update() {
        if (!_isClimbing) return;

        if (Input.GetKeyDown(KeyCode.Space)) {
            ExitRope();
            return;
        }

        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 move = Vector3.up * vertical * _climbSpeed;
        _rigidBody.linearVelocity = move;

        Vector3 alignedPos = new Vector3(_ropeSegment.position.x, transform.position.y, _ropeSegment.position.z);
        transform.position = Vector3.Lerp(transform.position, alignedPos, Time.deltaTime * 10f);
    }

    private void FixedUpdate() {
        if (!_isClimbing && _rigidBody.useGravity == false) {
            _rigidBody.useGravity = true;
        }
    }
}
