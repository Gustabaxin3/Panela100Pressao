using AUDIO;
using UnityEngine;
public class Captain : ISoldierState
{
    [SerializeField] private bool _isActive = false;

    [SerializeField] private float _jumpStrength = 2;
    [SerializeField] private event System.Action Jumped;

    [SerializeField] private GroundCheck _groundCheck;

    protected override void Start() {
        base.Start();
        _groundCheck = GetComponentInChildren<GroundCheck>();
    }
    public override void OnEnter(SoldierManager soldierManager) {
        base.OnEnter(soldierManager);
        Debug.Log("Captain state entered.");

        _isActive = true;
    }
    public override void LateUpdate() {
        if (Input.GetButtonDown("Jump") && (!_groundCheck || _groundCheck.isGrounded) && _isActive) {
            AudioEvents.OnPlayerJump?.Invoke();
            _rigidBody.AddForce(_jumpStrength * 100 * Vector3.up);
            Jumped?.Invoke();
        }
    }
    public override void OnExit()
    {
        base.OnExit();

        _isActive = false;
    }
}