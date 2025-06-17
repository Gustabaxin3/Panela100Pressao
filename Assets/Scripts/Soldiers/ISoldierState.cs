using UnityEngine;
using Unity.Cinemachine;

public abstract class ISoldierState : MonoBehaviour {
    protected SoldierManager _soldierManager;
    protected SoldierMovement _soldierMovement;

    protected Rigidbody _rigidBody;
    protected Transform _transform;
    protected CinemachineCamera _cinemachineCamera;
    protected virtual void Awake() {
        _cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
        _soldierMovement = GetComponent<SoldierMovement>();
    }
    protected virtual void Start() {
        _rigidBody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
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
