using UnityEngine;
public class Captain : ISoldierState
{
    [SerializeField] private bool IsActive = false;

    public float jumpStrength = 2;
    public event System.Action Jumped;

    [SerializeField, Tooltip("Prevents jumping when the transform is in mid-air.")]
    GroundCheck groundCheck;
    public override void OnEnter(SoldierManager soldierManager) {
        base.OnEnter(soldierManager);
        Debug.Log("Captain state entered.");

        IsActive = true;
    }
    public override void OnUpdate()
    {
    }
    public override void OnExit()
    {
        base.OnExit();
        Debug.Log("Captain state exited.");

        IsActive = false;
    }
    void Reset() {
        groundCheck = GetComponentInChildren<GroundCheck>();
    }

    public override void LateUpdate() {
        if (Input.GetButtonDown("Jump") && (!groundCheck || groundCheck.isGrounded) && IsActive) {
            _rigidBody.AddForce(Vector3.up * 100 * jumpStrength);
            Jumped?.Invoke();
        }
    }
}