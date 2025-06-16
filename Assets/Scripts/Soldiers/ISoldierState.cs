using UnityEngine;
using Unity.Cinemachine;
using Unity.VisualScripting;

public abstract class ISoldierState : MonoBehaviour {
    protected SoldierManager _soldierManager;

    protected SoldierMovement _soldierMovement;

    protected Rigidbody _rigidBody;
    protected Transform _transform;
    [SerializeField] protected CinemachineCamera _cinemachineCamera;

    protected virtual void Start() {
        _rigidBody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
        _soldierMovement = GetComponent<SoldierMovement>();
    }

    public virtual void OnEnter(SoldierManager soldierManager) {
        _soldierManager = soldierManager;
        _cinemachineCamera.Priority = 1;

        _soldierMovement.SetMovementEnabled(true);
    }

    public virtual void FixedUpdate() { }
    public virtual void OnUpdate() { }
    public virtual void LateUpdate() { }

    public virtual void OnExit() {
        _cinemachineCamera.Priority = 0;

        _soldierMovement.SetMovementEnabled(false);
    }
}
