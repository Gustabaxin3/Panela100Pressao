using UnityEngine;
public class Captain : ISoldierState
{
    [SerializeField] private bool _IsActive = false;

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

        _IsActive = true;
    }
    public override void OnUpdate()
    {
    }
    public override void OnExit()
    {
        base.OnExit();

        _IsActive = false;
    }
    public override void LateUpdate() {
        if (Input.GetButtonDown("Jump") && (!_groundCheck || _groundCheck.isGrounded) && _IsActive) {
            _rigidBody.AddForce(Vector3.up * 100 * _jumpStrength);
            Jumped?.Invoke();
        }
    }
}