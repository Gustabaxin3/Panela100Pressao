using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TrampolineLauncher : MonoBehaviour {
    [SerializeField] private float launchForce = 25f;
    [SerializeField] private float standTimeToLaunch = 1f;

    private Collider _collider;
    private Transform _player;
    private Rigidbody _playerRb;
    private float _timeInside;
    private bool _playerInside;

    private TrampolineTrigger _trampolimTrigger;

    private void Awake() {
        _collider = GetComponent<Collider>();
        _trampolimTrigger = GetComponent<TrampolineTrigger>();
        _collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other) {
        if (!_trampolimTrigger.IsActivated) return;
        if (other.TryGetComponent<ISoldierState>(out var soldier)) {
            _player = soldier.transform;
            _playerRb = soldier.GetComponent<Rigidbody>();
            _timeInside = 0f;
            _playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (!_trampolimTrigger.IsActivated) return;
        if (_player != null && other.transform == _player) {
            _playerInside = false;
            _player = null;
            _playerRb = null;
            _timeInside = 0f;
        }
    }

    private void Update() {
        if (!_trampolimTrigger.IsActivated || !_playerInside || _playerRb == null) return;

        Vector3 horizontalVel = new Vector3(_playerRb.linearVelocity.x, 0f, _playerRb.linearVelocity.z);
        if (horizontalVel.magnitude < 0.1f) {
            _timeInside += Time.deltaTime;
            if (_timeInside >= standTimeToLaunch) {
                LaunchPlayer();
                _timeInside = 0f;
            }
        } else {
            _timeInside = 0f;
        }
    }

    private void LaunchPlayer() {
        Vector3 vel = _playerRb.linearVelocity;
        _playerRb.linearVelocity = new Vector3(vel.x, 0f, vel.z);
        _playerRb.AddForce(Vector3.up * launchForce, ForceMode.VelocityChange);
    }
}
